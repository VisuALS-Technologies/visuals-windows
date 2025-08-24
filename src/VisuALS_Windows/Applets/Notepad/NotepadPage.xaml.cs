using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for Notepad.xaml
    /// </summary>
    public partial class NotepadPage : AppletPage
    {
        DocumentFolder NotesFolder;
        PeriodicBackgroundProcess AutoSave;
        List<string> LegacyNoteNames;

        /// <summary>
        /// Default constructor
        /// </summary>
        public NotepadPage()
        {
            InitializeComponent();
            ParentApplet = AppletManager.GetApplet<Notepad>();

            if (App.globalConfig.Get<bool>("compatibility_mode"))
            {
                LegacyNoteNames = File.ReadAllLines(AppPaths.DocumentsPath + "\\FileNames.txt").ToList();
                NotesFolder = new DocumentFolder(Config.Get<string>("notes_folder"), LegacyNamesFilter);
            }
            else
            {
                NotesFolder = new DocumentFolder(Config.Get<string>("notes_folder"), ExtensionFilter);
            }

            NotesFolder.FileWritten += NotesFolder_FileWritten;
            NotesFolder.FileDeleted += NotesFolder_FileDeleted;

            titleTextInput.Text = Session.Get<string>("title");
            textInput.Text = Session.Get<string>("text"); //set text before keyboard focus so that caret index is initialized at end of text

            fileOptions.Value = false;

            keyboard.Layout = KeyboardManager.GetCurrentLayout();
            keyboard.FocusElement = textInput;

            AutoSave = new PeriodicBackgroundProcess(AutoSaveRunFunction, 1000, this);
        }

        private void NotesFolder_FileWritten(object sender, DocumentFolderEventArgs e)
        {
            if (App.globalConfig.Get<bool>("compatibility_mode"))
            {
                if (!LegacyNoteNames.Contains(e.filename))
                {
                    LegacyNoteNames.Add(e.filename);
                    UpdateLegacyNoteNamesFile();
                }
            }
        }

        private void NotesFolder_FileDeleted(object sender, DocumentFolderEventArgs e)
        {
            if (App.globalConfig.Get<bool>("compatibility_mode"))
            {
                LegacyNoteNames.Remove(e.filename);
                UpdateLegacyNoteNamesFile();
            }
        }

        private bool LegacyNamesFilter(string filename)
        {
            return ExtensionFilter(filename) & LegacyNoteNames.Contains(Path.GetFileNameWithoutExtension(filename));
        }

        private bool ExtensionFilter(string filename)
        {
            return filename.EndsWith(".txt", StringComparison.OrdinalIgnoreCase);
        }

        private void UpdateLegacyNoteNamesFile()
        {
            File.WriteAllLines(AppPaths.DocumentsPath + "\\FileNames.txt", LegacyNoteNames.ToArray());
        }

        /// <summary>
        /// Called periodically to save current text and title in the text input boxes
        /// </summary>
        private void AutoSaveRunFunction()
        {
            this.Dispatcher.Invoke(() =>
            {
                Session.Set("text", textInput.Text);
                Session.Set("title", titleTextInput.Text);
                if (Config.Get<bool>("autosave"))
                {
                    save();
                }
            });
        }

        /// <summary>
        /// Event handler for the Title TextChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void titleTextInputHandler(object sender, TextChangedEventArgs e)
        {
            if (titleTextInput.Text.EndsWith("\n"))
            {
                titleTextInput.Text = titleTextInput.Text.TrimEnd();
                keyboard.FocusElement = textInput;
            }
        }

        /// <summary>
        /// Click handler for the Save button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            save();
            fileOptions.Value = false;
        }

        private async void save()
        {
            string invchars = FileUtils.getInvalidPathChars(titleTextInput.Text);
            if (invchars != "")
            {
                await DialogWindow.ShowMessage($"{LanguageManager.Tokens["np_illegal_file_name"]} {invchars}");
            }
            else
            {
                try
                {
                    if (NotesFolder.FileExists(titleTextInput.Text))
                    {
                        NotesFolder.WriteToFile(titleTextInput.Text, textInput.Text);
                    }
                    else
                    {
                        NotesFolder.NewFile(titleTextInput.Text, "txt", textInput.Text);
                    }
                }
                catch (Exception ex)
                {
                    await DialogWindow.ShowMessage(ex.Message);
                }
            }
        }

        /// <summary>
        /// Click handler for Open button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void openBtn_Click(object sender, RoutedEventArgs e)
        {
            List<string> notes = NotesFolder.GetFileNames();
            if (notes.Count() > 0)
            {
                DialogResponse r = await DialogWindow.ShowList(LanguageManager.Tokens["np_open_prompt"], notes);
                fileOptions.Value = false;
                if (r.ResponseObject != null)
                {
                    titleTextInput.Text = r.ResponseString;
                    textInput.Text = NotesFolder.GetFileContents(r.ResponseString);
                }
            }
            else
            {
                await DialogWindow.ShowMessage(LanguageManager.Tokens["np_no_notes"]);
            }
        }

        /// <summary>
        /// Click handler for New file button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void newBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSaved())
            {
                DialogResponse r = await DialogWindow.ShowMessage(LanguageManager.Tokens["np_save_changes"],
                    LanguageManager.Tokens["yes"], LanguageManager.Tokens["no"]);
                if (r.ResponseObject != null)
                {
                    if (r.ResponseString == LanguageManager.Tokens["yes"])
                    {
                        NotesFolder.WriteToFile(titleTextInput.Text, textInput.Text);
                    }
                    titleTextInput.Text = NotesFolder.GetNewFileName();
                    textInput.Text = "";
                }
            }
            else
            {
                titleTextInput.Text = NotesFolder.GetNewFileName();
                textInput.Text = "";
            }
            fileOptions.Value = false;
        }

        /// <summary>
        /// Click handler for Delete button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResponse r = await DialogWindow.ShowMessage($"{LanguageManager.Tokens["np_delete_confirmation"]} '{titleTextInput.Text}'?",
                LanguageManager.Tokens["yes"], LanguageManager.Tokens["no"]);
            if (r.ResponseString == LanguageManager.Tokens["yes"])
            {
                if (NotesFolder.FileExists(titleTextInput.Text))
                {
                    NotesFolder.DeleteFile(titleTextInput.Text);
                }
                textInput.Text = "";
                titleTextInput.Text = NotesFolder.GetNewFileName();
            }
            fileOptions.Value = false;
        }

        /// <summary>
        /// Click handler for Rename button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void renameBtn_Click(object sender, RoutedEventArgs e)
        {
            keyboard.FocusElement = titleTextInput;
            fileOptions.Value = false;
        }

        /// <summary>
        /// Checks if current text is saved under the current filename (ie. if the current note is saved)
        /// </summary>
        /// <returns></returns>
        private bool IsSaved()
        {
            if (NotesFolder.FileExists(titleTextInput.Text))
            {
                return textInput.Text == NotesFolder.GetFileContents(titleTextInput.Text);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Event handler for the FileOptions toggle button "OptionSelected" event.
        /// Runs when the buttons is toggled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileOptions_OptionSelected(object sender, RoutedEventArgs e)
        {
            if (fileOptions.Value)
            {
                fileMenu.Visibility = Visibility.Visible;
            }
            else
            {
                fileMenu.Visibility = Visibility.Hidden;
            }
        }
    }
}
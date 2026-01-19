using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for TextToSpeech.xaml
    /// </summary>
    public partial class TextToSpeechPage : AppletPage
    {
        DocumentFolder PhrasesFolder;
        PeriodicBackgroundProcess AutoSave;
        List<string> EnglishCategories = new List<string> { "Needs", "Feelings", "Conversation", "Home Automation" };
        List<string> CategoriesInOtherLanguages = new List<string> { LanguageManager.Tokens["tts_needs"], LanguageManager.Tokens["tts_feelings"],
            LanguageManager.Tokens["tts_conversation"], LanguageManager.Tokens["tts_home_automation"] };
        AudioRecorder recorder = new AudioRecorder();
        public DocumentFolder RecordingsFolder;

        /// <summary>
        /// Default constructor
        /// </summary>
        public TextToSpeechPage()
        {
            InitializeComponent();
            ParentApplet = AppletManager.GetApplet<TextToSpeech>();
            PhrasesFolder = new DocumentFolder(Config.Get<string>("phrases_folder"));
            RecordingsFolder = new DocumentFolder(AppletManager.GetApplet<Recorder>().Config.Get<string>("recordings_folder"));
            if (App.globalConfig.Get<bool>("compatibility_mode"))
            {
                EnglishCategories = new List<string> { "NeedsStrings", "FeelingsStrings", "ConversationStrings", "HomeAutomationStrings" };
            }
            foreach (string cat in EnglishCategories)
            {
                PhrasesFolder.NewFile(cat, "txt");
            }
            textfield.Text = Session.Get<string>("text"); //set text before keyboard focus so caret index is placed at end of text

            keyboard.Layout = KeyboardManager.GetCurrentLayout();
            keyboard.FocusElement = textfield;
            AutoSave = new PeriodicBackgroundProcess(AutoSaveRunFunction, 1000, this);
            speakBtn.PlaybackStopped += SpeakBtn_PlaybackStopped;
        }

        /// <summary>
        /// Called periodically to save current text in the text field
        /// </summary>
        private void AutoSaveRunFunction()
        {
            this.Dispatcher.Invoke(() =>
            {
                Session.Set("text", textfield.Text);
            });
        }

        private void AddRecent(string text)
        {
            List<string> recents = Session.Get<List<string>>("recents");
            if (recents.Contains(text))
            {
                recents.Remove(text);
            }
            recents.Insert(0, text);
            if (recents.Count > 10)
            {
                recents.Remove(recents.Last());
            }
            Session.Set("recents", recents);
        }

        /// <summary>
        /// Click handler for Speak button. This does not actually handle the speaking, 
        /// only saving to recents as the button is a VSpeakButton and handles speaking itself.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Speak_Click(object sender, RoutedEventArgs e)
        {
            AddRecent(textfield.Text);
        }

        /// <summary>
        /// Click handler for Recent button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Recent_Click(object sender, RoutedEventArgs e)
        {
            List<string> recents = Session.Get<List<string>>("recents");
            if (recents.Count() > 0)
            {
                DialogResponse r = await DialogWindow.ShowList(LanguageManager.Tokens["tts_recent"], recents, 4, 1);
                if (r.ResponseObject != null)
                {
                    speakBtn.TextSource = r.ResponseString;
                    speakBtn.RaiseEvent(new RoutedEventArgs(VSpeakButton.ClickEvent));
                    AddRecent(r.ResponseString);
                }
            }
            else
            {
                await DialogWindow.ShowMessage(LanguageManager.Tokens["tts_no_recents"]);
            }
        }

        private void SpeakBtn_PlaybackStopped(object sender, NAudio.Wave.StoppedEventArgs e)
        {
            speakBtn.TextSource = textfield;
        }

        /// <summary>
        /// Opens a phrase category in a dialog window list given the name of the category
        /// </summary>
        /// <param name="category"></param>
        private async void OpenCategory(string category)
        {
            string categoryName = CategoriesInOtherLanguages[EnglishCategories.IndexOf(category)];
            List<string> phrases = PhrasesFolder.GetFileAsList(category);

            if (phrases.Count() > 0)
            {
                DialogResponse r = await DialogWindow.ShowList(categoryName, phrases, 4, 1);
                if (r.ResponseObject != null)
                {
                    speakBtn.TextSource = r.ResponseString;
                    speakBtn.RaiseEvent(new RoutedEventArgs(VSpeakButton.ClickEvent));
                    AddRecent(r.ResponseString);
                }
            }
            else
            {
                await DialogWindow.ShowMessage(LanguageManager.Tokens["tts_category_has_no_phrases1"] + categoryName + LanguageManager.Tokens["tts_category_has_no_phrases2"]);
            }
        }

        /// <summary>
        /// Click handler for NeedsCategory button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NeedsCategory_Click(object sender, RoutedEventArgs e)
        {
            OpenCategory(EnglishCategories[0]);
        }

        /// <summary>
        /// Click handler for FeelingsCategory button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FeelingsCategory_Click(object sender, RoutedEventArgs e)
        {
            OpenCategory(EnglishCategories[1]);
        }

        /// <summary>
        /// Click handler for ConversationCategory button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConversationCategory_Click(object sender, RoutedEventArgs e)
        {
            OpenCategory(EnglishCategories[2]);
        }

        /// <summary>
        /// Click handler for HomeAutomationCategory button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HomeAutomationCategory_Click(object sender, RoutedEventArgs e)
        {
            OpenCategory(EnglishCategories[3]);
        }

        /// <summary>
        /// Click handler for RecordingsCategory button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RecordingsCategory_Click(object sender, RoutedEventArgs e)
        {
            List<string> phrases = RecordingsFolder.GetFileNames();
            if (phrases.Count() > 0)
            {
                DialogResponse r = await DialogWindow.ShowList("Recordings", phrases, 4, 1);
                if (r.ResponseObject != null)
                {
                    recorder.OpenFile(RecordingsFolder.GetFilePath(r.ResponseString), null);
                    recorder.Play();
                }
            }
            else
            {
                await DialogWindow.ShowMessage(LanguageManager.Tokens["tts_no_recordings"]);
            }
        }

        /// <summary>
        /// Click handler for AddPhrase button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddPhrase_Click(object sender, RoutedEventArgs e)
        {
            DialogResponse r = await DialogWindow.ShowList(LanguageManager.Tokens["tts_add_phrase_prompt"], CategoriesInOtherLanguages, 2, 2);
            if (r.ResponseObject != null)
            {
                string category = EnglishCategories[CategoriesInOtherLanguages.IndexOf(r.ResponseString)];
                PhrasesFolder.InsertLineIntoFile(category, 0, textfield.Text);
            }
        }

        /// <summary>
        /// Click handler for DeletePhrase button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DeletePhrase_Click(object sender, RoutedEventArgs e)
        {
            DialogResponse r = await DialogWindow.ShowList(LanguageManager.Tokens["tts_delete_from_category_prompt"], CategoriesInOtherLanguages, 2, 2);
            if (r.ResponseObject != null)
            {
                string category = EnglishCategories[CategoriesInOtherLanguages.IndexOf(r.ResponseString)];
                List<string> phrases = PhrasesFolder.GetFileAsList(category);
                r = await DialogWindow.ShowList(LanguageManager.Tokens["tts_delete_phrase_prompt"], phrases, 4, 1);
                if (r.ResponseObject != null)
                {
                    PhrasesFolder.RemoveLineFromFile(category, r.ResponseString);
                }
            }
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CarlsPause());
        }
    }
}

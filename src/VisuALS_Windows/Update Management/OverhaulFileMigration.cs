using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VisuALS_WPF_App
{
    static class OverhaulFileMigration
    {
        public static List<string> settingsLog = new List<string>();
        public static List<string> phrasesLog = new List<string>();
        public static List<string> bookmarksLog = new List<string>();
        public static List<string> notesLog = new List<string>();
        public static List<string> cleanupLog = new List<string>();
        public static event EventHandler OverhaulMigrationComplete;
        public static string PreV2VisuALSDocumentFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "VisuALS");

        public static bool PreV2VisuALSFilesPresent()
        {
            return Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\VisuALS_Technology_Soluti") ||
                Directory.Exists(PreV2VisuALSDocumentFolder + "\\VisuALS Components\\") ||
                File.Exists(PreV2VisuALSDocumentFolder + "\\FileNames.txt");
        }

        public static bool PreV2VisuALSAppPresent()
        {
            string programs_path = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
            IEnumerable<string> start_menu = Directory.GetDirectories(programs_path).Select(x => Path.GetFileName(x));
            return start_menu.Contains("VisuALS") && File.Exists(Path.Combine(programs_path, "VisuALS", "VisuALS.appref-ms"));
        }

        public static async Task StartFileMigrationProcess()
        {
            DialogWindow.DisableClose = true;
            DialogResponse response = null;
            bool migration_complete = false;

            if (PreV2VisuALSAppPresent())
            {
                response = await DialogWindow.ShowMessage(LanguageManager.Tokens["fm_keep_using_old_version?"], LanguageManager.Tokens["yes"], LanguageManager.Tokens["no"]);
                if (response.ResponseString == null)
                {
                    migration_complete = false;
                }
                else if (LanguageManager.KeyFromToken(response.ResponseString) == "no")
                {
                    response = await DialogWindow.ShowMessage(LanguageManager.Tokens["fm_suggest_uninstall_old_version"]);
                    DeleteOldSettings();
                    MoveFiles();
                    migration_complete = true;
                }
                else
                {
                    response = await DialogWindow.ShowMessage(LanguageManager.Tokens["fm_sync_files?"], LanguageManager.Tokens["yes"], LanguageManager.Tokens["no"]);

                    if (response.ResponseString == null)
                    {
                        migration_complete = false;
                    }
                    else if (LanguageManager.KeyFromToken(response.ResponseString) == "yes")
                    {
                        EnableCompatibilityMode();
                        migration_complete = true;
                    }
                    else
                    {
                        response = await DialogWindow.ShowMessage(LanguageManager.Tokens["fm_copy_files?"], LanguageManager.Tokens["yes"], LanguageManager.Tokens["no"]);
                        if (response.ResponseString == null)
                        {
                            migration_complete = false;
                        }
                        else if (LanguageManager.KeyFromToken(response.ResponseString) == "yes")
                        {
                            CopyFiles();
                            migration_complete = true;
                        }
                        else
                        {
                            migration_complete = true;
                        }
                    }
                }
            }
            else if (PreV2VisuALSFilesPresent())
            {
                response = await DialogWindow.ShowMessage(LanguageManager.Tokens["fm_move_files?"], LanguageManager.Tokens["yes"], LanguageManager.Tokens["no"]);
                if (response.ResponseString == null)
                {
                    migration_complete = false;
                }
                else if (LanguageManager.KeyFromToken(response.ResponseString) == "yes")
                {
                    DeleteOldSettings();
                    MoveFiles();
                    migration_complete = true;
                }
                else
                {
                    migration_complete = true;
                }
            }
            else
            {
                migration_complete = true;
            }

            if (PreV2VisuALSAppPresent() || PreV2VisuALSFilesPresent())
            {
                string logStr = "";
                logStr += CreateLog(LanguageManager.Tokens["fm_settings_file_error"], settingsLog);
                logStr += CreateLog(LanguageManager.Tokens["fm_phrases_file_error"], phrasesLog);
                logStr += CreateLog(LanguageManager.Tokens["fm_notes_file_error"], notesLog);
                logStr += CreateLog(LanguageManager.Tokens["fm_bookmarks_file_error"], bookmarksLog);
                logStr += CreateLog(LanguageManager.Tokens["fm_cleanup_file_error"], cleanupLog);

                if (logStr == "")
                {
                    logStr = LanguageManager.Tokens["fm_no_migration_errors"];
                    await DialogWindow.ShowMessage(LanguageManager.Tokens["fm_migration_complete"] + App.CurrentVersion + "!");
                }
                else
                {
                    await DialogWindow.ShowMessage(LanguageManager.Tokens["fm_migration_error"]);
                }

                File.WriteAllText(AppPaths.LogsPath + "\\migration_log.txt", logStr);
            }

            App.globalConfig.Set("file_migration_complete", migration_complete);
            OnOverhaulMigrationComplete(null, EventArgs.Empty);
            DialogWindow.DisableClose = false;
        }

        public static void MoveFiles()
        {
            CopyPhrases();
            CopyNotes();
            CopyBookmarks();
            DeleteOldDocuments();
        }

        public static void CopyFiles()
        {
            CopyPhrases();
            CopyNotes();
            CopyBookmarks();
        }

        public static void EnableCompatibilityMode()
        {
            App.globalConfig.Set("compatibility_mode", true);
            AppletManager.GetApplet<WebBrowser>().Config.Set("bookmarks_file", Path.Combine(PreV2VisuALSDocumentFolder, "VisuALS Components", "bookmarks.txt"));
            AppletManager.GetApplet<Notepad>().Config.Set("notes_folder", PreV2VisuALSDocumentFolder);
            AppletManager.GetApplet<TextToSpeech>().Config.Set("phrases_folder", Path.Combine(PreV2VisuALSDocumentFolder, "VisuALS Components"));
        }

        public static string CreateLog(string message, List<string> logs)
        {
            string logStr = "";
            if (logs.Count > 0)
            {
                logStr += message + "\n";
                foreach (string s in logs)
                {
                    logStr += "\t" + s;
                }
                logStr += "\n\n";
            }
            return logStr;
        }

        public static void DeleteOldSettings()
        {
            try
            {
                if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\VisuALS_Technology_Soluti"))
                {
                    Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\VisuALS_Technology_Soluti", true);
                }
            }
            catch (Exception e)
            {
                settingsLog.Add("Could not delete outdated settings folder.\n" + e.Message);
            }
        }

        public static void CopyPhrases()
        {
            try
            {
                if (Directory.Exists(PreV2VisuALSDocumentFolder + "\\VisuALS Components\\"))
                {
                    Directory.CreateDirectory(AppPaths.DocumentsPath + "\\phrases");
                    File.Copy(PreV2VisuALSDocumentFolder + "\\VisuALS Components\\ConversationStrings.txt", AppPaths.DocumentsPath + "\\phrases\\Conversation.txt");
                    File.Copy(PreV2VisuALSDocumentFolder + "\\VisuALS Components\\FeelingsStrings.txt", AppPaths.DocumentsPath + "\\phrases\\Feelings.txt");
                    File.Copy(PreV2VisuALSDocumentFolder + "\\VisuALS Components\\HomeAutomationStrings.txt", AppPaths.DocumentsPath + "\\phrases\\Home Automation.txt");
                    File.Copy(PreV2VisuALSDocumentFolder + "\\VisuALS Components\\NeedsStrings.txt", AppPaths.DocumentsPath + "\\phrases\\Needs.txt");
                }
            }
            catch (Exception e)
            {
                if (e.Message == "Cannot create a file when that file already exists.\r\n")
                {
                    phrasesLog.Add("Some or all phrase files from \"Documents\\VisuALS\\VisuALS Components\" are already in \"Documents\\VisuALS\\phrases\" and thus could not be copied over.\n");
                }
                else
                {
                    phrasesLog.Add(e.Message);
                }
            }
        }

        public static void CopyBookmarks()
        {
            try
            {
                if (Directory.Exists(PreV2VisuALSDocumentFolder + "\\VisuALS Components\\"))
                {
                    File.Copy(PreV2VisuALSDocumentFolder + "\\VisuALS Components\\bookmarks.txt", AppletManager.GetApplet<WebBrowser>().Config.Get<string>("bookmarks_file"));
                }
            }
            catch (Exception e)
            {
                if (e.Message.StartsWith("Could not find file")) { }
                else if (e.Message == "Cannot create a file when that file already exists.\r\n")
                {
                    bookmarksLog.Add("A file named \"bookmarks.txt\" already exists in \"" + AppletManager.GetApplet<WebBrowser>().Config.Get<string>("bookmarks_file") + "\" and so it could not be copied over.\n");
                }
                else
                {
                    bookmarksLog.Add(e.Message);
                }
            }
        }

        public static void DeleteOldDocuments()
        {
            if (notesLog.Count == 0 && phrasesLog.Count == 0 && bookmarksLog.Count == 0)
            {
                try
                {
                    Directory.Delete(PreV2VisuALSDocumentFolder + "\\VisuALS Components", true);
                }
                catch (Exception e)
                {
                    cleanupLog.Add("Could not delete VisuALS Components folder \n" + e.Message);
                }
                try
                {
                    File.Delete(PreV2VisuALSDocumentFolder + "\\FileNames.txt");
                }
                catch (Exception e)
                {
                    cleanupLog.Add("Could not delete FileNames.txt file \n" + e.Message);
                }

                foreach (string f in Directory.GetFiles(PreV2VisuALSDocumentFolder, "*.txt"))
                {
                    try
                    {
                        File.Delete(f);
                    }
                    catch (Exception e)
                    {
                        cleanupLog.Add("Could not delete \"" + Path.GetFileName(f) + "\"\n" + e.Message);
                    }
                }
            }
            else
            {
                cleanupLog.Add("Some phrases or notes could not be copied, so the associated folders and files were not be deleted.");
            }
        }

        public static void CopyNotes()
        {
            try
            {
                if (File.Exists(PreV2VisuALSDocumentFolder + "\\FileNames.txt"))
                {
                    Directory.CreateDirectory(AppPaths.DocumentsPath + "\\notes");
                    foreach (string f in Directory.GetFiles(PreV2VisuALSDocumentFolder, "*.txt"))
                    {
                        try
                        {
                            File.Copy(f, AppPaths.DocumentsPath + "\\notes\\" + f.Split('\\').Last());
                        }
                        catch (Exception e)
                        {
                            if (e.Message == "Cannot create a file when that file already exists.\r\n")
                            {
                                notesLog.Add("A note file with the same name as \"Documents\\VisuALS\\" + f.Split('\\').Last() + "\" is already located in the folder \"Documents\\VisuALS\\phrases\" and thus the file could not be copied over.\n");
                            }
                            else
                            {
                                notesLog.Add(e.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                notesLog.Add(e.Message);
            }
        }

        public static void OnOverhaulMigrationComplete(object sender, EventArgs e)
        {
            if (OverhaulMigrationComplete != null)
            {
                OverhaulMigrationComplete(sender, e);
            }
        }
    }
}

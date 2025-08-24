using System.IO;

namespace VisuALS_WPF_App
{
    public class Notepad : Applet
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Notepad() : base("ap_notepad", "ap_desc_notepad")
        {
            IconSource = "Resources/Images/notepad.png";
            Role = ButtonRole.Notes;
        }

        /// <summary>
        /// Returns the main page
        /// </summary>
        /// <returns></returns>
        public override AppletPage CreateMainPage()
        {
            return new NotepadPage();
        }

        public override AppletPage CreateSettingsPage()
        {
            return new NotepadSettings();
        }

        /// <summary>
        /// Initialize config and settings
        /// </summary>
        public override void InitializeSettingsValues()
        {
            Config.Initialize("notes_folder", Path.Combine(AppPaths.DocumentsPath, "notes"));
            Config.Initialize("autosave", false);
            Session.Initialize("title", "New Note");
            Session.Initialize("text", "");
        }
    }
}

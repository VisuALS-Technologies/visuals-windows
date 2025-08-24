using System.Windows;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for AudioSettings.xaml
    /// </summary>
    public partial class NotepadSettings : AppletPage
    {
        public NotepadSettings()
        {
            InitializeComponent();
            ParentApplet = AppletManager.GetApplet<Notepad>();
            AutoSave.Value = Config.Get<bool>("autosave");
            AutoSave.UpdateContent();
            NotesFolder.SelectedFile = Config.Get<string>("notes_folder");
            if (App.globalConfig.Get<bool>("compatibility_mode"))
            {
                NotesFolder.IsEnabled = false;
            }
        }

        private void AutoSave_OptionSelected(object sender, RoutedEventArgs e)
        {
            Config.Set("autosave", AutoSave.Value);
        }

        private void NotesFolder_FileSelected(object sender, RoutedEventArgs e)
        {
            Config.Set("notes_folder", NotesFolder.SelectedFile);
        }
    }
}

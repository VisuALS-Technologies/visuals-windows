using System.Collections.Generic;
using System.IO;

namespace VisuALS_WPF_App
{
    public class TextToSpeech : Applet
    {
        public TextToSpeech() : base("ap_text_to_speech", "ap_desc_text_to_speech")
        {
            IconSource = "Resources/Images/speaker.png";
            Role = ButtonRole.Speech;
        }

        public override AppletPage CreateMainPage()
        {
            return new TextToSpeechPage();
        }

        public override AppletPage CreateSettingsPage()
        {
            return new TextToSpeechSettings();
        }

        public override void InitializeSettingsValues()
        {
            Config.Initialize("phrases_folder", Path.Combine(AppPaths.DocumentsPath, "phrases"));
            Session.Initialize("text", "");
            Session.Initialize("recents", new List<string>());
        }
    }
}

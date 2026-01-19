using System.Windows;
using System.Windows.Controls;
using VisuALS_WPF_App.Utils;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for AudioSettings.xaml
    /// </summary>
    public partial class AudioSettings : Page
    {
        public AudioSettings()
        {
            InitializeComponent();
            TTSVoiceCombo.SetItems(SpeechUtils.LabelsAndVoiceNamesDictionary);
            TTSVoiceCombo.SelectedItem = App.globalConfig.Get<string>("tts_voice");
            MuteAlarmToggle.Value = App.globalConfig.Get<bool>("mute_alarm");
            ConfirmationsToggle.Value = App.globalConfig.Get<bool>("verbal_confirmations");
        }

        private void VolUp_Click(object sender, RoutedEventArgs e)
        {
            VolumeControl.VolUp();
        }

        private void VolDown_Click(object sender, RoutedEventArgs e)
        {
            VolumeControl.VolDown();
        }

        private void TTSVoice_ItemSelected(object sender, RoutedEventArgs e)
        {
            App.globalConfig.Set("tts_voice", TTSVoiceCombo.SelectedItem);
        }

        private void TestTTSVoice_Click(object sender, RoutedEventArgs e)
        {
            SpeechUtils.Speak($"{LanguageManager.Tokens["ast_test_1"]} {App.globalConfig.Get<string>("tts_voice")} {LanguageManager.Tokens["ast_test_2"]}");
        }

        private void Confirmations_OptionSelected(object sender, RoutedEventArgs e)
        {
            App.globalConfig.Set("verbal_confirmations", ConfirmationsToggle.Value);
        }

        private void MuteAlarm_OptionSelected(object sender, RoutedEventArgs e)
        {
            App.globalConfig.Set("mute_alarm", MuteAlarmToggle.Value);
        }

        private void TTSVoiceCombo_Click(object sender, RoutedEventArgs e)
        {
            DialogWindow.ShowBanner("Did you know? You can use ModelTalker to create a custom\nTTS voice using recordings of your own voice!", "Learn How", ModelTalkLearnHow_Click);
        }

        private void ModelTalkLearnHow_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

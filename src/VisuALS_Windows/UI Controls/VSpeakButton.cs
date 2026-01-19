using System;
using System.Management;
using System.Windows;
using System.Windows.Controls;

namespace VisuALS_WPF_App
{
    class VSpeakButton : VToggle
    {
        public object TextSource
        {
            get { return (object)base.GetValue(TextSourceProperty); }
            set { base.SetValue(TextSourceProperty, value); }
        }

        public event EventHandler<NAudio.Wave.StoppedEventArgs> PlaybackStopped
        {
            add { outputDevice.PlaybackStopped += value; }
            remove { outputDevice.PlaybackStopped -= value; }
        }

        public static readonly DependencyProperty TextSourceProperty = DependencyProperty.Register(
            "TextSource", typeof(object), typeof(VSpeakButton), new PropertyMetadata(default(object)));

        private AudioOutputDevice outputDevice;

        public VSpeakButton()
        {
            TrueOption = LanguageManager.Tokens["speak"];
            FalseOption = LanguageManager.Tokens["pb_stop"];
            Value = true;
            Role = ButtonRole.Speech;
            OptionSelected += SpeakButton_OptionSelected;
            outputDevice = DeviceManager.GetPreferredAudioOutputDevice(AudioOutputRole.Speech);
            outputDevice.PlaybackStopped += Instance_SpeakCompleted;
        }

        private string GetText()
        {
            if (TextSource.GetType().IsSubclassOf(typeof(TextBox)))
            {
                TextBox t = TextSource as TextBox;
                return t.Text;
            }
            else if (TextSource.GetType().IsSubclassOf(typeof(TextBlock)))
            {
                TextBlock t = TextSource as TextBlock;
                return t.Text;
            }
            else if (TextSource.GetType().IsSubclassOf(typeof(ContentControl)))
            {
                ContentControl e = TextSource as ContentControl;
                return e.Content.ToString();
            }
            else
            {
                return TextSource.ToString();
            }
        }

        private void Instance_SpeakCompleted(object sender, NAudio.Wave.StoppedEventArgs e)
        {
            Value = true;
        }

        private void SpeakButton_OptionSelected(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Value == false)
            {
                outputDevice.Speak(GetText());
            }
            else
            {
                outputDevice.Stop();
            }
        }
    }
}

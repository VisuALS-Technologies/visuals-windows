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

        public static readonly DependencyProperty TextSourceProperty = DependencyProperty.Register(
            "TextSource", typeof(object), typeof(VSpeakButton), new PropertyMetadata(default(object)));

        public VSpeakButton()
        {
            TrueOption = LanguageManager.Tokens["speak"];
            FalseOption = LanguageManager.Tokens["pb_stop"];
            Value = true;
            Role = ButtonRole.Speech;
            OptionSelected += SpeakButton_OptionSelected;
            SpeechFactory.Instance.SpeakCompleted += Instance_SpeakCompleted;
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

        private void Instance_SpeakCompleted(object sender, System.Speech.Synthesis.SpeakCompletedEventArgs e)
        {
            if (e.Cancelled == false)
            {
                Value = true;
            }
        }

        private void SpeakButton_OptionSelected(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Value == false)
            {
                SpeechFactory.Instance.Speak(GetText());
            }
            else
            {
                SpeechFactory.Instance.Stop();
            }
        }
    }
}

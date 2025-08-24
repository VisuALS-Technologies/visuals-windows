using System.Windows;
namespace VisuALS_WPF_App
{
    class VFileSelect : VButton
    {
        public object SelectedFile
        {
            get { return (object)base.GetValue(SelectedFileProperty); }
            set
            {
                Content = Prefix + value;
                base.SetValue(SelectedFileProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedFileProperty = DependencyProperty.Register(
            "SelectedFile", typeof(object), typeof(VFileSelect), new PropertyMetadata(default(object)));

        public static readonly DependencyProperty PromptProperty = DependencyProperty.Register(
            "Prompt", typeof(string), typeof(VFileSelect), new PropertyMetadata(default(string)));

        public string Prefix
        {
            get { return (string)base.GetValue(PrefixProperty); }
            set { base.SetValue(PrefixProperty, value); }
        }

        public static readonly DependencyProperty PrefixProperty = DependencyProperty.Register(
            "Prefix", typeof(string), typeof(VFileSelect), new PropertyMetadata(default(string)));

        public static readonly RoutedEvent FileSelectedEvent = EventManager.RegisterRoutedEvent(
            "FileSelected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(VFileSelect));

        public event RoutedEventHandler FileSelected
        {
            add { AddHandler(FileSelectedEvent, value); }
            remove { RemoveHandler(FileSelectedEvent, value); }
        }

        public bool SelectDirectory
        {
            get { return (bool)base.GetValue(SelectDirectoryProperty); }
            set { base.SetValue(SelectDirectoryProperty, value); }
        }

        public static readonly DependencyProperty SelectDirectoryProperty = DependencyProperty.Register(
            "SelectDirectory", typeof(bool), typeof(VFileSelect), new PropertyMetadata(false));

        public VFileSelect() : base()
        {
            Click += Button_Click;
            Prefix = "";
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            VFileBrowser fb = new VFileBrowser();
            fb.SelectDirectory = SelectDirectory;
            DialogResponse r = await DialogWindow.Show(fb);
            if (r.ResponseObject != null)
            {
                SelectedFile = r.ResponseString;
                Content = Prefix + r.ResponseString;
                RoutedEventArgs args = new RoutedEventArgs(VFileSelect.FileSelectedEvent);
                RaiseEvent(args);
            }
        }
    }
}

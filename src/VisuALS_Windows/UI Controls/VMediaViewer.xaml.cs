using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for VMediaViewer.xaml
    /// </summary>
    public partial class VMediaViewer : UserControl
    {
        public FileType fileType;

        public string Source
        {
            get { return path; }
            set { path = value; Update(); }
        }

        private string path;

        private MediaPlayer mediaPlayer = new MediaPlayer();

        public VMediaViewer()
        {
            InitializeComponent();
        }

        private void PlayPauseBtn_OptionSelected(object sender, RoutedEventArgs e)
        {
            if (PlayPauseBtn.Value)
            {
                Play();
            }
            else
            {
                Pause();
            }
        }

        private void ReplayBtn_Click(object sender, RoutedEventArgs e)
        {
            Stop();
            Play();
            PlayPauseBtn.Value = true;
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            Stop();
            PlayPauseBtn.Value = false;
        }

        public void Play()
        {
            if (fileType.name == "video")
            {
                mediaElem.Play();
            }
            else if (fileType.name == "audio")
            {
                mediaPlayer.Play();
            }
        }

        public void Pause()
        {
            if (fileType.name == "video")
            {
                mediaElem.Pause();
            }
            else if (fileType.name == "audio")
            {
                mediaPlayer.Pause();
            }
        }

        public void Stop()
        {
            if (fileType.name == "video")
            {
                mediaElem.Stop();
            }
            else if (fileType.name == "audio")
            {
                mediaPlayer.Stop();
            }
        }

        private void Update()
        {
            mediaElem.Source = null;
            imageElem.Source = null;
            mediaElem.Visibility = Visibility.Hidden;
            audioLabel.Visibility = Visibility.Hidden;
            imageElem.Visibility = Visibility.Hidden;
            textElem.Visibility = Visibility.Hidden;
            noPreviewElem.Visibility = Visibility.Hidden;
            PlaybackCol.Width = new GridLength(0);

            fileType = FileType.GetFileType(Source);
            FileName.Content = Path.GetFileName(Source);

            if (fileType.name == "video")
            {
                mediaElem.Source = new Uri(Source);
                mediaElem.Visibility = Visibility.Visible;
                PlaybackCol.Width = new GridLength(1, GridUnitType.Star);
            }
            else if (fileType.name == "audio")
            {
                mediaPlayer.Open(new Uri(Source));
                audioLabel.Visibility = Visibility.Visible;
                PlaybackCol.Width = new GridLength(1, GridUnitType.Star);
            }
            else if (fileType.name == "image")
            {
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.UriSource = new Uri(Source);
                img.EndInit();
                imageElem.Source = img;
                imageElem.Visibility = Visibility.Visible;
            }
            else if (fileType.name == "text")
            {
                ((VTextBlock)textElem.DisplayContent).Text = File.ReadAllText(Source);
                textElem.Visibility = Visibility.Visible;
            }
            else
            {
                noPreviewElem.Visibility = Visibility.Visible;
            }
        }
    }
}

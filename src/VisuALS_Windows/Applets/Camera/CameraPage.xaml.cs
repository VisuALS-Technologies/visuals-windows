using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Windows.Media.Capture;
using Windows.Media.Devices;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for Camera.xaml
    /// </summary>
    public partial class CameraPage : AppletPage
    {
        
        PeriodicBackgroundProcess captureWorker;
        CaptureElement captureElement;
        MediaCapture capture;

        /// <summary>
        /// Default constructor
        /// </summary>
        public CameraPage()
        {
            InitializeComponent();
            ParentApplet = AppletManager.GetApplet<Camera>();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            captureElement = new CaptureElement();
            capture = new MediaCapture();
            await capture.InitializeAsync();
            captureElement.Source = capture;
            cameraPreviewHost.Child = captureElement;
            await capture.StartPreviewAsync();
        }

        private async void CaptureBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            StorageFolder cameraRoll = await StorageFolder.GetFolderFromPathAsync(Path.Combine(AppPaths.PicturesPath, "Camera Roll"));
            StorageFile file = await cameraRoll.CreateFileAsync($"{DateTime.Now:yyyyMMddTHHmmss}.jpg");
            await capture.CapturePhotoToStorageFileAsync(Windows.Media.MediaProperties.ImageEncodingProperties.CreateJpeg(), file);
            captureElement.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            await Task.Delay(250);
            captureElement.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }
    }
}
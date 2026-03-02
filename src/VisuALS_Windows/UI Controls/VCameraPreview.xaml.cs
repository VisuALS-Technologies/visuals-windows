using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Media.Capture;
using Windows.Media.Devices;
using Windows.UI.Xaml.Controls;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for VMediaViewer.xaml
    /// </summary>
    public partial class VCameraPreview : System.Windows.Controls.UserControl
    {
        public MediaCaptureDevice Source
        {
            get
            {
                return captureDevice;
            }
            set
            {
                captureDevice = value;
                if (captureDevice != null)
                {
                    MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings
                    {
                        VideoDeviceId = captureDevice.DeviceID
                    };
                    captureElement = new CaptureElement();
                    captureElement.Source = captureDevice.mediaCapture;
                    cameraPreviewHost.Child = captureElement;
                    captureDevice.StartPreview();
                    cameraPreviewHost.Visibility = Visibility.Visible;
                    NoCameraLbl.Visibility = Visibility.Collapsed;
                }
            }
        }
        MediaCaptureDevice captureDevice;
        CaptureElement captureElement;

        public VCameraPreview()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Source == null)
            {
                Source = DeviceManager.GetPreferredMediaCaptureDevice();
            }

            if (Source == null)
            {
                cameraPreviewHost.Visibility = Visibility.Collapsed;
                NoCameraLbl.Visibility = Visibility.Visible;
            }
        }
    }
}

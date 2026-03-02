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

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for Camera.xaml
    /// </summary>
    public partial class CameraPage : AppletPage
    {
        PeriodicBackgroundProcess cameraSearchProcess;

        /// <summary>
        /// Default constructor
        /// </summary>
        public CameraPage()
        {
            InitializeComponent();
            ParentApplet = AppletManager.GetApplet<Camera>();
            cameraSearchProcess = new PeriodicBackgroundProcess(cameraSearchProcess_Run);
        }

        private async void CaptureBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CameraPreview.Source.CapturePhoto();
            CameraPreview.Visibility = Visibility.Collapsed;
            await Task.Delay(250);
            CameraPreview.Visibility = Visibility.Visible;
        }

        void cameraSearchProcess_Run()
        {
            MediaCaptureDevice mediaCaptureDevice = DeviceManager.GetPreferredMediaCaptureDevice();
            if (mediaCaptureDevice != null)
            {
                Dispatcher.Invoke(() =>
                {
                    CameraPreview.Source = mediaCaptureDevice;
                });
                cameraSearchProcess.StopProcess();
            }
        }

        private void SwapCameraBtn_Click(object sender, RoutedEventArgs e)
        {
            List<MediaCaptureDevice> devices = DeviceManager.ListMediaCaptureDevices();
            if (devices.Count > 0)
            {
                if (CameraPreview.Source == null)
                {
                    CameraPreview.Source = DeviceManager.GetPreferredMediaCaptureDevice();
                    return;
                }
                int currentIndex = devices.FindIndex(d => d.DeviceID == CameraPreview.Source.DeviceID);
                int nextIndex = (currentIndex + 1) % devices.Count;
                CameraPreview.Source = devices[nextIndex];
            }
        }
    }
}
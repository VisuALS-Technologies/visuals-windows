using NAudio.CoreAudioApi;
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

        private void AppletPage_Loaded(object sender, RoutedEventArgs e)
        {
            SetCamera(DeviceManager.GetPreferredMediaCaptureDevice());
        }

        private async void CaptureBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CameraPreview.Visibility = Visibility.Collapsed;
            DeviceManager.GetPreferredAudioOutputDevice(AudioOutputRole.Media).Play(".\\Resources\\624913__theplax__camera-shutter-open.wav");
            await Task.Delay(50);
            CameraPreview.Source.CapturePhoto(Config.Get<string>("photos_folder"));
            await Task.Delay(200);
            CameraPreview.Visibility = Visibility.Visible;
        }

        void cameraSearchProcess_Run()
        {
            MediaCaptureDevice mediaCaptureDevice = DeviceManager.GetPreferredMediaCaptureDevice();
            if (mediaCaptureDevice != null)
            {
                Dispatcher.Invoke(() =>
                {
                    SetCamera(mediaCaptureDevice);
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
                    SetCamera(DeviceManager.GetPreferredMediaCaptureDevice());
                    return;
                }
                int currentIndex = devices.FindIndex(d => d.DeviceID == CameraPreview.Source.DeviceID);
                int nextIndex = (currentIndex + 1) % devices.Count;
                SetCamera(devices[nextIndex]);
            }
        }

        private void FlashBtn_OptionSelected(object sender, RoutedEventArgs e)
        {
            if (((VToggle)sender).Value)
            {
                CameraPreviewBackground.Fill = System.Windows.Media.Brushes.White;
            }
            else
            {
                CameraPreviewBackground.Fill = System.Windows.Media.Brushes.Black;
            }
        }

        private void SetCamera(MediaCaptureDevice device)
        {
            CameraPreview.Source = device;
            SetFlash(device.GetFlash());
        }

        private void SetFlash(MediaCaptureDevice.FlashMode mode)
        {
            CameraPreview.Source.SetFlash(mode, true);
            switch (mode)
            {
                case MediaCaptureDevice.FlashMode.On:
                    FlashBtn.Content = "Flash On";
                    break;
                case MediaCaptureDevice.FlashMode.Off:
                    FlashBtn.Content = "Flash Off";
                    break;
                case MediaCaptureDevice.FlashMode.Auto:
                    FlashBtn.Content = "Flash Auto";
                    break;
            }
            if (!CameraPreview.Source.FlashSupported())
            {
                if (mode == MediaCaptureDevice.FlashMode.On)
                {
                    CameraPreviewBackground.Fill = System.Windows.Media.Brushes.White;
                }
                else
                {
                    CameraPreviewBackground.Fill = System.Windows.Media.Brushes.Black;
                }
            }
        }

        private void FlashBtn_Click(object sender, RoutedEventArgs e)
        {
            switch(CameraPreview.Source.GetFlash())
            {
                case MediaCaptureDevice.FlashMode.On:
                    SetFlash(MediaCaptureDevice.FlashMode.Off);
                    break;
                case MediaCaptureDevice.FlashMode.Off:
                    if (CameraPreview.Source.FlashSupported())
                    {
                        SetFlash(MediaCaptureDevice.FlashMode.Auto);
                    }
                    else
                    {
                        SetFlash(MediaCaptureDevice.FlashMode.On);
                    }
                    break;
                case MediaCaptureDevice.FlashMode.Auto:
                    SetFlash(MediaCaptureDevice.FlashMode.On);
                    break;
            }
        }
    }
}
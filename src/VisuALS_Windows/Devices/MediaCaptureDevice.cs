using ImapX;
using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media;
using VisuALS_WPF_App.Properties;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.Devices;
using Windows.Storage;

namespace VisuALS_WPF_App
{
    public class MediaCaptureDevice : ConfigurableDevice
    {
        public enum FlashMode
        {
            Auto,
            On,
            Off
        }
        public bool isRecording { get; private set; }
        public MediaCapture mediaCapture;
        public DeviceInformation deviceInfo;

        public MediaCaptureDevice(DeviceInformation _deviceInfo) : base(_deviceInfo.Id)
        {
            deviceInfo = _deviceInfo;
            IconEmoji = "📷";
            DeviceType = "dev_audio_input";
            IconSource = deviceInfo.GetGlyphThumbnailAsync().GetAwaiter().GetResult().ToString();
            Name = deviceInfo.Name;
            foreach(var k in deviceInfo.Properties.Keys)
            {
                if (deviceInfo.Properties[k] != null)
                    Info[k] = deviceInfo.Properties[k].ToString();
            }
            MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings
            {
                VideoDeviceId = DeviceID
            };
            mediaCapture = new MediaCapture();
            mediaCapture.InitializeAsync(settings).AsTask().Wait();

            if (FlashSupported())
                Config.Initialize("flash_mode", FlashMode.Auto.ToString());
            else
                Config.Initialize("flash_mode", FlashMode.Off.ToString());
            if (mediaCapture.VideoDeviceController.FlashControl.Supported)
            {
                FlashMode saved_mode = FlashMode.Auto;
                if (Enum.TryParse(Config.Get<string>("flash_mode"), out saved_mode))
                {
                    SetFlash(saved_mode);
                }
            }
        }

        public async void CapturePhoto(string path = null)
        {
            StorageFile file;

            if (path == null)
            {
                StorageFolder cameraRoll = await StorageFolder.GetFolderFromPathAsync(Path.Combine(AppPaths.PicturesPath, "Camera Roll"));
                file = await cameraRoll.CreateFileAsync($"{DateTime.Now:yyyyMMddTHHmmss}.jpg", CreationCollisionOption.GenerateUniqueName);
            }
            else if (Directory.Exists(path))
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(path);
                file = await folder.CreateFileAsync($"{DateTime.Now:yyyyMMddTHHmmss}.jpg", CreationCollisionOption.GenerateUniqueName);
            }
            else
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(path));
                file = await folder.CreateFileAsync(Path.GetFileName(path), CreationCollisionOption.FailIfExists);
            }
            
            await mediaCapture.CapturePhotoToStorageFileAsync(Windows.Media.MediaProperties.ImageEncodingProperties.CreateJpeg(), file);
        }

        public async void StartPreview()
        {
            await mediaCapture.StartPreviewAsync();
        }

        public void SetFlash(FlashMode mode, bool save_setting = false)
        {
            if (mediaCapture.VideoDeviceController.FlashControl.Supported)
            {
                if (mode == FlashMode.On)
                    mediaCapture.VideoDeviceController.FlashControl.Enabled = true;
                else if (mode == FlashMode.Off)
                    mediaCapture.VideoDeviceController.FlashControl.Enabled = false;
                else if (mode == FlashMode.Auto)
                    mediaCapture.VideoDeviceController.FlashControl.Auto = true;
            }

            if (save_setting)
            {
                Config.Set("flash_mode", mode.ToString());
            }
        }

        public FlashMode GetFlash()
        {
            FlashMode saved_mode = FlashMode.Auto;
            if (Enum.TryParse(Config.Get<string>("flash_mode"), out saved_mode))
            {
                SetFlash(saved_mode);
            }
            return saved_mode;
        }

        public bool FlashSupported()
        {
            return mediaCapture.VideoDeviceController.FlashControl.Supported;
        }
    }
}

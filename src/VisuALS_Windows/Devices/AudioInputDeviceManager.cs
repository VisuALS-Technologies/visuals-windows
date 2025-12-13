using CefSharp.DevTools.CSS;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisuALS_WPF_App
{
    public class AudioInputDeviceManager : ConfigurableDeviceManager
    {
        private MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
        public AudioInputDeviceManager() : base("dev_audio_input_manager")
        {
            Config.Initialize<string>("preferred_device_id", enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console).ID);
        }
        override public List<Device> ListDevices()
        {
            return enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).Select(dev => new AudioInputDevice(dev)).ToList<Device>();
        }
        override public Device GetDeviceByID(string id)
        {
            return new AudioInputDevice(enumerator.GetDevice(id));
        }
        public AudioInputDevice GetPreferredDevice()
        {
            return GetDeviceByID(Config.Get<string>("preferred_device_id")) as AudioInputDevice;
        }
        public void SetPreferredDevice(AudioInputDevice device)
        {
            Config.Set("preferred_device_id", device.DeviceID);
        }
    }
}

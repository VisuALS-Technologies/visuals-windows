using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisuALS_WPF_App
{
    public class AudioOutputDeviceManager : ConfigurableDeviceManager
    {
        public enum Role
        {
            Media,
            Notifications,
            Alarm,
            Speech
        }
        private MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
        public AudioOutputDeviceManager() : base("dev_audio_output_manager")
        {
            Config.Initialize<string>("preferred_media_device_id", enumerator.GetDefaultAudioEndpoint(DataFlow.Render, NAudio.CoreAudioApi.Role.Multimedia).ID);
            Config.Initialize<string>("preferred_notifications_device_id", enumerator.GetDefaultAudioEndpoint(DataFlow.Render, NAudio.CoreAudioApi.Role.Console).ID);
            Config.Initialize<string>("preferred_alarm_device_id", enumerator.GetDefaultAudioEndpoint(DataFlow.Render, NAudio.CoreAudioApi.Role.Communications).ID);
            Config.Initialize<string>("preferred_speech_device_id", enumerator.GetDefaultAudioEndpoint(DataFlow.Render, NAudio.CoreAudioApi.Role.Communications).ID);
        }
        override public List<Device> ListDevices()
        {
            return enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active).Select(dev => new AudioOutputDevice(dev)).ToList<Device>();
        }
        override public Device GetDeviceByID(string id)
        {
            return new AudioOutputDevice(enumerator.GetDevice(id));
        }
        public AudioOutputDevice GetPreferredDevice(Role role)
        {
            return GetDeviceByID(Config.Get<string>($"preferred_{role.ToString().ToLower()}_device_id")) as AudioOutputDevice;
        }
        public void SetPreferredDevice(AudioOutputDevice device, Role role)
        {
            Config.Set($"preferred_{role.ToString().ToLower()}_device_id", device.DeviceID);
        }
    }
}

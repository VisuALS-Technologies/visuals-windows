using System;
using NAudio.Wave;

namespace VisuALS_WPF_App
{
    public class AudioOutputDevice : Device
    {
        WaveOut waveOut;
        public AudioOutputDevice(Guid guid) : base(guid)
        {
            DeviceName = "Unknown Audio Output";
            IconEmoji = "🔈";
            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                var caps = WaveOut.GetCapabilities(i);
                if (caps.NameGuid == guid)
                {
                    waveOut = new WaveOut() { DeviceNumber = i };
                    DeviceName = caps.ProductName;
                    ManufacturerGUID = caps.ManufacturerGuid;
                    ManufacturerName = caps.ManufacturerName();
                    break;
                }
            }
        }
    }
}

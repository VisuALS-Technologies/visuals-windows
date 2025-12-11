using System;
using NAudio.Wave;

namespace VisuALS_WPF_App
{
    public class AudioInputDevice : Device
    {
        WaveIn waveIn;
        public AudioInputDevice(Guid guid) : base(guid)
        {
            DeviceName = "Unknown Audio Input";
            IconEmoji = "🎙";
            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                var caps = WaveIn.GetCapabilities(i);
                if (caps.NameGuid == guid)
                {
                    waveIn = new WaveIn() { DeviceNumber = i };
                    DeviceName = caps.ProductName;
                    ManufacturerGUID = caps.ManufacturerGuid;
                    ManufacturerName = caps.ManufacturerName();
                    break;
                }
            }
        }
    }
}

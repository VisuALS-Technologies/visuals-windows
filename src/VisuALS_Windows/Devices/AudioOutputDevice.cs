using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace VisuALS_WPF_App
{
    public class AudioOutputDevice : ConfigurableDevice
    {
        WasapiOut output;
        WaveFileReader reader;
        public bool isPlaying { get; private set; }
        public bool isPaused { get; private set; }
        public AudioOutputDevice(MMDevice device) : base(device.ID)
        {
            IconEmoji = "🔈";
            DeviceType = "dev_audio_output";
            IconSource = device.IconPath;
            Name = device.FriendlyName;
            for (int i = 0; i < device.Properties.Count; i++)
            {
                try
                {
                    string prop_name = MMDeviceUtils.NameFromPropertyKey(device.Properties[i].Key);
                    Info[prop_name] = device.Properties[i].Value.ToString();
                }
                catch
                {
                    // Ignore properties that can't be read
                }
            }
            output = new WasapiOut(device, AudioClientShareMode.Shared, false, 100);
        }

        public void Play(string filepath)
        {
            isPlaying = true;
            isPaused = false;
            reader = new WaveFileReader(filepath);
            WaveChannel32 waveChannel = new WaveChannel32(reader) { PadWithZeroes = false };
            output.Init(waveChannel);
            output.Play();
        }

        public void Stop()
        {
            isPlaying = false;
            isPaused = false;
            output.Stop();
            reader.Close();
        }

        public void Pause()
        {
            isPlaying = false;
            isPaused = true;
            output.Pause();
        }

        public void Resume()
        {
            isPlaying = true;
            isPaused = false;
            output.Play();
        }
    }
}
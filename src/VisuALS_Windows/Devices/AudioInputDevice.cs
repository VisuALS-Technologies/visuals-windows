using System;
using System.Windows.Media;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace VisuALS_WPF_App
{
    public class AudioInputDevice : ConfigurableDevice
    {
        private WasapiCapture capture;
        private WaveFileWriter writer;
        public const int DEFAULT_SAMPLE_RATE = 44100;
        public bool isRecording { get; private set; }

        public AudioInputDevice(MMDevice device) : base(device.ID)
        {
            IconEmoji = "🎙";
            DeviceType = "dev_audio_input";
            IconSource = device.IconPath;
            Name = device.FriendlyName;
            for (int i = 0; i < device.Properties.Count; i++)
            {
                string prop_name = MMDeviceUtils.NameFromPropertyKey(device.Properties[i].Key);
                Info[prop_name] = device.Properties[i].Value.ToString();
            }
            capture = new WasapiCapture(device);
            capture.DataAvailable += capture_DataAvailable;
        }

        public void StartRecording(string filepath)
        {
            isRecording = true;
            capture.StartRecording();
            writer = new WaveFileWriter(filepath, capture.WaveFormat);
        }

        public void StopRecording()
        {
            isRecording = false;
            capture.StopRecording();
            writer.Close();
        }

        private void capture_DataAvailable(object sender, WaveInEventArgs e)
        {
            writer.Write(e.Buffer, 0, e.BytesRecorded);
            writer.Flush();
        }
    }
}

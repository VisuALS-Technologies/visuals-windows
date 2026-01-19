using CefSharp.DevTools.CSS;
using CefSharp.DevTools.Runtime;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.IO;
using System.Speech.AudioFormat;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace VisuALS_WPF_App
{
    public class AudioOutputDevice : ConfigurableDevice
    {
        MMDevice device;
        WasapiOut output;
        WaveFileReader reader;
        public bool isPlaying { get; private set; }
        public bool isPaused { get; private set; }
        private SpeechAudioFormatInfo speechAudioFormatInfo;
        string speakTempFile = AppPaths.AudioPath + "\\~speech_output.wav";
        public event EventHandler<StoppedEventArgs> PlaybackStopped;
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
            this.device = device;
        }

        //public void Speak(string text)
        //{
        //    isPlaying = true;
        //    isPaused = false;
        //    using (var stream = new System.IO.MemoryStream())
        //    using (var synth = new System.Speech.Synthesis.SpeechSynthesizer())
        //    {
        //        synth.SelectVoice(App.globalConfig.Get<string>("tts_voice"));
        //        synth.SetOutputToWaveStream(stream);
        //        synth.Speak(text);
        //        stream.Flush();
        //        stream.Seek(0, SeekOrigin.Begin);
        //        reader = new WaveFileReader(stream);
        //        output.Init(reader);
        //        output.Play();
        //    }
        //}

        public void Speak(string text)
        {
            isPlaying = true;
            isPaused = false;
            
            using (var synth = new System.Speech.Synthesis.SpeechSynthesizer())
            {
                synth.SelectVoice(App.globalConfig.Get<string>("tts_voice"));
                synth.SetOutputToWaveFile(speakTempFile);
                synth.Speak(text);
            }
            Play(speakTempFile);
        }

        private void Instance_SpeakCompleted(object sender, NAudio.Wave.StoppedEventArgs e)
        {
            output.Dispose();
            reader.Dispose();
        }

        public void Play(string filepath)
        {
            isPlaying = true;
            isPaused = false;
            reader = new WaveFileReader(filepath);
            WaveChannel32 waveChannel = new WaveChannel32(reader) { PadWithZeroes = false };
            output = new WasapiOut(device, AudioClientShareMode.Shared, false, 100);
            output.PlaybackStopped += Instance_SpeakCompleted;
            output.PlaybackStopped += PlaybackStopped;
            output.Init(waveChannel);
            output.Play();
        }

        public void Stop()
        {
            isPlaying = false;
            isPaused = false;
            if (output != null)
            {
                output.Stop();
            }
        }

        public void Pause()
        {
            isPlaying = false;
            isPaused = true;
            if (output != null)
            {
                output.Pause();
            }
        }

        public void Resume()
        {
            isPlaying = true;
            isPaused = false;
            if (output != null)
            {
                output.Play();
            }
        }
    }
}
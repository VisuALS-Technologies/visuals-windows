using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;

namespace VisuALS_WPF_App
{
    public static class SpeechUtils
    {
        private static SpeechSynthesizer synthesizer = new SpeechSynthesizer();

        public static System.Collections.ObjectModel.ReadOnlyCollection<InstalledVoice> InstalledVoices
        {
            get
            {
                return synthesizer.GetInstalledVoices();
            }
        }

        public static List<string> InstalledVoiceNames
        {
            get
            {
                List<string> l = new List<string>();
                foreach (InstalledVoice v in synthesizer.GetInstalledVoices())
                {
                    l.Add(v.VoiceInfo.Name);
                }
                return l;
            }
        }

        public static Dictionary<string, string> LabelsAndVoiceNamesDictionary
        {
            get
            {
                return InstalledVoices.ToDictionary(x => FormatVoice(x), x => x.VoiceInfo.Name);
            }
        }

        private static string FormatVoice(InstalledVoice voice)
        {
            return voice.VoiceInfo.Name + " (" + voice.VoiceInfo.Culture + ")";
        }

        public static void Speak(string text)
        {
            AudioOutputDevice outputDevice = DeviceManager.GetPreferredAudioOutputDevice(AudioOutputRole.Speech);
            outputDevice.Speak(text);
        }
    }
}

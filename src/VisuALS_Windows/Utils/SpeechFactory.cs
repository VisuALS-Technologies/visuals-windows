using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Speech.Synthesis;
using VisuALS_WPF_App.Annotations;


/// <summary>
/// SpeechFactory is a class that is used to maintain program level consistency with speech synthesis
/// This class sets and maintains various speech properties such as Voice gender, age, as well as regional and cultural info.
/// SpeechFactory is also a Singleton class, meaning that there is only one instance of it at any given time.
/// 
/// One of the major items of note is that the constructor is private, it should remain this way in order to prevent null reference exceptions.
/// </summary>

namespace VisuALS_WPF_App
{
    public class SpeechFactory : INotifyPropertyChanged
    {
        //our Singleton instance of our SpeechFactory
        public static SpeechFactory instance;
        private VoiceGender gender;
        private SpeechSynthesizer synthesizer;

        //this checks to see if we already have an instance of our SpeechFactory, if we do not one is created
        public static SpeechFactory Instance => instance ?? (instance = new SpeechFactory());

        public event EventHandler<SpeakCompletedEventArgs> SpeakCompleted
        {
            add { synthesizer.SpeakCompleted += value; }
            remove { synthesizer.SpeakCompleted -= value; }
        }

        /// <summary>
        /// String property for the name of the Voice that is currently being used
        /// </summary>
        public string Voice
        {
            get { return synthesizer.Voice.Name; }
            set { synthesizer.SelectVoice(value); }
        }

        /// <summary>
        /// Property for the current gender of the voice being used
        /// </summary>
        public VoiceGender Gender
        {
            get { return synthesizer.Voice.Gender; }
            set { synthesizer.SelectVoiceByHints(value); }
        }

        /// <summary>
        /// Private constructor for our Singleton class
        /// </summary>
        private SpeechFactory()
        {
            synthesizer = new SpeechSynthesizer();

            this.gender = VoiceGender.Neutral;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="phrase"></param>
        /// <param name="cancelCurrent">(Optional): Cancels the phrase currently being spoken (if any), defaults to true if not set</param>
        public void Speak(String phrase, bool cancelCurrent = true)
        {
            this.Resume();
            //cancel all currently queued phrases
            if (cancelCurrent)
            {
                synthesizer.SpeakAsyncCancelAll();
            }
            synthesizer.SpeakAsync(phrase);
        }

        public SpeechSynthesizer getSpeechSynth()
        {
            return this.synthesizer;
        }

        public System.Collections.ObjectModel.ReadOnlyCollection<InstalledVoice> InstalledVoices
        {
            get
            {
                return synthesizer.GetInstalledVoices();
            }
        }

        public List<string> InstalledVoiceNames
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

        public Dictionary<string, string> LabelsAndVoiceNamesDictionary
        {
            get
            {
                return InstalledVoices.ToDictionary(x => FormatVoice(x), x => x.VoiceInfo.Name);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static string FormatVoice(InstalledVoice voice)
        {
            return voice.VoiceInfo.Name + " (" + voice.VoiceInfo.Culture + ")";
        }

        public void Pause()
        {
            synthesizer.Pause();
        }

        public void Stop()
        {
            synthesizer.SpeakAsyncCancelAll();
        }

        public void Resume()
        {
            synthesizer.Resume();
        }
    }
}

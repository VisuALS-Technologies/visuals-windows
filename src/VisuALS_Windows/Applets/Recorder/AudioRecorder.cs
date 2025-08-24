using NAudio.Wave;
using System;
using System.Collections.Generic;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Class for recording and playback of .WAV files
    /// </summary>
    public class AudioRecorder
    {
        public const int DEFAULT_SAMPLE_RATE = 44100;
        //public const int DEFAULT_DEVICE = 0;

        WaveFileReader reader;
        WaveOut output = new WaveOut();
        WaveIn waveIn = new WaveIn();
        WaveFileWriter writer;
        private bool isRecording = false;
        private bool isPlaying = false;
        private bool isPaused = false;
        private bool isMuted = false;

        public WaveIn WaveIn { get => waveIn; private set => waveIn = value; }
        public WaveOut Output { get => output; private set => output = value; }
        public bool IsRecording { get => isRecording; private set => isRecording = value; }
        public bool IsPlaying { get => isPlaying; private set => isPlaying = value; }
        public bool IsPaused { get => isPaused; private set => isPaused = value; }
        public bool IsMuted { get => isMuted; set => isMuted = value; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public AudioRecorder()
        {
            List<WaveInCapabilities> sources = new List<WaveInCapabilities>();
            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                if (WaveIn.GetCapabilities(i).SupportsWaveFormat(SupportedWaveFormat.WAVE_FORMAT_44S16))
                    sources.Add(WaveIn.GetCapabilities(i));
            }

            if (sources.Count > 0)
            {
                waveIn.WaveFormat = new WaveFormat(DEFAULT_SAMPLE_RATE, sources[0].Channels);
                waveIn.DataAvailable += waveIn_DataAvailable;
            }
            else
            {
                DialogWindow.ShowMessage(LanguageManager.Tokens["ar_no_microphone"]);
            }

            try
            {
                output.Volume = 1;
            }
            catch (NAudio.MmException)
            {
                DialogWindow.ShowMessage(LanguageManager.Tokens["ar_no_speaker"]);
            }
        }

        /// <summary>
        /// Event handler for the DataAvailable event of WaveIn recorder object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            writer.Write(e.Buffer, 0, e.BytesRecorded);
            writer.Flush();
        }

        /// <summary>
        /// Opens file for playback
        /// </summary>
        /// <param name="fileName"> Name of file to open </param>
        /// <param name="playbackStopped"> Event handler to handle playback stopped event</param>
        public void OpenFile(string fileName, EventHandler<StoppedEventArgs> playbackStopped)
        {
            reader = new WaveFileReader(fileName);
            output = new WaveOut();
            WaveChannel32 waveChannel = new WaveChannel32(reader) { PadWithZeroes = false };
            output.Init(waveChannel);
            output.PlaybackStopped += playbackStopped;
        }

        /// <summary>
        /// Begin playback of the currently opened file.
        /// </summary>
        public void Play()
        {
            output.Play();
            SetBoolProperties(false, true, false);
        }

        /// <summary>
        /// Pause playback of the currently opened file
        /// </summary>
        public void Pause()
        {
            output.Pause();
            SetBoolProperties(false, false, true);
        }

        /// <summary>
        /// Stop playback of the currently opened file
        /// </summary>
        public void StopPlayback()
        {
            output.Stop();
            output.Dispose();
            if (reader != null)
            {
                reader.Close();
            }
            SetBoolProperties(false, false, false);
        }

        /// <summary>
        /// Mute playback
        /// </summary>
        public void Mute()
        {
            output.Volume = 0;
            isMuted = true;
        }

        /// <summary>
        /// Unmute playback
        /// </summary>
        public void Unmute()
        {
            output.Volume = 1;
            isMuted = false;
        }

        /// <summary>
        /// Begin recording to given file
        /// </summary>
        /// <param name="fileName"> Name of file to record to </param>
        public void Record(string fileName)
        {
            WriteFile(fileName);
            waveIn.StartRecording();
            SetBoolProperties(true, false, false);
        }

        /// <summary>
        /// Stop recording
        /// </summary>
        public void StopRecording()
        {
            waveIn.StopRecording();
            waveIn.Dispose();
            writer.Dispose();
            SetBoolProperties(false, false, false);
        }

        /// <summary>
        /// Open file for writing wave data (???)
        /// </summary>
        /// <param name="fileName"></param>
        public void WriteFile(string fileName) => writer = new WaveFileWriter(fileName, waveIn.WaveFormat);

        /// <summary>
        /// Dispose of all WAV file handling objects
        /// </summary>
        public void Dispose()
        {
            if (output != null)
            {
                StopPlayback();
                output.Dispose();
                output = null;
            }
            if (waveIn != null)
            {
                StopRecording();
                waveIn.Dispose();
                waveIn = null;
            }
            if (writer != null)
            {
                writer.Dispose();
                writer = null;
            }
        }

        /// <summary>
        /// Set all the boolean properties
        /// </summary>
        /// <param name="isRecording"> Set value of IsRecording </param>
        /// <param name="isPlaying"> Set value of IsPlaying </param>
        /// <param name="isPaused"> Set value of IsPaused </param>
        private void SetBoolProperties(bool isRecording, bool isPlaying, bool isPaused)
        {
            IsRecording = isRecording;
            IsPlaying = isPlaying;
            IsPaused = isPaused;
        }
    }
}

using NAudio.Wave;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for Recorder.xaml
    /// </summary>
    public partial class RecorderPage : AppletPage
    {
        #region Local Variables
        private AudioInputDevice inputDevice;
        private AudioOutputDevice outputDevice;
        public DocumentFolder RecordingsFolder;
        PeriodicBackgroundProcess AutoSave;
        private string recordingFileName;
        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public RecorderPage()
        {
            InitializeComponent();

            ParentApplet = AppletManager.GetApplet<Recorder>();

            RecordingsFolder = new DocumentFolder(Config.Get<string>("recordings_folder"), ExtensionFilter);

            RecordingsList.SelectedItem = Session.Get<string>("filename");

            RecordingsList.Prompt = "Recordings";

            RecordingsList.Rows = 4;
            RecordingsList.Cols = 2;

            UpdateList();

            inputDevice = DeviceManager.GetPreferredAudioInputDevice();
            outputDevice = DeviceManager.GetPreferredAudioOutputDevice(AudioOutputRole.Media);

            AutoSave = new PeriodicBackgroundProcess(AutoSaveRunFunction, 1000, this);
        }

        private bool ExtensionFilter(string filename)
        {
            return filename.EndsWith(".wav", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Updates the list of recordings and the status of playback buttons
        /// </summary>
        private void UpdateList()
        {
            RecordingsList.SetItems(RecordingsFolder.GetFileNames());
            Play_Button.IsEnabled = true;
            Replay_Button.IsEnabled = true;
            Rename_Button.IsEnabled = true;
            Delete_Button.IsEnabled = true;

            if (RecordingsList.Items.Count == 0)
            {
                RecordingsList.SelectedItemString = "";
                SelectedFileLabel.Content = "";
                Play_Button.IsEnabled = false;
                Replay_Button.IsEnabled = false;
                Rename_Button.IsEnabled = false;
                Delete_Button.IsEnabled = false;
            }
            else if (!RecordingsFolder.FileExists((string)SelectedFileLabel.Content))
            {
                RecordingsList.SelectedItemString = RecordingsList.ItemStrings[0];
                SelectedFileLabel.Content = RecordingsList.SelectedItemString;
            }
            else
            {
                RecordingsList.SelectedItemString = (string)SelectedFileLabel.Content;
            }
        }

        /// <summary>
        /// Called periodically to save name of opened file
        /// </summary>
        private void AutoSaveRunFunction()
        {
            this.Dispatcher.Invoke(() =>
            {
                Session.Set("filename", RecordingsList.SelectedItemString);
            });
        }

        /// <summary>
        /// Event handler for ItemSelectedChanged event of RecordingsList
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecordingsList_ItemSelectionChanged(object sender, RoutedEventArgs e)
        {
            SelectedFileLabel.Content = RecordingsList.SelectedItemString;
        }

        #region Button Clicks

        /// <summary>
        /// Click handler for Record button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Record_Button_Click(object sender, RoutedEventArgs e)
        {
            // If not recording right now
            if (!inputDevice.isRecording)
            {
                Record_Button.Content = LanguageManager.Tokens["ar_stop_recording"];
                Play_Button.IsEnabled = false;
                Replay_Button.IsEnabled = false;
                recordingFileName = RecordingsFolder.GetNewFileName();
                RecordingsFolder.NewFile(recordingFileName, "wav");
                Recording_Indicator.Visibility = Visibility.Visible;
                inputDevice.StartRecording(RecordingsFolder.GetFilePath(recordingFileName));
            }
            else if (inputDevice.isRecording)
            {
                Record_Button.Content = LanguageManager.Tokens["ar_record"];
                inputDevice.StopRecording();
                Recording_Indicator.Visibility = Visibility.Hidden;

                DialogResponse r = await DialogWindow.ShowKeyboardInput("Enter name for recording");
                string invchars = FileUtils.getInvalidPathChars(r.ResponseString);
                if (invchars != "")
                {
                    await DialogWindow.ShowMessage($"{LanguageManager.Tokens["ar_illegal_file_name"]} {invchars}\n{LanguageManager.Tokens["ar_saving_file_as"]} {recordingFileName}");
                }
                else
                {
                    if (RecordingsFolder.FileExists(r.ResponseString))
                    {
                        await DialogWindow.ShowMessage($"{LanguageManager.Tokens["ar_file_already_exists"]}\n{LanguageManager.Tokens["ar_saving_file_as"]} {recordingFileName}");
                    }
                    else
                    {
                        RecordingsFolder.RenameFile(recordingFileName, r.ResponseString);
                    }
                }
                UpdateList();
            }
        }

        /// <summary>
        /// Click handler for Play button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Play_Button_Click(object sender, RoutedEventArgs e)
        {
            if (outputDevice.isPlaying)
            {
                outputDevice.Pause();
                Play_Button.Content = LanguageManager.Tokens["pb_play"];
            }
            else if (outputDevice.isPaused)
            {
                outputDevice.Play(RecordingsFolder.GetFilePath(recordingFileName));
                Play_Button.Content = LanguageManager.Tokens["pb_pause"];
            }
            else
            {
                PlaySelectedRecording();
                Play_Button.Content = LanguageManager.Tokens["pb_pause"];
            }
        }

        /// <summary>
        /// Click handler for Replay button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Replay_Button_Click(object sender, RoutedEventArgs e)
        {
            outputDevice.Stop();
            PlaySelectedRecording();
            Play_Button.Content = LanguageManager.Tokens["pb_pause"];
        }

        /// <summary>
        /// Click handler for Rename button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Rename_Button_Click(object sender, RoutedEventArgs e)
        {
            outputDevice.Stop();
            if (RecordingsFolder.FileExists(RecordingsList.SelectedItemString))
            {
                DialogResponse r = await DialogWindow.ShowKeyboardInput("Enter new name for file: " + RecordingsList.SelectedItemString);
                string invchars = FileUtils.getInvalidPathChars(r.ResponseString);
                if (invchars != "")
                {
                    await DialogWindow.ShowMessage($"{LanguageManager.Tokens["ar_illegal_file_name"]} {invchars}");
                }
                else
                {
                    if (RecordingsFolder.FileExists(r.ResponseString))
                    {
                        await DialogWindow.ShowMessage(LanguageManager.Tokens["ar_file_already_exists"]);
                    }
                    else
                    {
                        RecordingsFolder.RenameFile(RecordingsList.SelectedItemString, r.ResponseString);
                    }
                    UpdateList();
                }
            }
        }

        /// <summary>
        /// Click handler for Delete button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Recording_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!outputDevice.isPlaying)
            {
                RecordingsFolder.DeleteFile(RecordingsList.SelectedItemString);
                UpdateList();
            }
        }

        #endregion

        #region General Purpose Functions

        /// <summary>
        /// Begins playback of currently selected file
        /// </summary>
        private async void PlaySelectedRecording()
        {
            if (RecordingsFolder.FileExists(RecordingsList.SelectedItemString))
            {
                outputDevice.Play(RecordingsFolder.GetFilePath(RecordingsList.SelectedItemString));
                outputDevice.PlaybackStopped += AudioEndOfRecording;
            }
            else
            {
                await DialogWindow.ShowMessage(LanguageManager.Tokens["ar_no_recording_with_that_name"]);
            }
        }

        /// <summary>
        /// Event handler for playback stopped event of the AudioRecorder object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AudioEndOfRecording(object sender, StoppedEventArgs e)
        {
            Play_Button.Content = LanguageManager.Tokens["pb_play"];
        }
        #endregion
    }
}

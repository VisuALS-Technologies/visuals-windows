using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for VPredictionButtonsBar.xaml
    /// </summary>
    public partial class VPredictionButtonsBar : VKeyboardTab
    {
        private Predictor.PredictionType predictType;
        private string lastText = "";
        private PeriodicBackgroundProcess PredictorProcess;

        public VPredictionButtonsBar()
        {
            InitializeComponent();
            PredictorProcess = new PeriodicBackgroundProcess(PredictorProcessStartedFunction, PredictorProcessRunFunction, PredictorProcessStoppedFunction, 500, this);
        }

        public void StartPredicting()
        {
            PredictorProcess.StartProcess();
        }

        public void StopPredicting()
        {
            PredictorProcess.StopProcess();
        }

        private void Predictor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBox txt = (TextBox)focusElement;
                VButton b = sender as VButton;
                string s = b.Content.ToString();
                TextData data = App.predictor.ApplyPrediction(txt.Text, s, txt.CaretIndex, predictType);
                txt.Text = data.text;
                txt.CaretIndex = data.caretIndex;
                App.predictor.PartialStringTrainAsync(txt.Text, txt.CaretIndex);
            }
            catch { }
        }

        private void PredictorProcessStartedFunction()
        {
            App.predictor.OpenPersistentConnection();
        }

        private void PredictorProcessRunFunction()
        {
            this.Dispatcher.Invoke(() =>
            {
                if (focusElement != null)
                {
                    TextBox txt = (TextBox)focusElement;
                    if (txt.Text != lastText)
                    {
                        UpdatePredictionButtons(txt);
                    }
                    if (txt.Text.Length > lastText.Length && txt.Text != "" && txt.Text.Last() == ' ')
                    {
                        App.predictor.PartialStringTrainAsync(txt.Text, txt.CaretIndex);
                    }
                    lastText = txt.Text;
                }
            });
        }

        private void PredictorProcessStoppedFunction()
        {
            App.predictor.ClosePersistentConnection();
        }

        private async void UpdatePredictionButtons(TextBox txt)
        {
            List<string> newPredictions;
            string tpc = GetTextPrecedingCaret(txt);
            if (tpc != "")
            {
                if (txt.Text == "" || tpc.Last() == ' ')
                {
                    predictType = Predictor.PredictionType.NEXT_WORD;
                    newPredictions = await App.predictor.NextWordPredictAsync(txt.Text, txt.CaretIndex);
                }
                else
                {
                    predictType = Predictor.PredictionType.WORD_COMPLETE;
                    newPredictions = await App.predictor.FinishWordPredictAsync(txt.Text, txt.CaretIndex);
                }
            }
            else
            {
                newPredictions = new List<string>() { "", "", "" };
            }
            SetTextPredictions(newPredictions);
        }

        private void SetTextPredictions(List<string> predictions)
        {
            if (predictions.Count > 0)
            {
                predict1.Content = predictions[0];
            }
            if (predictions.Count > 1)
            {
                predict2.Content = predictions[1];
            }
            if (predictions.Count > 2)
            {
                predict3.Content = predictions[2];
            }
            if (predictions.Count == 0)
            {
                predict1.Content = "I";
                predict2.Content = "You";
                predict3.Content = "Where";
            }
        }

        private string GetTextPrecedingCaret(TextBox txt)
        {
            if (txt.CaretIndex == 0 || txt == null)
            {
                return "";
            }
            else
            {
                return txt.Text.Substring(0, txt.CaretIndex);
            }
        }
    }
}

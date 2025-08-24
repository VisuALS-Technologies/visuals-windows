using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for VKeyboard.xaml
    /// </summary>
    public partial class VKeyboard : UserControl
    {

        public VKeyboardLayout Layout
        {
            set
            {
                layout = value;
                UpdateTabs();
                SwitchTab("Main");
            }
        }

        VKeyboardLayout layout;

        public UIElement FocusElement
        {
            set
            {
                InputSimulator.SetFocus(this, value);
                focusElement = value;
                try
                {
                    TextBox t = focusElement as TextBox;
                    t.CaretIndex = t.Text.Length;
                }
                catch { }
                UpdateFocusElement();
            }
        }

        UIElement focusElement;

        public List<string> Tabs
        {
            get
            {
                return layout.Keys.ToList();
            }
        }

        public VKeyboard()
        {
            InitializeComponent();
            PredictionBar.ParentKeyboard = this;
        }

        private void UpdateTabs()
        {
            if (layout != null)
            {
                foreach (var item in layout)
                {
                    item.Value.ParentKeyboard = this;
                    item.Value.Update();
                    if (item.Key == "Bar")
                    {
                        barFrame.Content = item.Value;
                    }
                }
            }
        }

        private void UpdateFocusElement()
        {
            if (layout != null)
            {
                foreach (var item in layout)
                {
                    if (focusElement != null)
                        item.Value.FocusElement = focusElement;
                }
            }
            PredictionBar.FocusElement = focusElement;
        }

        public void SwitchTab(string tabName)
        {
            tabFrame.Content = layout[tabName];
        }

        public void EnableTextPrediction()
        {
            PredictionRow.Height = new GridLength(1, GridUnitType.Star);
            PredictionBar.StartPredicting();
        }

        public void DisableTextPrediction()
        {
            PredictionRow.Height = new GridLength(0, GridUnitType.Star);
            PredictionBar.StopPredicting();
        }
    }
}

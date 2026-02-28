using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VisuALS_WPF_App
{
    public static class EyeTrackerManager
    {
        public static EyeTrackerDevice SelectedEyeTracker
        {
            get { return _selectedEyeTracker; }
            set
            {
                if (_selectedEyeTracker != null)
                {
                    _selectedEyeTracker.EndGazePointStream();
                    _selectedEyeTracker.GazePointReceived -= GazePointReceivedHandler;
                }

                _selectedEyeTracker = value;

                if (_selectedEyeTracker != null)
                {
                    _selectedEyeTracker.StartGazePointStream();
                    _selectedEyeTracker.GazePointReceived += GazePointReceivedHandler;
                }
            }
        }
        private static EyeTrackerDevice _selectedEyeTracker = null;
        public static event EventHandler<Point> GazePointReceived;
        private static void GazePointReceivedHandler(object sender, Point e)
        {
            GazePointReceived?.Invoke(sender, e);
        }
        static EyeTrackerManager()
        {
            SelectedEyeTracker = DeviceManager.GetPreferredEyeTrackerDevice();
        }
    }
}

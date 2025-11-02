using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VisuALS_WPF_App
{
    public static class GazeTrackerManager
    {
        public static IGazeTracker SelectedGazeTracker
        {
            get { return _selectedGazeTracker; }
            set
            {
                if (_selectedGazeTracker != null)
                {
                    _selectedGazeTracker.EndGazePointStream();
                    _selectedGazeTracker.GazePointReceived -= GazePointReceivedHandler;
                }

                _selectedGazeTracker = value;

                if (_selectedGazeTracker != null)
                {
                    _selectedGazeTracker.StartGazePointStream();
                    _selectedGazeTracker.GazePointReceived += GazePointReceivedHandler;
                }
            }
        }
        private static IGazeTracker _selectedGazeTracker = null;
        public static event EventHandler<Point> GazePointReceived;
        public static List<IGazeTracker> GazeTrackers = new List<IGazeTracker>();
        private static void GazePointReceivedHandler(object sender, Point e)
        {
            GazePointReceived?.Invoke(sender, e);
        }
        static GazeTrackerManager()
        {
            GazeTrackers.Add(new MouseGazeSim());
            SelectedGazeTracker = GazeTrackers[0];
        }
        public static Dictionary<string, object> NamesAndGazeTrackersDictionary()
        {
            return GazeTrackers.ToDictionary(gt => gt.DeviceName, gt => gt as object);
        }
    }
}

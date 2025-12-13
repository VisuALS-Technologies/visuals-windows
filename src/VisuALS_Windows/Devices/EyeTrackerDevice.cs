using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace VisuALS_WPF_App.Devices
{
    public abstract class EyeTrackerDevice: ConfigurableDevice
    {
        public abstract Point GazePoint { get; }
        public EyeTrackerDevice(string id) : base(id)
        {
            Name = "Unknown Eye Tracker";
            DeviceType = "Eye Tracker";
            IconEmoji = "👁️";
        }
        public abstract bool SupportsExternalCalibration { get; }
        public abstract event EventHandler<Point> GazePointReceived;
        public abstract void StartGazePointStream();
        public abstract void EndGazePointStream();
        public abstract void StartExternalCalibration();
        public abstract void ResetExternalCalibration();
    }
}

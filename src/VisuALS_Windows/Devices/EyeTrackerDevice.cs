using System;
using System.Windows;

namespace VisuALS_WPF_App.Devices
{
    public abstract class EyeTrackerDevice: Device
    {
        public EyeTrackerDevice(Guid guid) : base(guid)
        {
            DeviceName = "Unknown Eye Tracker";
            IconEmoji = "👁️";
        }
        public abstract event EventHandler<Point> GazePointReceived;
        public abstract Point GazePoint { get; }
        public abstract bool SupportsExternalCalibration { get; }
        public abstract void StartGazePointStream();
        public abstract void EndGazePointStream();
        public abstract void StartExternalCalibration();
        public abstract void ResetExternalCalibration();
    }
}

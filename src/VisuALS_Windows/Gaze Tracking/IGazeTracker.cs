using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VisuALS_WPF_App
{
    public enum GazeSmoothingFunction
    {
        None = 0,
        Internal = 1,
        Mean = 2
    }

    public interface IGazeTracker
    {
        string DeviceName { get; }
        string DeviceModel { get; }
        string DeviceSerial { get; }
        string ManufacturerName { get; }
        string APIName { get; } // Name and version number
        int PollingSpeed { get; set; } // In Hz
        Point GazePoint { get; }
        event EventHandler<Point> GazePointReceived;
        int SmoothingWidowSize { get; set; }
        GazeSmoothingFunction Smoothing { get; set; }
        bool SupportsExternalCalibration { get; }
        void StartGazePointStream();
        void EndGazePointStream();
        void StartInternalCalibration();
        void StartExternalCalibration();
        void ResetInternalCalibration();
        void ResetExternalCalibration();
    }
}

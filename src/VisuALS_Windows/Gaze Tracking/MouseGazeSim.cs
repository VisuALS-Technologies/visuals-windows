using CefSharp.BrowserSubprocess;
using mshtml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace VisuALS_WPF_App
{
    internal class MouseGazeSim : IGazeTracker
    {
        public string DeviceName { get; } = "Mouse Gaze Simulator";
        public string DeviceModel { get; } = "N/A";
        public string DeviceSerial { get; } = "N/A";
        public string ManufacturerName { get; } = "VisuALS";
        public string APIName { get; } = "N/A"; // Name and version number
        public int PollingSpeed { get; set; } = 100; // In Hz
        public Point GazePoint { 
            get {
                System.Drawing.Point pos = System.Windows.Forms.Control.MousePosition;
                return new Point(pos.X, pos.Y);
            } 
        }
        public event EventHandler<Point> GazePointReceived;
        public int SmoothingWidowSize { get; set; }
        public GazeSmoothingFunction Smoothing { get; set; }
        public bool SupportsExternalCalibration { get; }
        private PeriodicBackgroundProcess mousePosProcess = null;
        private void getMousePosition()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate {
                GazePointReceived?.Invoke(this, GazePoint);
            });
        }
        public void StartGazePointStream()
        {
            PeriodicBackgroundProcess process = new PeriodicBackgroundProcess(getMousePosition, PollingSpeed, false);
            process.StartProcess();
        }
        public void EndGazePointStream()
        {
            if (mousePosProcess != null)
            {
                mousePosProcess.StopProcess();
                mousePosProcess = null;
            }
        }
        public void StartInternalCalibration() { }
        public void StartExternalCalibration() { }
        public void ResetInternalCalibration() { }
        public void ResetExternalCalibration() { }
    }
}

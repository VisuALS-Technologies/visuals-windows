using System;
using System.Windows;
using VisuALS_WPF_App.Devices;

namespace VisuALS_WPF_App
{
    public class MouseGazeSimulator : EyeTrackerDevice
    {
        public MouseGazeSimulator() : base("mouse_gaze_simulator")
        {
            Name = "Mouse Gaze Simulator";
            IconEmoji = "🖱️";
        }
        public override Point GazePoint
        {
            get
            {
                System.Drawing.Point pos = System.Windows.Forms.Control.MousePosition;
                return new Point(pos.X, pos.Y);
            }
        }
        public override bool SupportsExternalCalibration => false;
        private PeriodicBackgroundProcess mousePosProcess = null;
        public override event EventHandler<Point> GazePointReceived;
        private void getMousePosition()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate {
                GazePointReceived?.Invoke(this, GazePoint);
            });
        }
        public override void StartGazePointStream()
        {
            mousePosProcess = new PeriodicBackgroundProcess(getMousePosition, 100, false);
            mousePosProcess.StartProcess();
        }
        public override void EndGazePointStream()
        {
            if (mousePosProcess != null)
            {
                mousePosProcess.StopProcess();
                mousePosProcess = null;
            }
        }
        public override void StartExternalCalibration() { }
        public override void ResetExternalCalibration() { }
    }
}

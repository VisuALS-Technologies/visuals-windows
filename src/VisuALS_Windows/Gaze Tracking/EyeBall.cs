using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace VisuALS_WPF_App
{
    public static class EyeBall
    {
        // the ball on the screen
        public static Ellipse ball = new Ellipse();

        // encapsulated ball color data
        private static SolidColorBrush ballColor = Brushes.Blue;
        public static SolidColorBrush BallColor
        {
            get
            {
                return ballColor;
            }

            set
            {
                ball.Fill = ballColor = value;
            }
        }

        // encapsulated ball size data
        private static double ballSize = 10;
        public static double BallSize
        {
            get
            {
                return ballSize;
            }

            set
            {
                ballSize = ball.Width = ball.Height = value;
            }
        }

        // encapsulated outline color data
        private static SolidColorBrush lineColor = Brushes.DarkOrange;
        public static SolidColorBrush LineColor
        {
            get
            {
                return lineColor;
            }

            set
            {
                ball.Stroke = lineColor = value;
            }
        }

        // encapsulated outline size data
        private static double lineSize = 2;
        public static double LineSize
        {
            get
            {
                return lineSize;
            }

            set
            {
                lineSize = ball.StrokeThickness = value;
            }
        }
        private  static bool isEnabled = false;
        public static bool IsEnabled { 
            get
            {
                return isEnabled;
            }
            set
            {
                if (value)
                {
                    Enable();
                }
                else
                {
                    Disable();
                }
            }
        }

        public static void Enable()
        {
            ball.Visibility = Visibility.Visible;
            GazeTrackerManager.GazePointReceived += GazePointReceivedHandler;
            isEnabled = true;
        }

        public static void Disable()
        {
            ball.Visibility = Visibility.Hidden;
            GazeTrackerManager.GazePointReceived -= GazePointReceivedHandler;
            isEnabled = false;
        }

        static EyeBall ()
        {
            IsEnabled = false;
        }

        public static void GazePointReceivedHandler(object sender, Point point)
        {
            if (App.CurrentWindow.IsLoaded)
            {
                Point windowPoint = App.CurrentWindow.PointFromScreen(point);
                Canvas.SetTop(ball, windowPoint.Y - (ball.Height / 2));
                Canvas.SetLeft(ball, windowPoint.X - (ball.Width / 2));
            }
        }
    }
}
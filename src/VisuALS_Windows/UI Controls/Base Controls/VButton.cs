using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Role types for button
    /// The role property in the VisuALS framework allows for automatic styling
    /// </summary>
    public enum ButtonRole
    {
        NotSpecified,
        Selected,
        No, Stop, Exit, Delete,
        Audio, Speech, Language,
        Notes, Files, Data, Save,
        Submit, //Enter, Go, Search, etc.
        Start,
        Arrow,
        Navigation, //Previous, Next, etc.
        Yes,
        Open, New,
        Web, Account,
        Display,
        EyeTracking,
        Toggle,
        Help,
        Phantom,
        Settings, Edit, Cancel, Item, Pause, Home, Alarm, Key
    }

    /// <summary>
    /// The VisuALS button class
    /// Has properties that used by the stylesheet to change the button
    /// appearance, as well as the logic for eye track clicking
    /// </summary>
    public class VButton : Button
    {
        /// <summary>
        /// The buttons Role property, used for automatic styling
        /// </summary>
        public ButtonRole Role
        {
            get { return (ButtonRole)base.GetValue(RoleProperty); }
            set { base.SetValue(RoleProperty, value); }
        }

        /// <summary>
        /// Register the role property so it can be used in XAML
        /// </summary>
        public static readonly DependencyProperty RoleProperty = DependencyProperty.Register(
            "Role", typeof(ButtonRole), typeof(VButton), new PropertyMetadata(default(ButtonRole)));

        /// <summary>
        /// The corner radius property, used in styling
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)base.GetValue(CornerRadiusProperty); }
            set { base.SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Register the corner radius property so it can be used in XAML
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius", typeof(CornerRadius), typeof(VButton), new PropertyMetadata(default(CornerRadius)));

        /// <summary>
        /// Icon source property, used to display an icon on the button
        /// </summary>
        public ImageSource IconSource
        {
            get { return (ImageSource)base.GetValue(IconSourceProperty); }
            set { base.SetValue(IconSourceProperty, value); }
        }

        /// <summary>
        /// Register the icon source property so it can be used in XAML
        /// </summary>
        public static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register(
            "IconSource", typeof(ImageSource), typeof(VButton), new PropertyMetadata(default(ImageSource)));

        /// <summary>
        /// The icon size property, this is the width to display the icon as
        /// </summary>
        public double IconSize
        {
            get { return (double)base.GetValue(IconSizeProperty); }
            set { base.SetValue(IconSizeProperty, value); }
        }

        /// <summary>
        /// Register the icon size property so it can be used in XAML
        /// </summary>
        public static readonly DependencyProperty IconSizeProperty = DependencyProperty.Register(
            "IconSize", typeof(double), typeof(VButton), new PropertyMetadata(default(double)));

        /// <summary>
        /// Represents the percentage of dwell time completed used for the indicator.
        /// </summary>
        public double DwellIndicatorPercent
        {
            get { return (double)base.GetValue(DwellIndicatorPercentProperty); }
            set { base.SetValue(DwellIndicatorPercentProperty, value); }
        }

        ///// <summary>
        /// Register the dwell time percentage property so it can be used in XAML
        /// </summary>
        public static readonly DependencyProperty DwellIndicatorPercentProperty = DependencyProperty.Register(
            "DwellIndicatorPercent", typeof(double), typeof(VButton), new PropertyMetadata(default(double)));

        /// <summary>
        /// Whether or not the eye tracker gaze is currently over the button
        /// </summary>
        public bool IsGazeOver
        {
            get { return (bool)base.GetValue(IsGazeOverProperty); }
            set { base.SetValue(IsGazeOverProperty, value); }
        }

        ///// <summary>
        /// Register the is gaze over property so it can be used in XAML
        /// </summary>
        public static readonly DependencyProperty IsGazeOverProperty = DependencyProperty.Register(
            "IsGazeOver", typeof(bool), typeof(VButton), new PropertyMetadata(default(bool)));

        public static readonly RoutedEvent GazeEnterEvent = EventManager.RegisterRoutedEvent(
        "GazeEnter", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(VButton));

        public event RoutedEventHandler GazeEnter
        {
            add { AddHandler(GazeEnterEvent, value); }
            remove { RemoveHandler(GazeEnterEvent, value); }
        }

        public static readonly RoutedEvent GazeLeaveEvent = EventManager.RegisterRoutedEvent(
        "GazeLeave", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(VButton));

        public event RoutedEventHandler GazeLeave
        {
            add { AddHandler(GazeLeaveEvent, value); }
            remove { RemoveHandler(GazeLeaveEvent, value); }
        }

        public static bool DwellIndicator;

        private static DoubleAnimation DwellIndicatorAnimation
            = new DoubleAnimation(0, 1,
                new Duration(new TimeSpan(0, 0, 0, 0,
                (int)Application.Current.Resources["DwellTime"])),
                FillBehavior.Stop);

        private static DependencyObject CurrentHitTestObject;
        private static DependencyObject LastHitTestObject;

        private static DispatcherTimer DwellTimer = new DispatcherTimer();

        private static HitTestFilterBehavior VButtonHitTestFilter(DependencyObject o)
        {
            FrameworkElement e = o as FrameworkElement;
            if (e is VButton && (e as VButton).IsEnabled)
            {
                CurrentHitTestObject = e;
                return HitTestFilterBehavior.Stop;
            }
            else if (!(e is VButton))
            {
                if (CurrentHitTestObject == null)
                {
                    CurrentHitTestObject = e;
                }
            }
            return HitTestFilterBehavior.Continue;
        }

        private static HitTestResultBehavior VButtonHitTestCallback(HitTestResult result)
        {
            return HitTestResultBehavior.Continue;
        }

        private static EventHandler<Point> GazePointReceivedHandler = (s, e) =>
        {
            if (App.CurrentWindow.IsLoaded)
            {
                GazeHitTest(App.CurrentWindow.PointFromScreen(e));
            }
        };

        private static async void GazeHitTest(Point point)
        {
            CurrentHitTestObject = null;
            VisualTreeHelper.HitTest(App.CurrentWindow, VButtonHitTestFilter, VButtonHitTestCallback, new PointHitTestParameters(point));

            await Task.Delay(5);

            if (CurrentHitTestObject != LastHitTestObject)
            {
                VButton b1 = CurrentHitTestObject as VButton;
                VButton b2 = LastHitTestObject as VButton;

                if (b2 != null)
                {
                    RoutedEventArgs args = new RoutedEventArgs(VButton.GazeLeaveEvent);
                    b2.RaiseEvent(args);
                    b2.IsGazeOver = false;
                }

                if (b1 != null)
                {
                    RoutedEventArgs args = new RoutedEventArgs(VButton.GazeEnterEvent);
                    b1.RaiseEvent(args);
                    b1.IsGazeOver = true;
                }
            }

            LastHitTestObject = CurrentHitTestObject;
        }

        /// <summary>
        /// Default constructor for VButton
        /// </summary>
        public VButton() : base()
        {
            GazeTrackerManager.GazePointReceived += GazePointReceivedHandler;
            this.GazeEnter += VButton_GazeEnter;
            this.GazeLeave += VButton_GazeLeave;
            DwellTimer.Tick += (x, y) => DwellDelayComplete();
        }

        private void VButton_GazeLeave(object sender, RoutedEventArgs e)
        {
            VButton temp = sender as VButton;
            temp.BeginAnimation(VButton.DwellIndicatorPercentProperty, null);
            DwellTimer.Stop();
        }

        private void VButton_GazeEnter(object sender, RoutedEventArgs e)
        {
            VButton temp = sender as VButton;
            if (DwellIndicator && temp.IsEnabled)
            {
                temp.BeginAnimation(VButton.DwellIndicatorPercentProperty, DwellIndicatorAnimation);
            }
            DwellTimer.Start();
        }

        public static void UpdateDwellIndicatorAnimation()
        {
            int ms = (int)Application.Current.Resources["DwellTime"];
            Duration dur = new Duration(new TimeSpan(0, 0, 0, 0, ms));
            DwellIndicatorAnimation = new DoubleAnimation(0, 1, dur, FillBehavior.Stop);
            DwellTimer.Interval = TimeSpan.FromMilliseconds(ms);
        }

        private static void DwellDelayComplete()
        {
            VButton button = CurrentHitTestObject as VButton;
            if (button != null && button.IsEnabled && DwellTimer.IsEnabled)
            {
                button.RaiseEvent(new RoutedEventArgs(VButton.ClickEvent));
            }
            DwellTimer.Stop();
        }
    }
}

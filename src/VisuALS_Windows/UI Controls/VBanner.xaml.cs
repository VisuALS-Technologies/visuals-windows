using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for Banner.xaml
    /// </summary>
    public partial class VBanner : UserControl
    {
        public enum BannerAnchor
        {
            Top,
            Bottom
        }

        public bool ShowBannerButton
        {
            get
            {
                return BannerButton.Visibility == Visibility.Visible;
            }

            set
            {
                BannerButton.Visibility = value ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public string Text
        {
            get { return (string)base.GetValue(TextProperty); }
            set { base.SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(VBanner), new PropertyMetadata(default(string), TextChanged));

        public event RoutedEventHandler Click
        {
            add { BannerButton.Click += value; }
            remove { BannerButton.Click -= value; }
        }

        public event RoutedEventHandler DismissClick
        {
            add { DismissButton.Click += value; }
            remove { DismissButton.Click -= value; }
        }

        public double ExpandedHeight
        {
            get { return (double)base.GetValue(ExpandedHeightProperty); }
            set { base.SetValue(ExpandedHeightProperty, value); }
        }

        public static readonly DependencyProperty ExpandedHeightProperty = DependencyProperty.Register(
        "ExpandedHeight", typeof(double), typeof(VBanner), new PropertyMetadata(80.0, ExpandedHeightChanged));

        public BannerAnchor Anchor
        {
            get { return (BannerAnchor)base.GetValue(AnchorProperty); }
            set { base.SetValue(AnchorProperty, value); }
        }

        public static readonly DependencyProperty AnchorProperty = DependencyProperty.Register(
        "Anchor", typeof(BannerAnchor), typeof(VBanner), new PropertyMetadata(BannerAnchor.Top, AnchorChanged));

        private static DoubleAnimation ExpandAnimation
            = new DoubleAnimation(0,
                (double)ExpandedHeightProperty.DefaultMetadata.DefaultValue,
                new Duration(new TimeSpan(0, 0, 0, 0, 250)),
                FillBehavior.HoldEnd);

        public VBanner()
        {
            InitializeComponent();
            Height = ExpandedHeight;
            Loaded += VBanner_Loaded;
        }

        private void VBanner_Loaded(object sender, RoutedEventArgs e)
        {
            if (Anchor == BannerAnchor.Top)
            {
                VerticalAlignment = VerticalAlignment.Top;
            }
            else if (Anchor == BannerAnchor.Bottom)
            {
                VerticalAlignment = VerticalAlignment.Bottom;
            }
        }

        private static void TextChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            VBanner b = o as VBanner;
            b.BannerText.Text = (string)e.NewValue;
        }

        // TODO: Move this functionality to a style?
        private static void AnchorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            VBanner b = o as VBanner;
            if (b.Anchor == BannerAnchor.Top)
            {
                b.VerticalAlignment = VerticalAlignment.Top;
                b.BannerLabel.CornerRadius = new CornerRadius(0, 0, 15, 15);

            }
            else if (b.Anchor == BannerAnchor.Bottom)
            {
                b.VerticalAlignment = VerticalAlignment.Bottom;
                b.BannerLabel.CornerRadius = new CornerRadius(15, 15, 0, 0);
            }
        }

        private static void ExpandedHeightChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ExpandAnimation = new DoubleAnimation(0,
                (double)ExpandedHeightProperty.DefaultMetadata.DefaultValue,
                new Duration(new TimeSpan(0, 0, 0, 0, 800)),
                FillBehavior.HoldEnd);
        }

        public void Show()
        {
            this.BeginAnimation(HeightProperty, ExpandAnimation);
        }

        public void Hide()
        {
            this.BeginAnimation(HeightProperty, null);
            Height = 0;
        }

        private void DismissButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}

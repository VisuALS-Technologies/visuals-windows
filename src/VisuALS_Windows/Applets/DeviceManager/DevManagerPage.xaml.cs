using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Xml;
using VisuALS_WPF_App.Devices;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class DevManagerPage : AppletPage
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public DevManagerPage()
        {
            InitializeComponent();
            populateDeviceInfo();
        }

        private void populateDeviceInfo()
        {
            (outputScrollBox.DisplayContent as VTextBlock).Text = "";
            foreach (var device in DeviceHelper.ListDevices())
            {
                (outputScrollBox.DisplayContent as VTextBlock).Text += $"{device.Name} ({device.DeviceType})\n";
                foreach(var info in device.Info)
                {
                    (outputScrollBox.DisplayContent as VTextBlock).Text += $"  {info.Key}: {info.Value}\n";
                }
                (outputScrollBox.DisplayContent as VTextBlock).Text += "\n";
            }
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            populateDeviceInfo();
        }
    }
}
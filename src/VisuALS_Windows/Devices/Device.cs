using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Management;

namespace VisuALS_WPF_App
{
    public class Device
    {
        public string DeviceName = "Unknown";
        public Guid DeviceGUID;
        public string ManufacturerName = "Unknown";
        public Guid ManufacturerGUID;
        public string DeviceType = "Unknown";
        public string IconEmoji = "🖳";
        public ImageSource IconSource = null;
        public SettingsFile Config;
        public Device(Guid guid)
        {
            DeviceGUID = guid;
            Config = new SettingsFile("devices\\" + DeviceGUID.ToString() + "\\config");
        }
    }
}

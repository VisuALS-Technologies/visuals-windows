using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Management;

namespace VisuALS_WPF_App
{
    public class ConfigurableDevice : Device
    {
        public SettingsFile Config;
        public ConfigurableDevice(string id)
        {
            DeviceID = id;
            Config = new SettingsFile(AppPaths.SettingsPath + "\\devices\\" + DeviceID.Replace("?", "").Replace("\\", "") + "\\config");
        }
    }
}

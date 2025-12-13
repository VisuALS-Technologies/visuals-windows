using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using NAudio.CoreAudioApi;

namespace VisuALS_WPF_App
{
    public abstract class ConfigurableDeviceManager<T> : DeviceManager<T> where T: Device
    {
        public SettingsFile Config;
        public ConfigurableDeviceManager(string device_manager_name)
        {
            Config = new SettingsFile(AppPaths.SettingsPath + "\\device_managers\\" + device_manager_name + "\\config");
        }
    }
}

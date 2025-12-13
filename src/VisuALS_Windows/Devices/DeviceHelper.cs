using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisuALS_WPF_App.Devices
{
    public static class DeviceHelper
    {
        public static List<DeviceManager> deviceManagers = new List<DeviceManager>()
        {
            new SystemDeviceManager(),
            new AudioInputDeviceManager(),
            new AudioOutputDeviceManager()
        };
        public static List<Device> ListDevices()
        {
            List<Device> devices = new List<Device>();
            for (int i = 0; i < deviceManagers.Count; i++)
            {
                devices.AddRange(deviceManagers[i].ListDevices());
            }
            return devices;
        }
    }
}

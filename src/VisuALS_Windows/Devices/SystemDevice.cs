using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace VisuALS_WPF_App
{
    public struct SystemDeviceType
    {
        public string device_type;
        public string wmi_class;
        public string icon_emoji;
        public SystemDeviceType(string dev_type, string wmi_cls, string icon_emo)
        {
            device_type = dev_type;
            wmi_class = wmi_cls;
            icon_emoji = icon_emo;
        }
    }

    public class SystemDevice : Device
    {
        string WMIClass;
        public SystemDevice(ManagementBaseObject device, SystemDeviceType sysdevtype)
        {
            WMIClass = sysdevtype.wmi_class;
            DeviceType = sysdevtype.device_type;
            IconEmoji = sysdevtype.icon_emoji;
            DeviceID = device["DeviceID"].ToString();
            Name = device["FriendlyName"].ToString();
            foreach (var prop in device.Properties)
            {
                Info[prop.Name] = prop.Value.ToString();
            }
        }
    }
}

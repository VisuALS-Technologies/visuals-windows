using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace VisuALS_WPF_App
{

    public class SystemDevice : Device
    {
        string WMIClass;
        public SystemDevice(ManagementBaseObject device, SystemDeviceType sysdevtype)
        {
            WMIClass = sysdevtype.wmi_class;
            DeviceType = sysdevtype.device_type;
            IconEmoji = sysdevtype.icon_emoji;

            foreach (var prop in device.Properties)
            {
                if (prop.Value != null)
                    if (prop.Value.GetType().IsArray)
                        Info[prop.Name] = "[" + string.Join(", ", (prop.Value as Array).Cast<object>()) + "]";
                    else
                        Info[prop.Name] = prop.Value?.ToString() ?? "null";
            }

            if (Info.ContainsKey("FriendlyName"))
                Name = Info["FriendlyName"];
            else if (Info.ContainsKey("Name"))
                Name = Info["Name"];
            else
                Name = "Unknown Device";

            if (Info.ContainsKey("DeviceID"))
                DeviceID = Info["DeviceID"];
            else
                DeviceID = "null";
        }
    }
}

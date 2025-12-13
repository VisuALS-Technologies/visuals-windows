using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace VisuALS_WPF_App
{
    public class SystemDeviceManager : DeviceManager<SystemDevice>
    {
        public static List<SystemDeviceType> DeviceTypes = new List<SystemDeviceType>() {
            new SystemDeviceType("dev_keyboard", "Win32_Keyboard", "⌨"),
            new SystemDeviceType("dev_mouse", "Win32_PointingDevice", "🖱️"),
            new SystemDeviceType("dev_monitor", "Win32_DesktopMonitor", "🖥️"),
            new SystemDeviceType("dev_printer", "Win32_Printer", "🖨️"),
            new SystemDeviceType("dev_battery", "Win32_Battery", "🔋"),
            new SystemDeviceType("dev_fan", "Win32_Fan", "🌀"),
            new SystemDeviceType("dev_heat_pipe", "Win32_HeatPipe", "🌡️"),
            new SystemDeviceType("dev_temperature_probe", "Win32_TemperatureProbe", "🌡️"),
            new SystemDeviceType("dev_optical_drive", "Win32_CDROMDrive", "💿"),
            new SystemDeviceType("dev_disk_drive", "Win32_DiskDrive", "💽"),
            new SystemDeviceType("dev_floppy_drive", "Win32_FloppyDrive", "💾"),
            new SystemDeviceType("dev_memory", "Win32_MemoryDevice", "💾"),
            new SystemDeviceType("dev_network_adapter", "Win32_NetworkAdapter", "🌐"),
            new SystemDeviceType("dev_sound_device", "Win32_SoundDevice", "🔊"),
            new SystemDeviceType("dev_motherboard", "Win32_BaseBoard", "💻"),
            new SystemDeviceType("dev_processor", "Win32_Processor", "🧠"),
            new SystemDeviceType("dev_serial_port", "Win32_SerialPort", "🔌"),
            new SystemDeviceType("dev_usb_controller", "Win32_USBController", "🔌"),
            new SystemDeviceType("dev_battery", "Win32_PortableBattery", "🔋"),
            new SystemDeviceType("dev_current_probe", "Win32_CurrentProbe", "⚡"),
            new SystemDeviceType("dev_voltage_probe", "Win32_VoltageProbe", "⚡"),
            new SystemDeviceType("dev_power_supply", "Win32_PowerSupply", "⚡"),
            new SystemDeviceType("dev_refrigeration", "Win32_Refrigeration", "❄️"),
            new SystemDeviceType("dev_tape_drive", "Win32_TapeDrive", "📼"),
            new SystemDeviceType("dev_parallel_port", "Win32_ParallelPort", "🔌"),
            new SystemDeviceType("dev_video_controller", "Win32_VideoController", "🎥"),
            new SystemDeviceType("dev_telephone", "Win32_POTSModem", "📞"),
            new SystemDeviceType("dev_scsi_controller", "Win32_SCSIController", "💽"),
            new SystemDeviceType("dev_port", "Win32_PortConnector", "🔌"),
            new SystemDeviceType("dev_hub", "Win32_USBHub", "🔌"),
            new SystemDeviceType("dev_onboard_device", "Win32_OnBoardDevice", "💻"),
            new SystemDeviceType("dev_IDE_controller", "Win32_IDEController", "💽"),
            new SystemDeviceType("dev_floppy_controller", "Win32_FloppyController", "💾"),
            new SystemDeviceType("dev_bus", "Win32_Bus", "🚌"),
            new SystemDeviceType("dev_firewire_controller", "Win32_1394Controller", "🔌"),
            new SystemDeviceType("dev_infrared_device", "Win32_InfraredDevice", "📡"),
            new SystemDeviceType("dev_pcmia_controller", "Win32_PCMCIAController", "💾"),
            new SystemDeviceType("dev_enclosure", "Win32_SystemEnclosure", "🖥️"),
            new SystemDeviceType("dev_bios", "Win32_BIOS", "🖥️")
        };
        override public List<SystemDevice> ListDevices()
        {
            List<SystemDevice> devices = new List<SystemDevice>();
            foreach (var sysdevtype in DeviceTypes)
            {
                ManagementObjectCollection objs = new ManagementObjectSearcher("SELECT * FROM " + sysdevtype.wmi_class).Get();
                foreach(var obj in objs)
                {
                    devices.Add(new SystemDevice(obj, sysdevtype));
                }
            }
            return devices;
        }
        override public SystemDevice GetDeviceByID(string id)
        {
            return ListDevices().Find(d => d.DeviceID == id);
        }
    }
}

using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace VisuALS_WPF_App
{
    public enum AudioOutputRole
    {
        Media,
        Notifications,
        Alarm,
        Speech
    }
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
    public static class DeviceManager
    {
        private static MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
        private static List<SystemDeviceType> SystemDeviceTypes = new List<SystemDeviceType>() {
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
            //new SystemDeviceType("dev_floppy_drive", "Win32_FloppyDrive", "💾"),
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
            //new SystemDeviceType("dev_power_supply", "Win32_PowerSupply", "⚡"),
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
            //new SystemDeviceType("dev_floppy_controller", "Win32_FloppyController", "💾"),
            new SystemDeviceType("dev_bus", "Win32_Bus", "🚌"),
            new SystemDeviceType("dev_firewire_controller", "Win32_1394Controller", "🔌"),
            new SystemDeviceType("dev_infrared_device", "Win32_InfraredDevice", "📡"),
            new SystemDeviceType("dev_pcmia_controller", "Win32_PCMCIAController", "💾"),
            new SystemDeviceType("dev_enclosure", "Win32_SystemEnclosure", "🖥️"),
            new SystemDeviceType("dev_bios", "Win32_BIOS", "🖥️")
        };
        private static List<EyeTrackerDevice> eyeTrackingDevices = new List<EyeTrackerDevice>()
        {
            new MouseGazeSimulator()
        };
        static DeviceManager()
        {
            App.globalConfig.Initialize<string>("preferred_media_audio_output_device_id", enumerator.GetDefaultAudioEndpoint(DataFlow.Render, NAudio.CoreAudioApi.Role.Multimedia).ID);
            App.globalConfig.Initialize<string>("preferred_notifications_audio_output_device_id", enumerator.GetDefaultAudioEndpoint(DataFlow.Render, NAudio.CoreAudioApi.Role.Console).ID);
            App.globalConfig.Initialize<string>("preferred_alarm_audio_output_device_id", enumerator.GetDefaultAudioEndpoint(DataFlow.Render, NAudio.CoreAudioApi.Role.Communications).ID);
            App.globalConfig.Initialize<string>("preferred_speech_audio_output_device_id", enumerator.GetDefaultAudioEndpoint(DataFlow.Render, NAudio.CoreAudioApi.Role.Communications).ID);
            App.globalConfig.Initialize<string>("preferred_audio_input_device_id", enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console).ID);
            App.globalConfig.Initialize<string>("preferred_eye_tracker_device_id", eyeTrackingDevices[0].DeviceID);
        }
        public static List<Device> ListAllDevices()
        {
            List<Device> devices = new List<Device>();
            devices.AddRange(ListAudioInputDevices());
            devices.AddRange(ListAudioOutputDevices());
            devices.AddRange(ListSystemDevices());
            devices.AddRange(ListEyeTrackingDevices());
            return devices;
        }

        public static List<AudioInputDevice> ListAudioInputDevices()
        {
            return enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).Select(dev => new AudioInputDevice(dev)).ToList();
        }

        public static List<AudioOutputDevice> ListAudioOutputDevices()
        {
            return enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active).Select(dev => new AudioOutputDevice(dev)).ToList();
        }
        public static List<SystemDevice> ListSystemDevices()
        {
            List<SystemDevice> devices = new List<SystemDevice>();
            foreach (var sysdevtype in SystemDeviceTypes)
            {
                ManagementObjectCollection objs = new ManagementObjectSearcher("SELECT * FROM " + sysdevtype.wmi_class).Get();
                foreach (var obj in objs)
                {
                    devices.Add(new SystemDevice(obj, sysdevtype));
                }
            }
            return devices;
        }
        public static List<EyeTrackerDevice> ListEyeTrackingDevices()
        {
            return eyeTrackingDevices;
        }
        public static EyeTrackerDevice GetPreferredEyeTrackerDevice()
        {
            string device_id = App.globalConfig.Get<string>("preferred_eye_tracker_device_id");
            if (device_id == "none")
                return null;
            var device = eyeTrackingDevices.FirstOrDefault(dev => dev.DeviceID == device_id);
            if (device != null)
                return device;
            else
                return eyeTrackingDevices.FirstOrDefault();
        }
        public static AudioOutputDevice GetPreferredAudioOutputDevice(AudioOutputRole role)
        {
            string device_id = "";
            switch (role)
            {
                case AudioOutputRole.Media:
                    device_id = App.globalConfig.Get<string>("preferred_media_audio_output_device_id");
                    break;
                case AudioOutputRole.Notifications:
                    device_id = App.globalConfig.Get<string>("preferred_notifications_audio_output_device_id");
                    break;
                case AudioOutputRole.Alarm:
                    device_id = App.globalConfig.Get<string>("preferred_alarm_audio_output_device_id");
                    break;
                case AudioOutputRole.Speech:
                    device_id = App.globalConfig.Get<string>("preferred_speech_audio_output_device_id");
                    break;
            }
            var device = enumerator.GetDevice(device_id);
            if (device != null)
                return new AudioOutputDevice(device);
            else
                return ListAudioOutputDevices().FirstOrDefault();
        }
        public static AudioInputDevice GetPreferredAudioInputDevice()
        {
            string device_id = App.globalConfig.Get<string>("preferred_audio_input_device_id");
            var device = enumerator.GetDevice(device_id);
            if (device != null)
                return new AudioInputDevice(device);
            else
                return ListAudioInputDevices().FirstOrDefault();
        }
        
    }
}

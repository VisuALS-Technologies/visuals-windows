using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace VisuALS_WPF_App.Utils
{
    static class DisplayControl
    {
        public struct BrightnessData
        {
            public uint MinimumBrightness;
            public uint CurrentBrightness;
            public uint MaximumBrightness;

            public BrightnessData(uint min, uint cur, uint max)
            {
                this.MinimumBrightness = min;
                this.CurrentBrightness = cur;
                this.MaximumBrightness = max;
            }
        }

        public struct _PHYSICAL_MONITOR
        {
            IntPtr hPhysicalMonitor;
            ushort[] szPhysicalMonitorDescription;
        }

        public enum ColorTemp : uint
        {
            unknown = 0,
            Temp_4000K = 1,
            Temp_5000K = 2,
            Temp_6500K = 3,
            Temp_7500K = 4,
            Temp_8200K = 5,
            Temp_9300K = 6,
            Temp_10000K = 7,
            Temp_11500K = 8
        }

        public static readonly _PHYSICAL_MONITOR _monitor;

        static DisplayControl()
        {
            IntPtr hmonitor = MonitorFromWindow(new WindowInteropHelper(App.CurrentWindow).Handle, 0);
            _PHYSICAL_MONITOR monitor = new _PHYSICAL_MONITOR();
            //szPhysicalMonitorDescription = new ushort[128];
            //hPhysicalMonitor = new IntPtr();
        }

        [DllImport("user32.dll")]
        private static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize, IntPtr pPhysicalMonitorArray);

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        //[DllImport("user32.dll")]
        //private static extern bool GetPhysicalMonitorsFromHMONITOR( IntPtr hMonitor, 
        //    uint dwPhysicalMonitorArraySize, LPPHYSICAL_MONITOR pPhysicalMonitorArray);

        [DllImport("Dxva2.dll")]
        private static extern bool SetMonitorBrightness(IntPtr hMonitor, uint dwNewBrightness);

        [DllImport("Dxva2.dll")]
        private static extern bool GetMonitorBrightness(IntPtr hMonitor,
            UIntPtr pdwMinimumBrightness, UIntPtr pdwCurrentBrightness, UIntPtr pdwMaximumBrightness);

        [DllImport("Dxva2.dll")]
        private static extern bool SetMonitorColorTemperature(IntPtr hMonitor, uint ctCurrentColorTemperature);

        [DllImport("Dxva2.dll")]
        private static extern bool GetMonitorColorTemperature(IntPtr hMonitor, UIntPtr pctCurrentColorTemperature);

        public static void SetBrightness(uint brightness)
        {
            //SetMonitorBrightness(_monitor, brightness);
        }

        public static BrightnessData GetBrightnessData()
        {
            UIntPtr min = new UIntPtr();
            UIntPtr cur = new UIntPtr();
            UIntPtr max = new UIntPtr();
            //GetMonitorBrightness(_monitor, min, cur, max);
            return new BrightnessData(min.ToUInt32(), cur.ToUInt32(), max.ToUInt32());
        }

        public static uint GetBrightness()
        {
            return GetBrightnessData().CurrentBrightness;
        }

        public static uint GetMaxBrightness()
        {
            return GetBrightnessData().MaximumBrightness;
        }

        public static uint GetMinimumBrightness()
        {
            return GetBrightnessData().MinimumBrightness;
        }

        public static void IncreaseBrightness(uint increase = 1)
        {
            BrightnessData d = GetBrightnessData();
            if ((d.CurrentBrightness + increase) <= d.MaximumBrightness)
            {
                SetBrightness(d.CurrentBrightness + increase);
            }
            else
            {
                SetBrightness(d.MaximumBrightness);
            }
        }

        public static void DecreaseBrightness(uint decrease = 1)
        {
            BrightnessData d = GetBrightnessData();
            if ((d.CurrentBrightness - decrease) >= d.MinimumBrightness)
            {
                SetBrightness(d.CurrentBrightness - decrease);
            }
            else
            {
                SetBrightness(d.MinimumBrightness);
            }
        }

        public static ColorTemp GetColorTemperature()
        {
            UIntPtr p = new UIntPtr();
            //GetMonitorColorTemperature(_monitor, p);
            return (ColorTemp)p;
        }

        public static void SetColorTemperature(ColorTemp temp)
        {
            //SetMonitorColorTemperature(_monitor, (uint)temp);
        }

        public static void IncreaseColorTemperature(uint increase = 1)
        {
            ColorTemp c = GetColorTemperature();
            c += increase;

            if (Enum.IsDefined(typeof(ColorTemp), c))
            {
                SetColorTemperature(c);
            }
        }


        public static void DecreaseColorTemperature(uint decrease = 1)
        {
            ColorTemp c = GetColorTemperature();
            c -= decrease;

            if (Enum.IsDefined(typeof(ColorTemp), c))
            {
                SetColorTemperature(c);
            }
        }
    }
}

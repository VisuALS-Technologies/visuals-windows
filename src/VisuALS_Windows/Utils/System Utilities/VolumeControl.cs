using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace VisuALS_WPF_App.Utils
{
    static class VolumeControl
    {
        private const int AppcommandVolumeMute = 0x80000;
        private const int AppcommandVolumeUp = 0xA0000;
        private const int AppcommandVolumeDown = 0x90000;
        private const int WmAppcommand = 0x319;
        private static readonly Window _window;

        [DllImport("winmm.dll", SetLastError = true, CharSet = CharSet.Auto)]

        private static extern uint waveOutGetVolume(IntPtr hwo, uint dwVolume);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessageW(IntPtr hWnd, int Msg,
            IntPtr wParam, IntPtr lParam);

        static VolumeControl()
        {
            _window = App.CurrentWindow;
        }

        public static void Mute()
        {
            SendMessageW(new WindowInteropHelper(_window).Handle, WmAppcommand, new WindowInteropHelper(_window).Handle,
                (IntPtr)AppcommandVolumeMute);
        }

        public static void VolDown()
        {
            SendMessageW(new WindowInteropHelper(_window).Handle, WmAppcommand, new WindowInteropHelper(_window).Handle,
                (IntPtr)AppcommandVolumeDown);
        }

        public static void VolUp()
        {
            SendMessageW(new WindowInteropHelper(_window).Handle, WmAppcommand, new WindowInteropHelper(_window).Handle,
                (IntPtr)AppcommandVolumeUp);
        }
        public static void GetVol()
        {
        }
    }
}

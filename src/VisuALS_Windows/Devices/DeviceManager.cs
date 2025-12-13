using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace VisuALS_WPF_App
{
    public abstract class DeviceManager<T> where T : Device
    {
        public abstract List<T> ListDevices();
        public abstract T GetDeviceByID(string id);
    }
}

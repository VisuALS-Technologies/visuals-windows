using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Management;

namespace VisuALS_WPF_App
{
    public class Device
    {
        public string Name = "Unknown Device";
        public string DeviceType = "dev_generic";
        public string DeviceID = null;
        public string IconEmoji = "⍰";
        public string IconSource;
        public Dictionary<string, string> Info = new Dictionary<string, string>();
    }
}
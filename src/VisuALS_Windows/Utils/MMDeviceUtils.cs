using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;

namespace VisuALS_WPF_App
{
    public static class MMDeviceUtils
    {        
        public static string NameFromPropertyKey(PropertyKey key)
        {
            List<string> matches = typeof(PropertyKeys).GetFields().Where(f => comparePropertyKeys((PropertyKey)f.GetValue(null), key)).Select(f => f.Name).ToList();
            if (matches.Count > 0)
            {
                return matches[0];
            }
            else
            {
                return "{" + key.formatId.ToString() + "}[" + key.propertyId.ToString() + "]";
            }
        }

        static bool comparePropertyKeys(PropertyKey key1, PropertyKey key2)
        {
            return key1.formatId == key2.formatId && key1.propertyId == key2.propertyId;
        }
    }
}

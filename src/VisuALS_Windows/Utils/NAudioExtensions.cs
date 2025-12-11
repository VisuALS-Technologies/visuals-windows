using NAudio.Wave;
using System;
using System.Reflection;
using System.Xml.Linq;

namespace VisuALS_WPF_App
{
    public static class NAudioExtensions
    {
        public static string ManufacturerName(this WaveOutCapabilities caps)
        {
            FieldInfo mfgIdField = caps.GetType().GetField("manufacturerId", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            short mfgId = (short)mfgIdField?.GetValue(caps);
            return Enum.GetName(typeof(NAudio.Manufacturers), mfgId) ?? "Unknown";
        }

        public static string ManufacturerName(this WaveInCapabilities caps)
        {
            FieldInfo mfgIdField = caps.GetType().GetField("manufacturerId", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            short mfgId = (short)mfgIdField?.GetValue(caps);
            return Enum.GetName(typeof(NAudio.Manufacturers), mfgId) ?? "Unknown";
        }
    }
}
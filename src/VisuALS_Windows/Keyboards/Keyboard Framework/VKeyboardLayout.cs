using Newtonsoft.Json;
using System.Collections.Generic;

namespace VisuALS_WPF_App
{
    [JsonConverter(typeof(KLJsonConverter))]
    public class VKeyboardLayout : Dictionary<string, VKeyboardTab> { }
}
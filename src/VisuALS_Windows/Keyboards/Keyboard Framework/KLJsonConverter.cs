using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace VisuALS_WPF_App
{
    class KLJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            VKeyboardLayout l = value as VKeyboardLayout;
            Dictionary<string, string> l2 = new Dictionary<string, string>();
            foreach (var tab in l)
            {
                l2.Add(tab.Key, tab.Value.name);
            }
            JObject.FromObject(l2).WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<string, string> l = JObject.Parse(reader.Value.ToString()).ToObject<Dictionary<string, string>>();
            VKeyboardLayout l2 = new VKeyboardLayout();
            foreach (var tab in l)
            {
                l2.Add(tab.Key, KeyboardManager.TabFromName(tab.Value));
            }
            return l2;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(VKeyboardLayout);
        }
    }
}

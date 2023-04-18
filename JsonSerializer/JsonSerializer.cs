using Newtonsoft.Json;
using System;

namespace XFrame.Common.JsonSerializer
{
    public class JsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerSettings _settingsNotIndented = new JsonSerializerSettings();
        private readonly JsonSerializerSettings _settingsIndented = new JsonSerializerSettings();

        public JsonSerializer(IJsonOptions options = null)
        {
            options?.Apply(_settingsIndented);
            options?.Apply(_settingsNotIndented);

            _settingsIndented.Formatting = Formatting.Indented;
            _settingsNotIndented.Formatting = Formatting.None;
        }

        public string Serialize(object obj, bool indented = false)
        {
            var settings = indented ? _settingsIndented : _settingsNotIndented;
            return JsonConvert.SerializeObject(obj, settings);
        }

        public object Deserialize(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type, _settingsNotIndented);
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _settingsNotIndented);
        }
    }
}

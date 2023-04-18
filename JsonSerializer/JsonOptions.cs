using Newtonsoft.Json;

namespace XFrame.Common.JsonSerializer
{
    public class JsonOptions : IJsonOptions
    {
        public void Apply(JsonSerializerSettings settings)
        {
        }

        public static JsonOptions New => new JsonOptions();
    }
}

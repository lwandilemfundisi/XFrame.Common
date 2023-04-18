using Newtonsoft.Json;

namespace XFrame.Common.JsonSerializer
{
    public interface IJsonOptions
    {
        void Apply(JsonSerializerSettings settings);
    }
}

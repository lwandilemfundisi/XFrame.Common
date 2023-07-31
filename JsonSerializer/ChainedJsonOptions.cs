using Newtonsoft.Json;
using System;

namespace XFrame.Common.JsonSerializer
{
    public class ChainedJsonOptions : IJsonOptions
    {
        private readonly Action<JsonSerializerSettings> _action;
        private readonly IJsonOptions _parent;

        public ChainedJsonOptions(IJsonOptions parent, Action<JsonSerializerSettings> action)
        {
            _parent = parent;
            _action = action;
        }

        void IJsonOptions.Apply(JsonSerializerSettings settings)
        {
            _parent.Apply(settings);
            _action.Invoke(settings);
        }
    }
}

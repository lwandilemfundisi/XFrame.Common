using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace XFrame.Common.Extensions
{
    public static class ConcurrentDictionaryExtensions
    {
        public static T GetOrAddTo<T>(this ConcurrentDictionary<string, object> value, string key, Func<T> addValue)
        {
            if (!value.ContainsKey(key))
            {
                value.TryAdd(key, addValue());
            }

            var entry = value[key];

            if (entry.IsNull())
            {
                return default(T);
            }

            return (T)entry;
        }
    }
}

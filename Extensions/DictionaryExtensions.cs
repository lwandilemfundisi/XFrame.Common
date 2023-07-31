using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XFrame.Common.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> factory)
        {
            TValue value;
            if (dictionary.TryGetValue(key, out value))
            {
                return value;
            }

            value = factory(key);

            dictionary[key] = value;

            return value;
        }

        public static IDictionary<string, object> SafeAdd(this IDictionary<string, object> instance, string key, object value)
        {
            if (value.IsNotNull() && value.ToString().IsNotNullOrEmpty())
            {
                instance.Add(key, value);
            }
            return instance;
        }

        public static T GetOrAddTo<T>(this IDictionary<string, object> value, string key, Func<T> addValue)
        {
            if (!value.ContainsKey(key))
            {
                value.Add(key, addValue());
            }

            var entry = value[key];

            if (entry.IsNull())
            {
                return default(T);
            }

            return (T)entry;
        }

                                public static void NullSafeAdd(this IDictionary<string, object> instance, string key, string value)
        {
            if (value.IsNotNullOrEmpty())
            {
                instance.Add(key, value);
            }
            else
            {
                instance.Add(key, null);
            }
        }

        public static void NullSafeAdd(this IDictionary<string, object> instance, string key, object value)
        {
            if (value.IsNotNull())
            {
                instance.Add(key, value);
            }
            else
            {
                instance.Add(key, null);
            }
        }
        public static IDictionary<T, K> SafeAddKey<T, K>(this IDictionary<T, K> instance, T key, K value)
        {
            if (instance.IsNotNull() && !instance.ContainsKey(key))
            {
                instance.Add(key, value);
            }
            return instance;
        }
    }
}

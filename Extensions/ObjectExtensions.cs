using Newtonsoft.Json;
using System;
using System.Globalization;

namespace XFrame.Common.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsNotNull(this object value)
        {
            return value != null;
        }

        public static T Copy<T>(this object source)
        {
            return source.Serialise().Deserialise<T>();
        }

        public static string AsString(this object value)
        {
            return value.IsNull() ? string.Empty : value.ToString();
        }

        public static string AsString(this object value, string format)
        {
            return value.IsNull() ? string.Empty : string.Format(CultureInfo.InvariantCulture, format, value);
        }

        public static string AsString(this object value, string format, string defaultValue)
        {
            return value.IsNull() ? defaultValue : string.Format(CultureInfo.InvariantCulture, format, value);
        }

        public static string AsString(this object value, string format, string defaultValue, IFormatProvider provider)
        {
            return value.IsNull() ? defaultValue : string.Format(provider, format, value);
        }

        public static bool? AsBool(this object value)
        {
            var result = false;

            if (bool.TryParse(value.AsString(), out result))
            {
                return result;
            }

            return null;
        }

        public static bool SafeEquals(this object value, object instance)
        {
            if (value.IsNotNull())
            {
                return value.Equals(instance);
            }
            else
            {
                if (instance.IsNull())
                {
                    return true;
                }

                return false;
            }
        }

        public static bool? YesNoAsBool(this object value)
        {
            var stringValue = value.AsString();
            if (stringValue.IsNullOrEmpty())
            {
                return null;
            }

            if (stringValue.Equals("YES", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (stringValue.Equals("NO", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return null;
        }

        public static bool IsNull(this object value)
        {
            return value == null;
        }
        public static bool HasMemberValue(this object value, string path)
        {
            return value.GetMemberValue(path).IsNotNull();
        }

        public static T SafeValue<K, T>(this K value, Func<K, T> getValue) where K : class
        {
            if (value.IsNotNull())
            {
                return getValue(value);
            }
            return default(T);
        }

        public static object GetMemberValue(this object value, string path)
        {
            if (value.IsNotNull())
            {
                var valueType = value.GetType();
                var currentValue = value;

                foreach (var pathItem in path.Split('.'))
                {
                    var propertyInfo = valueType.GetProperty(pathItem, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
                    if (propertyInfo.IsNull())
                    {
                        throw new ArgumentException("No property found, property: {0}".FormatInvariantCulture(pathItem));
                    }
                    var propertyValue = propertyInfo.GetValue(currentValue, null);
                    if (propertyValue.IsNotNull())
                    {
                        currentValue = propertyValue;
                        valueType = propertyInfo.PropertyType;
                    }
                    else
                    {
                        return null;
                    }
                }
                return currentValue;
            }
            return null;
        }

        public static string Serialise(this object value)
        {
            if (value.IsNotNull())
            {
                return JsonConvert.SerializeObject(value);
            }

            return string.Empty;
        }

        public static decimal? AsDecimal(this object value)
        {
            if (value.IsNull())
            {
                return null;
            }

            var result = 0M;
            if (decimal.TryParse(value.AsString(), out result))
            {
                return result;
            }
            return null;
        }

        public static double? AsDouble(this object value)
        {
            if (value.IsNull())
            {
                return null;
            }

            var result = 0d;
            if (double.TryParse(value.AsString(), out result))
            {
                return result;
            }
            return null;
        }
        public static Guid? AsGuid(this object value)
        {
            if (value.IsNull())
            {
                return null;
            }

            var result = Guid.Empty;
            if (Guid.TryParse(value.AsString(), out result))
            {
                return result;
            }
            return null;
        }

        public static long? AsLong(this object value)
        {
            if (value.IsNull())
            {
                return null;
            }

            var result = 0L;
            if (long.TryParse(value.AsString(), out result))
            {
                return result;
            }
            return null;
        }

    }
}

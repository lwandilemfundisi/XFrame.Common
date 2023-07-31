using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XFrame.Common.Extensions
{
    public static class BooleanExtensions
    {
        public static string ToString(this bool value, string trueValue, string falseValue)
        {
            return value ? trueValue : falseValue;
        }

        public static string AsStringBool(this bool? value, string trueValue, string falseValue)
        {
            if (value.HasValue)
            {
                return value.Value.ToString(trueValue, falseValue);
            }
            return string.Empty;
        }
    }
}

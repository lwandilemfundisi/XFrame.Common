using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XFrame.Common.Extensions
{
    public static class DoubleExtensions
    {
        public static string FormatCurrencyValue(this double source)
        {
            return source.AsDecimal().FormatCurrencyValue();
        }
        public static string Trunc(this double? source, int decimalPlaces)
        {
            var temp = source.AsString();
            return temp.Left(temp.IndexOf('.') + decimalPlaces + 1);
        }

        public static double AsDoubleAmount(this double? source)
        {
            if (source.IsNull())
            {
                return 0;
            }

            return source.Value.AsDoubleAmount();
        }

        public static double AsDoubleAmount(this double source)
        {
            return Math.Round(Convert.ToDouble(source), 2);
        }
    }
}

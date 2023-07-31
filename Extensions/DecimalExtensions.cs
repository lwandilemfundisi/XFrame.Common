using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Globalization;

namespace XFrame.Common.Extensions
{
    [DebuggerStepThrough]
    public static class DecimalExtensions
    {
        public static string FormatCurrencyValue(this decimal? source, string currencySymbol = "R")
        {
            if (source.IsNull())
            {
                return "{0} 0.00".FormatInvariantCulture(currencySymbol);
            }

            return FormatCurrencyValue(source.Value, currencySymbol);
        }

        public static string FormatCurrencyValueNoSpace(this decimal? source, string currencySymbol = "R")
        {
            if (source.IsNull())
            {
                return "{0}0.00".FormatInvariantCulture(currencySymbol);
            }

            return FormatCurrencyValueNoSpace(source.Value, currencySymbol);
        }

        public static string FormatCurrencyValueNoDecimals(this decimal? source, string currencySymbol = "R")
        {
            if (source.IsNull())
            {
                return "{0} 0".FormatInvariantCulture(currencySymbol);
            }

            return FormatCurrencyValueNoDecimals(source.Value, currencySymbol);
        }

        public static string FormatCurrencyValue(this decimal source, string currencySymbol = "R")
        {
            return "{0} {1}".FormatInvariantCulture(currencySymbol,source.AsString("{0:#,##0.00}")); 
        }

        public static string FormatCurrencyValueNoSpace(this decimal source, string currencySymbol = "R")
        {
            return "{0}{1}".FormatInvariantCulture(currencySymbol, source.AsString("{0:#,##0.00}")); 
        }

        public static string FormatCurrencyValueNoDecimals(this decimal source, string currencySymbol = "R")
        {
            return "{0} {1}".FormatInvariantCulture(currencySymbol, source.AsString("{0:# ##0}"));
        }

        public static string FormatCurrencyOrNotApplicable(this decimal? amount, string currencySymbol = "R")
        {
            if (amount.IsNull())
            {
                return "N/A";
            }

            return amount.Value.FormatCurrencyOrNotApplicable(currencySymbol);
        }

        public static string FormatCurrencyOrNotApplicable(this decimal amount, string currencySymbol = "R")
        {
            if (amount.IsNull() || amount == 0)
            {
                return "N/A";
            }

            return amount.FormatCurrencyValue(currencySymbol);
        }

        public static string FormatPercentageOrNotApplicable(this decimal? amount)
        {
            if (amount.IsNull())
            {
                return "N/A";
            }

            return amount.Value.FormatPercentageOrNotApplicable();
        }

        public static string FormatPercentageOrNotApplicable(this decimal? amount, int decimalPlaces)
        {
            if (amount.IsNull())
            {
                return "N/A";
            }

            return amount.Value.FormatPercentageOrNotApplicable(decimalPlaces);
        }

        public static string FormatPercentageOrNotApplicable(this decimal amount)
        {
            amount = amount.Round();

            if (amount.IsNull() || amount == 0)
            {
                return "N/A";
            }

            return amount.FormatPercentageValue();
        }

        public static string FormatPercentageOrNotApplicable(this decimal amount, int decimalPlaces)
        {
            amount = Decimal.Round(amount, decimalPlaces);

            if (amount == 0)
            {
                return "N/A";
            }

            return amount.FormatPercentageValueOneDecimalPlace();
        }

        public static string FormatPercentageWithAsterisk(this decimal? amount)
        {
            if (amount.IsNull())
            {
                return "N/A";
            }

            return amount.Value.FormatPercentageWithAsterisk();
        }

        public static string FormatPercentageWithAsterisk(this decimal amount)
        {
            var percentageValue = amount.FormatPercentageValue();

            if (amount < 0.01m)
            {
                return "{0}*".FormatInvariantCulture(percentageValue);
            }

            return percentageValue;
        }

        public static string FormatPercentageValue(this decimal? source)
        {
            if (source.IsNull())
            {
                source = 0M;
            }

            return source.Value.FormatPercentageValue();
        }

        public static string FormatPercentageValueOneDecimalPlace(this decimal? source)
        {
            if (source.IsNull())
            {
                source = 0M;
            }

            return source.Value.FormatPercentageValueOneDecimalPlace();
        }

        public static string FormatPercentageValue(this decimal source)
        {
            return "{0}{1}".FormatInvariantCulture(source.AsString("{0:#0.00}"), "%");
        }

        public static string FormatPercentageValueOneDecimalPlace(this decimal source)
        {
            return "{0}{1}".FormatInvariantCulture(source.AsString("{0:#0.0}"), "%");
        }

        public static string FormatPercentageValue(this decimal? source, string isNullText)
        {
            if (source.IsNotNull())
            {
                return "{0}{1}".FormatInvariantCulture(source.GetValueOrDefault().AsString("{0:#0.00}"), "%");
            }
            return isNullText;
        }

        public static decimal? MultiplyByOneHundred(this decimal? source)
        {
            return source.IsNotNull() ? (source * 100) : null;
        }

        public static double AsDouble(this decimal? source)
        {
            return source.GetValueOrDefault(0).AsDouble();
        }

        public static double AsDouble(this decimal source)
        {
            return Convert.ToDouble(source, CultureInfo.InvariantCulture);
        }

        public static string Money(this decimal? source)
        {
            return source.AsString("{0:#,##0.00}", "&nbsp;", CultureInfo.InvariantCulture);
        }

        public static bool InRangeInclusive(this decimal? source, decimal? minimum, decimal? maximum)
        {
            if (source.IsNull())
            {
                return false;
            }

            return source.Value >= minimum.GetValueOrDefault(0) && source.Value <= maximum.GetValueOrDefault(0);
        }

        public static decimal Round(this decimal source)
        {
            return Decimal.Round(source, 2);
        }
        
        public static decimal? Round(this decimal? source)
        {
            if (source.IsNotNull())
            {
                return Decimal.Round(source.Value, 2);
            }
            return null;
        }

        public static decimal? RoundDown(this decimal? source, int decimalPlaces)
        {
            if (source.IsNull())
            {
                return 0;
            }

            var power = Math.Pow(10, decimalPlaces);
            return (Math.Floor((double)source.Value * power) / power).AsDecimal();
        }

        public static decimal? AsPercentage(this decimal? source)
        {
            if (source.IsNotNull())
            {
                return source.Value / 100;
            }
            return null;
        }

        public static string Trunc(this decimal? source, int decimalPlaces)
        {
            var temp = source.AsString();
            int i = temp.IndexOf('.');
            if (i > 0)
            {
                if (decimalPlaces == 0)
                {
                    decimalPlaces = -1;
                }
                return temp.Left(i + decimalPlaces + 1);
            }
            return temp;
        }

        public static string FormatDecimalWithSpace(this decimal? source)
        {
            if (source.IsNull())
            {
                return String.Empty;
            }

            return  source.AsString("{0:### ###}");
        }

        public static double AsDoublePercentage(this decimal? source)
        {
            if (source.IsNull())
            {
                return 0;
            }

            return source.Value.AsDoublePercentage();
        }

        public static double AsDoublePercentage(this decimal? source, double defaultValue)
        {
            var value = source.AsDoublePercentage();
            if (value.IsNull())
            {
                return defaultValue;
            }
            return value;
        }

        public static double AsDoublePercentage(this decimal source)
        {
            var percentage = (source / 100);
            return Math.Round(Convert.ToDouble(percentage), 4);
        }

        public static double AsDoubleAmount(this decimal? source)
        {
            if (source.IsNull())
            {
                return 0;
            }

            return source.Value.AsDoubleAmount();
        }

        public static double AsDoubleAmount(this decimal source)
        {
            return Math.Round(Convert.ToDouble(source), 2);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Diagnostics;

namespace XFrame.Common.Extensions
{
    #region NumberConverter

    public static class NumberConverter
    {
        private static string[] TenToNineteen = new string[10] {
            "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen"};

        private static string[] TwentyToNinetyNine = new string[10] {
            string.Empty, string.Empty, "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"};

        private static string[] Digits = new string[11] {
            string.Empty, "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", string.Empty};

        private static string[] ThousandAndOver = new string[10] {
            string.Empty, " Thousand ", " Million ", " Billion ", " Trillion ", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };

        #region Methods

        public static string ConvertToPlace(int number)
        {
            var value = number.ToString(CultureInfo.InvariantCulture);

            var tens = int.Parse(value.Right(2), CultureInfo.InvariantCulture);

            if (tens > 10 && tens < 20)
            {
                return "{0}th".FormatInvariantCulture(value);
            }
            else
            {
                switch (value.Right(1))
                {
                    case "1":
                        {
                            return "{0}st".FormatInvariantCulture(value);
                        }
                    case "2":
                        {
                            return "{0}nd".FormatInvariantCulture(value);
                        }
                    case "3":
                        {
                            return "{0}rd".FormatInvariantCulture(value);
                        }
                    default:
                        {
                            return "{0}th".FormatInvariantCulture(value);
                        }
                }
            }
        }

        public static string ConvertToWords(int number)
        {
            number = Math.Abs(number);
            var result = string.Empty;
            var value = number.ToString(CultureInfo.InvariantCulture);

            if (number == 0)
            {
                result = "Zero";
            }
            else if (number < 10)
            {
                result = ConvertDigit(value);
            }
            else if (number < 100)
            {
                result = ConvertTens(value);
            }
            else
            {
                var index = 0;

                while (value.IsNotNullOrEmpty())
                {
                    var word = string.Empty;

                    if (value.Length == 1)
                    {
                        word = ConvertDigit(value);
                    }
                    else if (value.Length == 2)
                    {
                        word = ConvertTens(value);
                    }
                    else
                    {
                        word = ConvertHundreds(value.Right(3));
                    }

                    if (word.IsNotNullOrEmpty())
                    {
                        result = "{0}{1}{2}".FormatInvariantCulture(word, ThousandAndOver[index], result);
                    }

                    if (value.Length > 3)
                    {
                        value = value.Left(value.Length - 3);
                    }
                    else
                    {
                        value = string.Empty;
                    }

                    index++;
                }
            }

            return result.Trim();
        }

        #endregion

        #region Private Methods

        private static string ConvertHundreds(string value)
        {
            var result = string.Empty;

            if (value.IsNotNullOrEmpty())
            {
                value = value.PadLeft(3, '0');

                if (!value.Left(1).Equals("0"))
                {
                    result = "{0} Hundred".FormatInvariantCulture(ConvertDigit(value.Left(1)));
                }

                if (!value.Substring(1, 1).Equals("0"))
                {
                    result += " And {0}".FormatInvariantCulture(ConvertTens(value.Substring(1)));
                }
                else if (!value.Right(1).Equals("0"))
                {
                    result += " And {0}".FormatInvariantCulture(ConvertDigit(value.Substring(2)));
                }
            }

            return result.Trim();
        }

        private static string ConvertDigit(string value)
        {
            return Digits[int.Parse(value[0].ToString(), CultureInfo.InvariantCulture)];
        }

        private static string ConvertTens(string value)
        {
            var result = string.Empty;

            if (value.Left(1).Equals("1"))
            {
                result = TenToNineteen[int.Parse(value.Right(1), CultureInfo.InvariantCulture)];
            }
            else
            {
                result = "{0} {1}".FormatInvariantCulture(TwentyToNinetyNine[int.Parse(value.Left(1), CultureInfo.InvariantCulture)], ConvertDigit(value.Right(1)).Replace("Zero", string.Empty));
            }

            return result;
        }

        #endregion
    }

    #endregion

    [DebuggerStepThrough]
    public static class IntExtensions
    {
        public static string ToWords(this int value)
        {
            return NumberConverter.ConvertToWords(value);
        }

        public static string ToWords(this int? value)
        {
            if (value.HasValue)
            {
                return value.Value.ToWords();
            }
            return string.Empty;
        }

        public static string ToPlace(this int? value)
        {
            if (value.HasValue)
            {
                return value.Value.ToPlace();
            }
            return string.Empty;
        }

        public static string ToPlace(this int value)
        {
            return NumberConverter.ConvertToPlace(value);
        }

        public static decimal CalculatePercentage(this int? source, decimal? totalValue, int rounding)
        {
            if (source.GetValueOrDefault(0) == 0
                || totalValue.GetValueOrDefault(0) == 0m)
            {
                return 0m;
            }

            if (rounding <= 0)
            {
                rounding = 2;
            }

            return Decimal.Round((decimal)(source / totalValue) * 100, rounding);
        }

        public static decimal CalculatePercentage(this int? source, decimal? totalValue)
        {
            return source.CalculatePercentage(totalValue, 2);
        }

        public static string ToMonth(this int? source)
        {
            if (source.IsNull())
            {
                return string.Empty;
            }

            if (source.Value < 1 || source.Value > 12)
            {
                return string.Empty;
            }

            return CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(source.Value);
        }
    }
}

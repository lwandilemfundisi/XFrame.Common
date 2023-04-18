using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace XFrame.Common.Extensions
{
    public static class StringExtensions
    {
        public static StringBuilder AppendLineFormat(this StringBuilder stringBuilder, string format, params object[] args)
        {
            stringBuilder.AppendLine(string.Format(format, args));
            return stringBuilder;
        }

        public static string FormatInvariantCulture(this string format, params object[] values)
        {
            return string.Format(CultureInfo.InvariantCulture, format, values);
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNotNullOrEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static string OrDefault(this string value, string defaultValue)
        {
            if (value.IsNullOrEmpty())
            {
                return defaultValue;
            }
            return value;
        }

        public static string OrDefaultLazy(this string value, Func<string> defaultValue)
        {
            if (value.IsNullOrEmpty())
            {
                return defaultValue();
            }
            return value;
        }

        public static string Left(this string value, int length)
        {
            if (value.IsNotNullOrEmpty())
            {
                if (value.Length <= length)
                {
                    return value;
                }
                return value.Substring(0, length);
            }
            return string.Empty;
        }

        public static string Right(this string value, int length)
        {
            if (value.IsNotNullOrEmpty())
            {
                if (value.Length <= length)
                {
                    return value;
                }
                return value.Substring(value.Length - length);
            }
            return string.Empty;
        }

        public static string SubstringAfter(this string source, string value)
        {
            if (source.IsNullOrEmpty())
            {
                return string.Empty;
            }

            if (value.IsNullOrEmpty())
            {
                return source;
            }

            CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
            int index = compareInfo.IndexOf(source, value, CompareOptions.OrdinalIgnoreCase);
            if (index < 0)
            {
                return string.Empty;
            }
            return source.Substring(index + value.Length);
        }

        public static string SubstringBefore(this string source, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
            int index = compareInfo.IndexOf(source, value, CompareOptions.OrdinalIgnoreCase);
            if (index < 0)
            {
                return string.Empty;
            }
            return source.Substring(0, index);
        }

        public static string ToMD5Checksum(this string value)
        {
            if (value.IsNotNullOrEmpty())
            {
                using (var myMD5 = MD5.Create())
                {
                    var data = myMD5.ComputeHash(Encoding.Default.GetBytes(value));
                    var stringBuilder = new StringBuilder();
                    for (var i = 0; i < data.Length; i++)
                    {
                        stringBuilder.Append(data[i].ToString("x2", CultureInfo.CurrentCulture));
                    }
                    return stringBuilder.ToString();
                }
            }
            return string.Empty;
        }

        public static string Encrypt(this string value, string key)
        {
            using (var tdes = TripleDES.Create())
            {
                tdes.Key = UTF8Encoding.UTF8.GetBytes(key);
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                var encryptor = tdes.CreateEncryptor();
                var valueBytes = UTF8Encoding.UTF8.GetBytes(value);
                var encryptedBlock = encryptor.TransformFinalBlock(valueBytes, 0, valueBytes.Length);
                return Convert.ToBase64String(encryptedBlock, 0, encryptedBlock.Length);
            };
        }

        public static string Decrypt(this string value, string key)
        {
            using (var tdes = TripleDES.Create())
            {
                tdes.Key = UTF8Encoding.UTF8.GetBytes(key);
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                var decryptor = tdes.CreateDecryptor();
                var valueBytes = Convert.FromBase64String(value);
                var decryptedBlock = decryptor.TransformFinalBlock(valueBytes, 0, valueBytes.Length);
                return UTF8Encoding.UTF8.GetString(decryptedBlock);
            }
        }

        public static T Deserialise<T>(this string value)
        {
            var result = value.Deserialise(typeof(T));

            if (result.IsNull())
            {
                return default(T);
            }

            return (T)result;
        }

        public static object Deserialise(this string value, Type type)
        {
            if (value.IsNotNullOrEmpty())
            {
                try
                {
                    return JsonConvert.DeserializeObject(value, type);
                }
                catch
                {
                    return null;
                }
            }

            return null;

        }

        public static T SafeCastDeserialise<T>(this string value)
        {
            if (value.IsNotNullOrEmpty())
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(value);
                }
                catch
                {
                    return default(T);
                }
            }

            return default(T);
        }

        public static string RemoveCamelCasing(this string value)
        {
            if (value.IsNullOrEmpty())
            {
                return string.Empty;
            }

            var words = Regex.Split(value, @"(?<!^)(?=[A-Z])");
            var result = new StringBuilder();

            words.ForEach(w => result.Append("{0} ".FormatInvariantCulture(w.Trim())));

            return result.ToString().Trim();
        }

        public static string ValidFileName(this string value)
        {
            return Regex.Replace(value, @"[^\w\.-]", " ");
        }

        public static int? AsInt(this string value)
        {
            if (value.IsNullOrEmpty())
            {
                return null;
            }

            var result = 0;
            if (int.TryParse(value, out result))
            {
                return result;
            }

            return null;
        }

        public static long? AsLong(this string value)
        {
            if (value.IsNullOrEmpty())
            {
                return null;
            }

            long result = 0;
            if (long.TryParse(value, out result))
            {
                return result;
            }

            return null;
        }

        public static DateTime? AsDate(this string value, params string[] formats)
        {
            DateTime result;
            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(value, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out result))
                {
                    return result;
                }
            }

            return default(DateTime);
        }

        public static DayOfWeek? AsDayOfWeek(this string value)
        {
            DayOfWeek result;

            if (value.IsNotNull())
            {
                if (Enum.IsDefined(typeof(DayOfWeek), value))
                {
                    result = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), value, true);
                    return result;
                }
            }

            return default(DayOfWeek);
        }


        public static float? AsFloat(this string value)
        {
            if (value.IsNullOrEmpty())
            {
                return null;
            }

            var result = 0F;
            if (float.TryParse(value, out result))
            {
                return result;
            }

            return null;
        }

        public static string TrimEnd(this string value, string trimString)
        {
            if (value.IsNullOrEmpty())
            {
                return string.Empty;
            }

            if (value.Trim().EndsWith(trimString, StringComparison.OrdinalIgnoreCase))
            {
                if (value.Length >= trimString.Length)
                {
                    return value.Left(value.Length - trimString.Length);
                }
            }

            return value;
        }

        public static string SafeAppend(this string value, string appendedValue)
        {
            if (value.IsNotNullOrEmpty())
            {
                return value + appendedValue;
            }

            return value;
        }

        public static bool IsIn(this string value, params string[] values)
        {
            if (value.IsNullOrEmpty())
            {
                return false;
            }

            foreach (var item in values)
            {
                if (value.Equals(item, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public static string ToUpperFirstLetter(this string value)
        {
            if (value.IsNullOrEmpty())
            {
                return string.Empty;
            }

            var letters = value.ToCharArray();
            letters[0] = char.ToUpperInvariant(letters[0]);
            return new string(letters);
        }

        public static string ToTitleCase(this string value)
        {
            if (value.IsNullOrEmpty())
            {
                return string.Empty;
            }
            return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(value.ToLower(CultureInfo.InvariantCulture));
        }

        public static string RemoveLineBreaksAndEscapeCharacters(this string value)
        {
            if (value.IsNullOrEmpty())
            {
                return string.Empty;
            }

            var patternString = @"\\r\\n|\\n|\\r|\\\n";
            var result = Regex.Replace(value, patternString, "", RegexOptions.IgnoreCase);

            var quoteString = @"\\\""";
            result = Regex.Replace(result, quoteString, @"\\""", RegexOptions.IgnoreCase);

            quoteString = @"[ ]{2,}";
            result = Regex.Replace(result, quoteString, @"\\""", RegexOptions.IgnoreCase);

            quoteString = @"\\{2,}""";
            return Regex.Replace(result, quoteString, @"\\\""", RegexOptions.IgnoreCase);
        }

        public static string FormatCellNumberWithSpaces(this string value)
        {
            string splitNumber = string.Empty;

            for (int i = 0; i < value.Length; i++)
            {
                if (i == 3 || i == 6)
                {
                    splitNumber += " {0}".FormatInvariantCulture(value[i]);
                }
                else
                {
                    splitNumber += value[i];
                }
            }

            return splitNumber;
        }

        public static bool StringCompareIgnoreCaseAndSpace(this string source, string value)
        {
            if (source.IsNotNull() && value.IsNotNull())
            {
                return source.ToLower().Replace(" ", "") == value.ToLower().Replace(" ", "");
            }
            return false;
        }

        public static List<string> DelimitedSplit(this string src, char delimeter, char quote = '\"')
        {
            List<string> ret = new List<string>();
            int idx = 0;
            int start = 0;
            bool inQuote = false;

            while (idx < src.Length)
            {
                if ((!inQuote) && (src[idx] == delimeter))
                {
                    ret.Add(src.Substring(start, idx - start).Trim());
                    start = idx + 1;
                }

                if (src[idx] == quote)
                {
                    inQuote = !inQuote;
                }

                ++idx;
            }

            if (!inQuote)
            {
                ret.Add(src.Substring(start, idx - start).Trim());
            }

            return ret;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using XFrame.Common.Comparers;

namespace XFrame.Common.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> value, Func<T, T, bool> comparer, Func<T, int> hashCode)
        {
            return value.Distinct(new GenericEqualityComparer<T>(comparer, hashCode));
        }

        public static IEnumerable<T> SafeConcat<T>(this IEnumerable<T> value, IEnumerable<T> second)
        {
            if (value.IsNotNull() && second.IsNotNull())
            {
                return value.Concat(second);
            }
            else if (value.IsNotNull())
            {
                return value;
            }
            else if (second.IsNotNull())
            {
                return second;
            }

            return new T[] { };
        }

        public static byte[] ToCSVData(this IEnumerable<Dictionary<string, object>> value)
        {
            var csvData = string.Empty;

            if (value.Count() > 0)
            {
                var firstRow = value.First();

                csvData += firstRow.Keys.ToCSV(k => k) + "\r\n";

                foreach (var item in value)
                {
                    csvData += item.Values.ToCSV(v => v.AsString().Replace(',', ' ')) + "\r\n";
                }
            }

            return System.Text.Encoding.ASCII.GetBytes(csvData);
        }

        public static bool Contains<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return source.Any(predicate);
        }

        public static bool HasDuplicates<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> predicate)
        {
            var set = new HashSet<TKey>();
            foreach (TSource item in source)
            {
                if (!set.Add(predicate(item)))
                {
                    return true;
                }
            }

            return false;
        }
        public static bool ContainsAndNotEmpty<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source.IsNull())
            {
                return false;
            }

            return source.Count() > 0 && source.Any(predicate);
        }

        public static bool AllAndNotEmpty<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return source.Count() > 0 && source.All(predicate);
        }

        public static bool HasItems<TSource>(this IEnumerable<TSource> source)
        {
            return source.IsNotNull() && source.Count() > 0;
        }

        public static bool HasItems(this IEnumerable source)
        {
            return source.OfType<object>().HasItems();
        }

        public static void ForEach<TSource>(this IEnumerable<TSource> list, Action<TSource> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }

        public static IEnumerable<TSource> Batch<TSource>(this IEnumerable<TSource> list, int batchNumber, int batchSize)
        {
            return list.Skip((batchNumber * batchSize) - batchSize).Take(batchSize);
        }

        public static void ApplyRounding<T>(this IEnumerable<T> values, decimal targetValue, Func<T, decimal> propertyValue, Action<T, decimal> modifyPropertyValue, bool capToTargetValue = false)
        {
            if (values.Count() > 0)
            {
                var currentTotalValue = values.Sum(v => propertyValue(v));

                if (capToTargetValue)
                {
                    if (currentTotalValue > targetValue)
                    {
                        var lastItem = values.Last();
                        var lastItemValue = propertyValue(lastItem);
                        lastItemValue -= currentTotalValue - targetValue;
                        modifyPropertyValue(lastItem, lastItemValue);
                    }
                }
                else
                {
                    if (currentTotalValue < targetValue)
                        throw new InvalidOperationException("Total sum of values may not be more than target value");
                }

                var results = new List<decimal>();

                foreach (var item in values)
                {
                    results.Add(propertyValue(item));
                }

                var total = results.Sum();
                var diff = targetValue - total;

                if (diff != 0)
                {
                    var itemIndex = results.IndexOf(results.OrderByDescending(f => f).First());
                    var value = values.ToArray()[itemIndex];
                    modifyPropertyValue(value, propertyValue(value) + diff);
                }
            }
        }

        public static IEnumerable<T> ReOrderList<T>(this IEnumerable<T> list, string[] order, Func<T, string> comparerDelegate)
        {
            var reorderedList = new List<T>();
            for (var i = 0; i < order.Length; i++)
            {
                var value = order[i];
                var items = list.Where(o => value == comparerDelegate.Invoke(o));
                if (items.HasItems())
                {
                    foreach (var item in items)
                    {
                        reorderedList.Add(item);
                    }
                }
            }
            return reorderedList;
        }

        public static string ToCSV<T>(this IEnumerable<T> value, Func<T, string> property)
        {
            return value.ToCSV(property, ",");
        }

        public static string ToCSV<T>(this IEnumerable<T> value, Func<T, string> property, string separator)
        {
            if (value.IsNotNull())
            {
                return string.Join(separator, value.Select(v => property(v)).ToArray());
            }
            return string.Empty;
        }

        public static IList<KeyValuePair<string, string>> ToKeyValuePair<T>(this IEnumerable<T> source, Func<T, string> key, Func<T, string> value)
        {
            var result = new List<KeyValuePair<string, string>>();

            foreach (var item in source)
            {
                result.Add(new KeyValuePair<string, string>(key.Invoke(item), value.Invoke(item)));
            }

            return result;
        }

        public static IList<KeyValuePair<string, decimal?>> ToKeyValuePair<T>(this IEnumerable<T> source, Func<T, string> key, Func<T, decimal?> value)
        {
            var result = new List<KeyValuePair<string, decimal?>>();

            foreach (var item in source)
            {
                result.Add(new KeyValuePair<string, decimal?>(key.Invoke(item), value.Invoke(item)));
            }

            return result;
        }

        public static IEnumerable<object> EnumerateObjects(this IEnumerable value)
        {
            var result = new List<object>();

            if (value.IsNotNull())
            {
                var enumerator = value.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    result.Add(enumerator.Current);
                }
            }

            return result;
        }

        public static IEnumerable<T> FilterInt<T>(this IEnumerable<T> result, Func<T, int?> propertyValue, int? value) where T : class
        {
            if (value.HasValue)
            {
                return result.Where(r => propertyValue(r).HasValue && propertyValue(r).Value == value.Value).ToList();
            }

            return result;
        }

        public static IEnumerable<T> FilterStringLike<T>(this IEnumerable<T> result, Func<T, string> propertyValue, string value) where T : class
        {
            if (value.IsNotNullOrEmpty())
            {
                return result.Where(r => propertyValue(r).IsNotNullOrEmpty() && propertyValue(r).IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            return result;
        }

        public static IEnumerable<T> FilterStringEquals<T>(this IEnumerable<T> result, Func<T, string> propertyValue, string value) where T : class
        {
            if (value.IsNotNullOrEmpty())
            {
                value = value.ToUpperInvariant();
                return result.Where(r => propertyValue(r).IsNotNullOrEmpty() && string.Compare(propertyValue(r), value, StringComparison.OrdinalIgnoreCase) == 0).ToList();
            }

            return result;
        }

        public static IEnumerable<T> FilterStringCSV<T>(this IEnumerable<T> result, Func<T, string> propertyValue, string value) where T : class
        {
            if (value.IsNotNullOrEmpty())
            {
                var values = value.ToUpperInvariant().Split(',');
                return result.Where(r => propertyValue(r).IsNotNullOrEmpty() && values.Contains(propertyValue(r).ToUpperInvariant())).ToList();
            }

            return result;
        }

        public static int SafeCount<T>(this IEnumerable<T> source)
        {
            if (source.IsNotNull())
            {
                return source.Count();
            }

            return 0;
        }
    }
}

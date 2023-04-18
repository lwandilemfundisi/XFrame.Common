using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace XFrame.Common.Extensions
{
    public static class IListExtensions
    {
        public static int IndexOf<T>(this IList<T> list, Func<T, bool> match)
        {
            var item = list.FirstOrDefault(match);
            if (item.IsNotNull())
            {
                return list.IndexOf(item);
            }
            return -1;
        }

        public static void SafeAdd<T>(this IList<T> list, T item)
        {
            if (list.IsNotNull() && item.IsNotNull() && !list.Contains(item))
            {
                list.Add(item);
            }
        }

        public static int RemoveAll<T>(this IList<T> list, Func<T, bool> match)
        {
            return RemoveItems(list, match).Count;
        }

        public static void AddRange<T>(this IList<T> list, IEnumerable<T> range)
        {
            foreach (T item in range)
            {
                list.Add(item);
            }
        }

        public static void SafeAddRange<T>(this IList<T> list, IEnumerable<T> range)
        {
            foreach (T item in range)
            {
                list.SafeAdd(item);
            }
        }

        public static IList<T> RemoveItems<T>(this IList<T> list, Func<T, bool> match)
        {
            var removedItems = new List<T>();

            if (list.IsNotNull())
            {
                foreach (T item in list.Where(match).ToList())
                {
                    list.Remove(item);
                    removedItems.Add(item);
                }
            }
            return removedItems;
        }

        public static void ForEach<T>(this IList<T> list, Action<T> action)
        {
            list.ToList<T>().ForEach(action);
        }

        public static T[] AsArray<T>(this IList<T> list)
        {
            if (list.IsNotNull())
            {
                return list.ToArray();
            }
            return new T[] { };
        }

        public static object[] CopyToArray(this IList list)
        {
            var result = new object[list.Count];

            list.CopyTo(result, 0);

            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace XFrame.Common.Extensions
{
    public static class GenericTypeExtensions
    {
        public static T IfNotNullReturnOtherwiseThrow<T>(this T obj)
        {
            if (obj != null)
            {
                return obj;
            }
            else
            {
                throw new NullReferenceException(obj.GetType().Name + " is null");
            }
        }
    }
}

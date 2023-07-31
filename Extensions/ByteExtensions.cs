using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Globalization;

namespace XFrame.Common.Extensions
{
    public static class ByteExtensions
    {
        public static bool IsEqualTo(this byte[] value, byte[] comparedTo)
        {
            if (value.IsNull() && comparedTo.IsNull())
            {
                return true;
            }

            if (value.IsNull() || comparedTo.IsNull())
            {
                return false;
            }

            if (value.Length != comparedTo.Length)
            {
                return false;
            }

            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] != comparedTo[i])
                {
                    return false;
                }
            }

            return true;
        }       
    }
}
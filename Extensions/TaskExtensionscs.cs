using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace XFrame.Common.Extensions
{
    public static class TaskExtensionscs
    {
        public static bool IsIn(this TaskStatus value, params TaskStatus[] args)
        {
            if (args.HasItems())
            {
                foreach (var arg in args)
                {
                    if (value == arg)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

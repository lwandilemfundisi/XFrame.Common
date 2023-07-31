using System;
using System.Collections.Generic;
using System.Text;

namespace XFrame.Common.Extensions
{
    public static class SourceIdExtensions
    {
        public static bool IsNone(this ISourceId sourceId)
        {
            return string.IsNullOrEmpty(sourceId?.Value);
        }
    }
}

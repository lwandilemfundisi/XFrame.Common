using System;
using XFrame.Common.Exceptions;
using XFrame.Common.Extensions;

namespace XFrame.Common
{
    public static class Invariant
    {
        public static void ArgumentNotNull(object argument, Func<string> argumentName)
        {
            if (argument == null)
            {
                throw new InvariantException(argumentName());
            }
        }

        public static void ArgumentNotEmpty(string argument, Func<string> argumentName)
        {
            if (argument.IsNullOrEmpty())
            {
                throw new InvariantException("Empty argument: " + argumentName());
            }
        }

        public static void IsNotNull(object value, Func<string> error)
        {
            if (value.IsNull())
            {
                throw new InvariantException(error());
            }
        }

        public static void IsNull(object value, Func<string> error)
        {
            if (value.IsNotNull())
            {
                throw new InvariantException(error());
            }
        }

        public static void IsTrue(bool test, Func<string> error)
        {
            if (!test)
            {
                throw new InvariantException(error());
            }
        }

        public static void IsFalse(bool test, Func<string> error)
        {
            if (test)
            {
                throw new InvariantException(error());
            }
        }

        public static void Fail(string error)
        {
            throw new InvariantException(error);
        }
    }
}
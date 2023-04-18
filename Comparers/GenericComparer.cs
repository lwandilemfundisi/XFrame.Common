using System;
using System.Collections.Generic;

namespace XFrame.Common.Comparers
{
    public class GenericComparer<T> : IComparer<T>
    {
        private Func<T, T, int> compareDelegate;

        #region Constructors

        public GenericComparer(Func<T, T, int> compareDelegate)
        {
            this.compareDelegate = compareDelegate;
        }

        #endregion

        #region IComparer members

        public int Compare(T x, T y)
        {
            return compareDelegate(x, y);
        }

        #endregion
    }
}

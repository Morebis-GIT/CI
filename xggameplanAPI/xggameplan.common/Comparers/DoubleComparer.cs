using System;
using System.Collections.Generic;

namespace xggameplan.Common
{
    public class DoubleComparer : IComparer<object>
    {
        public int Compare(object x, object y)
        {
            Double theX = (x != null && Double.TryParse(x.ToString(), out theX)) ? theX : Double.MaxValue;
            Double theY = (y != null && Double.TryParse(y.ToString(), out theY)) ? theY : Double.MaxValue;

            return theX.CompareTo(theY);
        }
    }
}

using System;
using System.Collections.Generic;

namespace xggameplan.Common
{
    public class DateTimeComparer : IComparer<object>
    {
        public int Compare(object x, object y)
        {
            DateTime theX = (x != null && DateTime.TryParse(x.ToString(), out theX)) ? theX : DateTime.MaxValue;
            DateTime theY = (y != null && DateTime.TryParse(y.ToString(), out theY)) ? theY : DateTime.MaxValue;

            return theX.CompareTo(theY);
        }
    }
}

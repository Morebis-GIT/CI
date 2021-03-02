using System;
using System.Collections.Generic;

namespace xggameplan.Common
{
    public class EnumNameComparer : IComparer<object>
    {
        public int Compare(object x, object y)
        {
            if (x == null)
            {
                return y == null ? 0 : -1;
            }
            else
            {
                if (y == null)
                {
                    return 1;
                }
                else
                {
                    var xStr = ((Enum)x).ToString();
                    var yStr = ((Enum)y).ToString();
                    return string.Compare(xStr, yStr, StringComparison.InvariantCulture);
                }
            }
        }
    }
}

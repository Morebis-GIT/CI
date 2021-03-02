using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.AutoBook
{
    public class AutoBookIdComparer : IEqualityComparer<AutoBookModel>
    {
        public bool Equals(AutoBookModel x, AutoBookModel y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.Id == y.Id;
        }

        public int GetHashCode(AutoBookModel obj) => 0;
    }
}

using Microsoft.EntityFrameworkCore;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions
{
    static public class DbFunctionsExtensions
    {
        public static bool Contains(this DbFunctions _, string propertyReference, string searchCondition)
        {
            return true;
        }
    }
}

using System.Linq;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer.Interception
{
    public static class FtsInterceptionHelpers
    {
        public static string ComputedField(params string[] values)
        {
            return values != null ? string.Join(" ", values.Where(s => !string.IsNullOrWhiteSpace(s))) : string.Empty;
        }
    }
}

using System;
using System.Globalization;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer
{
    public class SqlServerTenantIdentifierTestSequence : ISqlServerTenantIdentifierSequence
    {
        private int intSequenceValue = 0;

        public T GetNextValue<T>(string sequenceName) where T : IConvertible
        {
            if (typeof(T) == typeof(int))
            {
                intSequenceValue++;
                return (T)Convert.ChangeType(intSequenceValue, typeof(T), CultureInfo.InvariantCulture);
            }
            else
            {
                return default(T);
            }
        }
    }
}

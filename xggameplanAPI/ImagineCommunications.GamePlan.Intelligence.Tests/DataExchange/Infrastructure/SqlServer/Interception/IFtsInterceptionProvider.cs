using System;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer.Interception
{
    public interface IFtsInterceptionProvider
    {
        bool Contains<TEntity>(TEntity entity, string propertyName, string searchCondition);
        bool Contains(Type entityType, object entity, string propertyName, string searchCondition);
    }
}

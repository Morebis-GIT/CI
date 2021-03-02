using System;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces
{
    public interface ISqlServerSalesAreaByIdCacheAccessor : IEntityCacheAccessor<Guid, SalesArea>
    {
    }
}

using System;

namespace ImagineCommunications.GamePlan.Domain.Shared.System.Tenants
{
    public interface ITenantSettingsRepository
    {
        TenantSettings Get();

        void AddOrUpdate(TenantSettings tenantSettings);

        void SaveChanges();

        Guid GetDefaultSalesAreaPassPriorityId();

        Guid GetDefaultScenarioId();

        DayOfWeek GetStartDayOfWeek();
        void Truncate();
    }
}

using System;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;

namespace ImagineCommunications.GamePlan.Persistence.Memory.Repositories
{
    public class MemoryTenantSettingsRepository :
        MemoryRepositoryBase<TenantSettings>,
        ITenantSettingsRepository
    {
        public MemoryTenantSettingsRepository()
        {
        }

        public void AddOrUpdate(TenantSettings item)
        {
            InsertOrReplaceItem(item, item.Id.ToString());
        }

        public TenantSettings Get() =>
            GetAllItems().FirstOrDefault();

        public Guid GetDefaultSalesAreaPassPriorityId() => throw new NotImplementedException();

        public Guid GetDefaultScenarioId() => throw new NotImplementedException();

        public DayOfWeek GetStartDayOfWeek() => throw new NotImplementedException();

        public void SaveChanges()
        {
        }

        public void Truncate()
        {
            DeleteAllItems();
        }
    }
}

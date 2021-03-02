using System;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using xgcore.auditevents.Repository.File;

namespace ImagineCommunications.GamePlan.Persistence.File.Repositories
{
    public class FileTenantSettingsRepository
        : FileRepositoryBase, ITenantSettingsRepository
    {
        public FileTenantSettingsRepository(string folder)
            : base(folder, "tenantSetting")
        {
        }

        public void AddOrUpdate(TenantSettings item) =>
            UpdateOrInsertItem(_folder, _type, item, item.Id.ToString());

        public TenantSettings Get() =>
            GetAllByType<TenantSettings>(_folder, _type).FirstOrDefault();

        public Guid GetDefaultSalesAreaPassPriorityId() => throw new NotImplementedException();

        public Guid GetDefaultScenarioId() => throw new NotImplementedException();

        public DayOfWeek GetStartDayOfWeek() => throw new NotImplementedException();

        public void SaveChanges() => throw new NotImplementedException();

        public void Truncate() => throw new NotImplementedException();
    }
}

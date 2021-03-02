using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using TenantSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.TenantSettings.TenantSettings;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class TenantSettingsDomainModelHandler : IDomainModelHandler<TenantSettings>
    {
        private readonly ITenantSettingsRepository _tenantSettingsRepository;
        private readonly ISqlServerDbContext _dbContext;

        public TenantSettingsDomainModelHandler(
            ITenantSettingsRepository tenantSettingsRepository,
            ISqlServerDbContext dbContext)
        {
            _tenantSettingsRepository = tenantSettingsRepository ??
                                                 throw new ArgumentNullException(
                                                     nameof(tenantSettingsRepository));
            _dbContext = dbContext;
        }

        public TenantSettings Add(TenantSettings model)
        {
            _tenantSettingsRepository.AddOrUpdate(model);
            return model;
        }

        public void AddRange(params TenantSettings[] models)
        {
            var tenantSettings = models.FirstOrDefault();
            if(tenantSettings != null)
            {
                _tenantSettingsRepository.AddOrUpdate(tenantSettings);
            }
        }

        public virtual int Count() => _dbContext.Query<TenantSettingsEntity>().Count();

        public virtual void DeleteAll() => _dbContext.RemoveRange(_dbContext.Query<TenantSettingsEntity>().ToArray());

        public IEnumerable<TenantSettings> GetAll()
        {
            var tenantSettings = _tenantSettingsRepository.Get();
            return tenantSettings != null ? new[] {tenantSettings} : default(IEnumerable<TenantSettings>);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Settings;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using AutoBookSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.AutoBookSettings;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class AutoBookSettingsDomainModelHandler : IDomainModelHandler<AutoBookSettings>
    {
        private readonly IAutoBookSettingsRepository _autoBookSettingsRepository;
        private readonly ISqlServerTenantDbContext _dbContext;

        public AutoBookSettingsDomainModelHandler(
            IAutoBookSettingsRepository autoBookSettingsRepository,
            ISqlServerTenantDbContext dbContext)
        {
            _autoBookSettingsRepository = autoBookSettingsRepository;
            _dbContext = dbContext;
        }

        public AutoBookSettings Add(AutoBookSettings model)
        {
            _autoBookSettingsRepository.AddOrUpdate(model);
            return model;
        }

        public void AddRange(params AutoBookSettings[] models)
        {
            foreach (var model in models)
            {
                _ = Add(model);
            }
        }

        public int Count() => _dbContext.Query<AutoBookSettingsEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<AutoBookSettingsEntity>();

        public IEnumerable<AutoBookSettings> GetAll()
        {
            var model = _autoBookSettingsRepository.Get();
            return model != null ? new[] {model} : Enumerable.Empty<AutoBookSettings>();
        }
    }
}

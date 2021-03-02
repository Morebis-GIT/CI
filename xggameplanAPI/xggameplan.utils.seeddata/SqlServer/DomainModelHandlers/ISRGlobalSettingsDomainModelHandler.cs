using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRGlobalSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRGlobalSettings.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ISRGlobalSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ISRGlobalSettings;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class ISRGlobalSettingsDomainModelHandler : IDomainModelHandler<ISRGlobalSettings>
    {
        private readonly IISRGlobalSettingsRepository _repository;
        private readonly ISqlServerTenantDbContext _dbContext;

        public ISRGlobalSettingsDomainModelHandler(IISRGlobalSettingsRepository repository, ISqlServerTenantDbContext dbContext)
        {
            _repository = repository;
            _dbContext = dbContext;
        }

        public ISRGlobalSettings Add(ISRGlobalSettings model) =>
            _repository.Update(model);


        public void AddRange(params ISRGlobalSettings[] models) =>
            _repository.Update(models.SingleOrDefault() ?? new ISRGlobalSettings());

        public int Count() =>
            _dbContext.Query<ISRGlobalSettingsEntity>().Count();

        public void DeleteAll() =>
            _dbContext.Truncate<ISRGlobalSettingsEntity>();

        public IEnumerable<ISRGlobalSettings> GetAll()
        {
            var record = _repository.Get();
            return new List<ISRGlobalSettings> { record };
        }
    }
}

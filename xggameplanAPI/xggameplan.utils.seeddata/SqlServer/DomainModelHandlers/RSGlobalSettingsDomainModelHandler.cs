using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSGlobalSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSGlobalSettings.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using RSGlobalSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RSGlobalSettings;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class RSGlobalSettingsDomainModelHandler: IDomainModelHandler<RSGlobalSettings>
    {
        private readonly IRSGlobalSettingsRepository _repository;
        private readonly ISqlServerTenantDbContext _dbContext;

        public RSGlobalSettingsDomainModelHandler(IRSGlobalSettingsRepository repository, ISqlServerTenantDbContext dbContext)
        {
            _repository = repository;
            _dbContext = dbContext;
        }

        public RSGlobalSettings Add(RSGlobalSettings model) =>
            _repository.Update(model);

        public void AddRange(params RSGlobalSettings[] models) =>
            _repository.Update(models.Single());

        public int Count() =>
            _dbContext.Query<RSGlobalSettingsEntity>().Count();

        public void DeleteAll() =>
            _dbContext.Truncate<RSGlobalSettingsEntity>();

        public IEnumerable<RSGlobalSettings> GetAll()
        {
            var record = _repository.Get();
            return new List<RSGlobalSettings> { record };
        }
    }
}

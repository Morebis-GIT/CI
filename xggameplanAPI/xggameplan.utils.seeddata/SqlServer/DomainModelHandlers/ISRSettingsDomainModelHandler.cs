using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ISRSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ISRSettings.ISRSettings;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class ISRSettingsDomainModelHandler : IDomainModelHandler<ISRSettings>
    {
        private readonly IISRSettingsRepository _isrSettingsRepository;
        private readonly ISqlServerDbContext _dbContext;

        public ISRSettingsDomainModelHandler(IISRSettingsRepository isrSettingsRepository,
            ISqlServerDbContext dbContext)
        {
            _isrSettingsRepository =
                isrSettingsRepository ?? throw new ArgumentNullException(nameof(isrSettingsRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public ISRSettings Add(ISRSettings model)
        {
            AddRange(model);
            return model;
        }

        public void AddRange(params ISRSettings[] models) => _isrSettingsRepository.Add(models);

        public int Count() => _dbContext.Query<ISRSettingsEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<ISRSettingsEntity>();

        public IEnumerable<ISRSettings> GetAll() => _isrSettingsRepository.GetAll();
    }
}

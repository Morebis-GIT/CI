using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using RSSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RSSettings.RSSettings;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class RSSettingsDomainModelHandler : IDomainModelHandler<RSSettings>
    {
        private readonly IRSSettingsRepository _rsSettingsRepository;
        private readonly ISqlServerDbContext _dbContext;

        public RSSettingsDomainModelHandler(IRSSettingsRepository rsSettingsRepository,
            ISqlServerDbContext dbContext)
        {
            _rsSettingsRepository =
                rsSettingsRepository ?? throw new ArgumentNullException(nameof(rsSettingsRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public RSSettings Add(RSSettings model)
        {
            AddRange(model);
            return model;
        }

        public void AddRange(params RSSettings[] models) => _rsSettingsRepository.Add(models);

        public int Count() => _dbContext.Query<RSSettingsEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<RSSettingsEntity>();

        public IEnumerable<RSSettings> GetAll() => _rsSettingsRepository.GetAll();
    }
}

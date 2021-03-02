using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Autopilot.Settings;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using AutopilotSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutopilotSettings;


namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class AutopilotSettingsDomainModelHandler : IDomainModelHandler<AutopilotSettings>
    {
        private readonly IAutopilotSettingsRepository _repository;
        private readonly ISqlServerDbContext _dbContext;

        public AutopilotSettingsDomainModelHandler(IAutopilotSettingsRepository autopilotSettingsRepository, ISqlServerDbContext dbContext)
        {
            _repository = autopilotSettingsRepository ?? throw new ArgumentNullException(nameof(autopilotSettingsRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public AutopilotSettings Add(AutopilotSettings model)
        {
            _repository.Add(model);
            return model;
        }

        public void AddRange(params AutopilotSettings[] models)
        {
            foreach (var model in models)
            {
                _repository.Add(model);
            }
        }

        public int Count() => _dbContext.Query<AutopilotSettingsEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<AutopilotSettingsEntity>();

        public IEnumerable<AutopilotSettings> GetAll() => _repository.GetAll();
    }
}

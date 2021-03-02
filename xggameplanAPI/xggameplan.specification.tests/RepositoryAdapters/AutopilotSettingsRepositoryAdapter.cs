using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Autopilot.Settings;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class AutopilotSettingsRepositoryAdapter : RepositoryTestAdapter<AutopilotSettings, IAutopilotSettingsRepository, int>
    {
        public AutopilotSettingsRepositoryAdapter(IScenarioDbContext dbContext, IAutopilotSettingsRepository repository) : base(dbContext, repository)
        {
        }

        protected override AutopilotSettings Add(AutopilotSettings model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<AutopilotSettings> AddRange(params AutopilotSettings[] models) =>
            throw new NotImplementedException();

        protected override int Count() => throw new NotImplementedException();

        protected override void Delete(int id) => Repository.Delete(id);

        protected override IEnumerable<AutopilotSettings> GetAll() => Repository.GetAll();

        protected override AutopilotSettings GetById(int id) => Repository.Get(id);

        protected override void Truncate() => throw new NotImplementedException();

        protected override AutopilotSettings Update(AutopilotSettings model)
        {
            Repository.Update(model);
            return model;
        }
    }
}

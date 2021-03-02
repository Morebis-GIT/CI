using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSGlobalSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSGlobalSettings.Objects;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class RSGlobalSettingsRepositoryAdapter : RepositoryTestAdapter<RSGlobalSettings, IRSGlobalSettingsRepository, int>
    {
        public RSGlobalSettingsRepositoryAdapter(IScenarioDbContext dbContext, IRSGlobalSettingsRepository repository) : base(dbContext, repository)
        {
        }

        protected override RSGlobalSettings Add(RSGlobalSettings model) => throw new NotImplementedException();

        protected override IEnumerable<RSGlobalSettings> AddRange(params RSGlobalSettings[] models) => throw new NotImplementedException();

        protected override RSGlobalSettings Update(RSGlobalSettings model) => Repository.Update(model);

        protected override RSGlobalSettings GetById(int id) => throw new NotImplementedException();

        protected override IEnumerable<RSGlobalSettings> GetAll() => throw new NotImplementedException();

        protected override void Delete(int id) => throw new NotImplementedException();

        protected override void Truncate() => throw new NotImplementedException();

        protected override int Count() => throw new NotImplementedException();
    }
}

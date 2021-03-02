using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRGlobalSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRGlobalSettings.Objects;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class ISRGlobalSettingsRepositoryAdapter : RepositoryTestAdapter<ISRGlobalSettings, IISRGlobalSettingsRepository, int>
    {
        public ISRGlobalSettingsRepositoryAdapter(IScenarioDbContext dbContext, IISRGlobalSettingsRepository repository) : base(dbContext, repository)
        {
        }

        protected override ISRGlobalSettings Add(ISRGlobalSettings model) => throw new NotImplementedException();

        protected override IEnumerable<ISRGlobalSettings> AddRange(params ISRGlobalSettings[] models) => throw new NotImplementedException();

        protected override ISRGlobalSettings Update(ISRGlobalSettings model) => Repository.Update(model);

        protected override ISRGlobalSettings GetById(int id) => throw new NotImplementedException();

        protected override IEnumerable<ISRGlobalSettings> GetAll() => throw new NotImplementedException();

        protected override void Delete(int id) => throw new NotImplementedException();

        protected override void Truncate() => throw new NotImplementedException();

        protected override int Count() => throw new NotImplementedException();
    }
}

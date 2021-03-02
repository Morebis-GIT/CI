using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.EfficiencySettings;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class EfficiencySettingsRepositoryAdapter : RepositoryTestAdapter<EfficiencySettings, IEfficiencySettingsRepository, Guid>
    {
        public EfficiencySettingsRepositoryAdapter(IScenarioDbContext dbContext, IEfficiencySettingsRepository repository) : base(dbContext, repository)
        {
        }

        protected override EfficiencySettings Add(EfficiencySettings model) => throw new NotImplementedException();

        protected override IEnumerable<EfficiencySettings> AddRange(params EfficiencySettings[] models) => throw new NotImplementedException();

        protected override EfficiencySettings Update(EfficiencySettings model) => Repository.UpdateDefault(model);

        protected override EfficiencySettings GetById(Guid id) => throw new NotImplementedException();

        protected override IEnumerable<EfficiencySettings> GetAll() => throw new NotImplementedException();

        protected override void Delete(Guid id) => throw new NotImplementedException();

        protected override void Truncate() => throw new NotImplementedException();

        protected override int Count() => throw new NotImplementedException();       
    }
}

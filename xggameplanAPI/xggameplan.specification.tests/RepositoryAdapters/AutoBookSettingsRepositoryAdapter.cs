using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Settings;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class AutoBookSettingsRepositoryAdapter :
        RepositoryTestAdapter<AutoBookSettings, IAutoBookSettingsRepository, int>
    {
        public AutoBookSettingsRepositoryAdapter(
            IScenarioDbContext dbContext,
            IAutoBookSettingsRepository repository
            ) : base(dbContext, repository) { }

        protected override AutoBookSettings Add(AutoBookSettings model)
        {
            Repository.AddOrUpdate(model);
            return model;
        }

        protected override IEnumerable<AutoBookSettings> AddRange(params AutoBookSettings[] models) =>
            throw new NotImplementedException();

        protected override int Count() =>
            throw new NotImplementedException();

        protected override void Delete(int id) =>
            throw new NotImplementedException();

        protected override IEnumerable<AutoBookSettings> GetAll() =>
            throw new NotImplementedException();

        protected override AutoBookSettings GetById(int id) =>
            Repository.Get();

        protected override void Truncate() =>
            throw new NotImplementedException();

        protected override AutoBookSettings Update(AutoBookSettings model)
        {
            Repository.AddOrUpdate(model);
            return model;
        }
    }
}

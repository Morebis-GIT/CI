using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings.Objects;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class RSSettingsRepositoryAdapter : RepositoryTestAdapter<RSSettings, IRSSettingsRepository, int>
    {
        public RSSettingsRepositoryAdapter(IScenarioDbContext dbContext, IRSSettingsRepository repository) : base(dbContext, repository)
        {
        }

        protected override RSSettings Add(RSSettings model)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<RSSettings> AddRange(params RSSettings[] models)
        {
            Repository.Add(models);
            return models;
        }

        protected override int Count()
        {
            throw new NotImplementedException();
        }

        protected override void Delete(int id)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<RSSettings> GetAll()
        {
            return Repository.GetAll();
        }

        protected override RSSettings GetById(int id)
        {
            var all = Repository.GetAll();
            return all.FirstOrDefault(x => x.Id == id);
        }

        protected override void Truncate()
        {
            throw new NotImplementedException();
        }

        protected override RSSettings Update(RSSettings model)
        {
            Repository.Update(model);
            return model;
        }

        [RepositoryMethod]
        protected CallMethodResult Delete(string salesArea)
        {
            DbContext.WaitForIndexesAfterSaveChanges();
            Repository.Delete(salesArea);
            DbContext.SaveChanges();
            return CallMethodResult.CreateHandled();
        }
    }
}

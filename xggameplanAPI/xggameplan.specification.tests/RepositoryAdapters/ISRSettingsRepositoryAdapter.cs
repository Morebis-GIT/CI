using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings.Objects;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class ISRSettingsRepositoryAdapter : RepositoryTestAdapter<ISRSettings, IISRSettingsRepository, int>
    {
        public ISRSettingsRepositoryAdapter(IScenarioDbContext dbContext, IISRSettingsRepository repository) : base(dbContext, repository)
        {
        }

        protected override ISRSettings Add(ISRSettings model)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<ISRSettings> AddRange(params ISRSettings[] models)
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

        protected override IEnumerable<ISRSettings> GetAll()
        {
            return Repository.GetAll();
        }

        protected override ISRSettings GetById(int id)
        {
            var all = Repository.GetAll();
            return all.FirstOrDefault(x => x.Id == id);
        }

        protected override void Truncate()
        {
            throw new NotImplementedException();
        }

        protected override ISRSettings Update(ISRSettings model)
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

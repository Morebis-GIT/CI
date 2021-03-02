using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class ScenariosRepositoryAdapter : RepositoryTestAdapter<Scenario, IScenarioRepository, Guid>
    {
        public ScenariosRepositoryAdapter(IScenarioDbContext dbContext, IScenarioRepository repository) : base(dbContext, repository)
        {
        }

        protected override Scenario Add(Scenario model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<Scenario> AddRange(params Scenario[] models)
        {
            Repository.Add(models);
            return models;
        }

        protected override int Count()
        {
            throw new NotImplementedException();
        }

        protected override void Delete(Guid id)
        {
            Repository.Delete(id);
        }

        protected override IEnumerable<Scenario> GetAll()
        {
            return Repository.GetAll();
        }

        protected override Scenario GetById(Guid id)
        {
            return Repository.Get(id);
        }

        protected override void Truncate()
        {
            throw new NotImplementedException();
        }

        protected override Scenario Update(Scenario model)
        {
            Repository.Update(model);
            return model;
        }

        [RepositoryMethod]
        protected CallMethodResult Remove(IEnumerable<Guid> ids)
        {
            DbContext.WaitForIndexesAfterSaveChanges();
            Repository.Remove(ids);
            DbContext.SaveChanges();
            return CallMethodResult.CreateHandled();
        }

        [RepositoryMethod]
        protected CallMethodResult UpdateScenarios(Guid id, string name, int customId, bool isLibraried)
        {
            var scenario = Repository.Get(id);
            scenario.Name = name;
            scenario.CustomId = customId;
            scenario.IsLibraried = isLibraried;
            DbContext.WaitForIndexesAfterSaveChanges();
            Repository.Update(new List<Scenario>() { scenario });
            DbContext.SaveChanges();
            return CallMethodResult.CreateHandled();
        }
    }
}

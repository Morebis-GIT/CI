using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class UniverseRepositoryAdapter : RepositoryTestAdapter<Universe, IUniverseRepository, Guid>
    {
        public UniverseRepositoryAdapter(IScenarioDbContext dbContext, IUniverseRepository repository) : base(dbContext, repository)
        {
        }

        protected override Universe Add(Universe model)
        {
            Repository.Insert(new [] {model});
            return model;
        }

        protected override IEnumerable<Universe> AddRange(params Universe[] models)
        {
            Repository.Insert(models);
            return models;
        }

        protected override Universe Update(Universe model)
        {
            Repository.Update(model);
            return model;
        }

        protected override Universe GetById(Guid id)
        {
            return Repository.Find(id);
        }

        protected override IEnumerable<Universe> GetAll()
        {
            return Repository.GetAll();
        }

        protected override void Delete(Guid id)
        {
            Repository.Remove(id);
        }

        protected override void Truncate()
        {
            Repository.Truncate();
        }

        protected override int Count()
        {
            throw new NotImplementedException();
        }

        protected override Guid ConvertIdValue(string id)
        {
            return Guid.Parse(id);
        }

        [RepositoryMethod]
        protected CallMethodResult DeleteByCombination(
            string salesArea,
            string demographic,
            DateTime? startDate,
            DateTime? endDate)
        {
            DbContext.WaitForIndexesAfterSaveChanges();
            Repository.DeleteByCombination(salesArea, demographic, startDate, endDate);
            DbContext.SaveChanges();
            return CallMethodResult.CreateHandled();
        }
    }
}

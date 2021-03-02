using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.LengthFactors;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class LegnthFactorRepositoryAdapter : RepositoryTestAdapter<LengthFactor, ILengthFactorRepository, int>
    {
        public LegnthFactorRepositoryAdapter(IScenarioDbContext dbContext, ILengthFactorRepository repository) : base(dbContext, repository)
        {
        }

        protected override LengthFactor Add(LengthFactor model)
        {
            Repository.AddRange(new[] { model });
            return model;
        }

        protected override LengthFactor Update(LengthFactor model)
        {
            Repository.Update(model);
            return model;
        }

        protected override void Delete(int id) => Repository.Delete(id);

        protected override IEnumerable<LengthFactor> GetAll() => Repository.GetAll();

        protected override LengthFactor GetById(int id) => Repository.Get(id);

        protected override IEnumerable<LengthFactor> AddRange(params LengthFactor[] models)
        {
            Repository.AddRange(models);
            return models;
        }

        protected override int Count() => throw new NotImplementedException();

        protected override void Truncate() => Repository.Truncate();
    }
}

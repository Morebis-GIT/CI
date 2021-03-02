using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.IndexTypes;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class IndexTypeRepositoryAdapter : RepositoryTestAdapter<IndexType, IIndexTypeRepository, int>
    {
        public IndexTypeRepositoryAdapter(IScenarioDbContext dbContext, IIndexTypeRepository repository)
            : base(dbContext, repository)
        {
        }

        protected override IndexType Add(IndexType model)
        {
            Repository.Add(new List<IndexType>() { model });
            return model;
        }

        protected override IEnumerable<IndexType> AddRange(params IndexType[] models)
        {
            Repository.Add(models);
            return models;
        }

        protected override int Count() => Repository.CountAll;

        protected override void Delete(int id) => Repository.Remove(id);

        protected override IEnumerable<IndexType> GetAll() => Repository.GetAll();

        protected override IndexType GetById(int id) => Repository.Find(id);

        protected override void Truncate() => Repository.Truncate();

        protected override IndexType Update(IndexType model)
        {
            Repository.Update(model);
            return model;
        }
    }
}

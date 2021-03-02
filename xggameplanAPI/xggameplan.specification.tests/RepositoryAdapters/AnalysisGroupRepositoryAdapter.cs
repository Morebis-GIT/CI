using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups.Objects;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class AnalysisGroupRepositoryAdapter : RepositoryTestAdapter<AnalysisGroup, IAnalysisGroupRepository, int>
    {
        public AnalysisGroupRepositoryAdapter(IScenarioDbContext dbContext, IAnalysisGroupRepository repository) : base(dbContext, repository)
        {
        }

        protected override AnalysisGroup Add(AnalysisGroup model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<AnalysisGroup> AddRange(params AnalysisGroup[] models) => throw new System.NotImplementedException();

        protected override AnalysisGroup Update(AnalysisGroup model)
        {
            Repository.Update(model);
            return model;
        }

        protected override AnalysisGroup GetById(int id) => Repository.Get(id);

        protected override IEnumerable<AnalysisGroup> GetAll() => Repository.GetAll();

        protected override void Delete(int id) => Repository.Delete(id);

        protected override void Truncate() => throw new System.NotImplementedException();

        protected override int Count() => throw new System.NotImplementedException();
    }
}

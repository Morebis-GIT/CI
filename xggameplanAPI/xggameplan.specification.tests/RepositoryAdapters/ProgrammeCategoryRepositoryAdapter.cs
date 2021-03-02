using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class ProgrammeCategoryRepositoryAdapter : RepositoryTestAdapter<ProgrammeCategoryHierarchy, IProgrammeCategoryHierarchyRepository, int>
    {
        public ProgrammeCategoryRepositoryAdapter(IScenarioDbContext dbContext, IProgrammeCategoryHierarchyRepository repository) : base(dbContext,
            repository)
        {
        }

        protected override ProgrammeCategoryHierarchy Add(ProgrammeCategoryHierarchy model) => throw new NotImplementedException();

        protected override IEnumerable<ProgrammeCategoryHierarchy> AddRange(params ProgrammeCategoryHierarchy[] models)
        {
            Repository.AddRange(models);
            return models;
        }

        protected override ProgrammeCategoryHierarchy Update(ProgrammeCategoryHierarchy model) => throw new NotImplementedException();

        protected override ProgrammeCategoryHierarchy GetById(int id) => Repository.Get(id);

        protected override IEnumerable<ProgrammeCategoryHierarchy> GetAll() => Repository.GetAll();

        protected override void Delete(int id) => throw new NotImplementedException();

        protected override void Truncate() => Repository.Truncate();

        protected override int Count() => throw new NotImplementedException();
    }
}

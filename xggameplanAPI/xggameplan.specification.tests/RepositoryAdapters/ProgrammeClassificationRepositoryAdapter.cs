using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.ProgrammeClassifications;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class ProgrammeClassificationRepositoryAdapter : RepositoryTestAdapter<ProgrammeClassification, IProgrammeClassificationRepository, int>
    {
        public ProgrammeClassificationRepositoryAdapter(IScenarioDbContext dbContext, IProgrammeClassificationRepository repository) : base(dbContext, repository)
        {
        }

        protected override ProgrammeClassification Add(ProgrammeClassification model)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<ProgrammeClassification> AddRange(params ProgrammeClassification[] models)
        {
            Repository.Add(models);
            return models;
        }

        protected override int Count()
        {
            return Repository.CountAll;
        }

        protected override void Delete(int id)
        {
            Repository.Delete(id);
        }

        protected override IEnumerable<ProgrammeClassification> GetAll()
        {
            return Repository.GetAll();
        }

        protected override ProgrammeClassification GetById(int id)
        {
            return Repository.GetById(id);
        }

        protected override void Truncate()
        {
            Repository.Truncate();
        }

        protected override ProgrammeClassification Update(ProgrammeClassification model)
        {
            Repository.Update(model);
            return model;
        }
    }
}

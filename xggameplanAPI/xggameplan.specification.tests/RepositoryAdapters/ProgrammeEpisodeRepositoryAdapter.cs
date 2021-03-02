using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class ProgrammeEpisodeRepositoryAdapter : RepositoryTestAdapter<ProgrammeEpisode, IProgrammeEpisodeRepository, Guid>
    {
        public ProgrammeEpisodeRepositoryAdapter(IScenarioDbContext dbContext, IProgrammeEpisodeRepository repository) : base(dbContext, repository)
        {
        }

        protected override ProgrammeEpisode Add(ProgrammeEpisode model)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<ProgrammeEpisode> AddRange(params ProgrammeEpisode[] models)
        {
            throw new NotImplementedException();
        }

        protected override int Count()
        {
            throw new NotImplementedException();
        }

        protected override void Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<ProgrammeEpisode> GetAll() => Repository.GetAll();

        protected override ProgrammeEpisode GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        protected override void Truncate()
        {
            throw new NotImplementedException();
        }

        protected override ProgrammeEpisode Update(ProgrammeEpisode model)
        {
            throw new NotImplementedException();
        }
    }
}

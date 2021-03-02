using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Sponsorships;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class SponsorshipsRepositoryAdapter : RepositoryTestAdapter<Sponsorship, ISponsorshipRepository, string>
    {
        public SponsorshipsRepositoryAdapter(IScenarioDbContext dbContext, ISponsorshipRepository repository) : base(dbContext, repository)
        {
        }

        protected override Sponsorship Add(Sponsorship model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<Sponsorship> AddRange(params Sponsorship[] models) => throw new NotImplementedException();

        protected override int Count() => throw new NotImplementedException();

        protected override void Delete(string id) => throw new NotImplementedException();

        protected override IEnumerable<Sponsorship> GetAll() => Repository.GetAll();

        protected override Sponsorship GetById(string id) => throw new NotImplementedException();

        protected override void Truncate() => throw new NotImplementedException();

        protected override Sponsorship Update(Sponsorship model) => throw new NotImplementedException();
    }
}

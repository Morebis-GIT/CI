using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Sponsorships;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using SponsorshipEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Sponsorships.Sponsorship;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class SponsorshipDomainModelHandler : IDomainModelHandler<Sponsorship>
    {
        private readonly ISponsorshipRepository _sponsorshipRepository;
        private readonly ISqlServerDbContext _dbContext;

        public SponsorshipDomainModelHandler(ISponsorshipRepository sponsorshipRepository,
            ISqlServerDbContext dbContext)
        {
            _sponsorshipRepository =
                sponsorshipRepository ?? throw new ArgumentNullException(nameof(sponsorshipRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Sponsorship Add(Sponsorship model)
        {
            _sponsorshipRepository.Add(model);
            return model;
        }

        public void AddRange(params Sponsorship[] models)
        {
            foreach (var model in models)
            {
                _sponsorshipRepository.Add(model);
            }
        }

        public int Count() => _dbContext.Query<SponsorshipEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<SponsorshipEntity>();

        public IEnumerable<Sponsorship> GetAll() => _sponsorshipRepository.GetAll();
    }
}

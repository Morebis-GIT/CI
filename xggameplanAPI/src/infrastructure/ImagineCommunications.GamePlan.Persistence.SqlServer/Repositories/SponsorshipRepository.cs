using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Sponsorships;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using SponsorshipEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Sponsorships.Sponsorship;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class SponsorshipRepository : ISponsorshipRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public SponsorshipRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public Sponsorship Get(string externalReferenceId) =>
            _dbContext.Query<SponsorshipEntity>()
                .ProjectTo<Sponsorship>(_mapper.ConfigurationProvider)
                .FirstOrDefault(x => x.ExternalReferenceId == externalReferenceId);

        public IEnumerable<Sponsorship> GetAll() =>
            _dbContext.Query<SponsorshipEntity>()
                .ProjectTo<Sponsorship>(_mapper.ConfigurationProvider)
                .ToList();

        public void Add(Sponsorship sponsorship)
        {
            var entity = _mapper.Map<SponsorshipEntity>(sponsorship);
            _dbContext.Add(entity,
                post => post.MapTo(sponsorship), _mapper);
        }

        public void Update(Sponsorship sponsorship)
        {
            var entity = _dbContext.Query<SponsorshipEntity>()
                .Include(x => x.SponsoredItems)
                    .ThenInclude(x => x.AdvertiserExclusivities)
                .Include(x => x.SponsoredItems)
                    .ThenInclude(x => x.ClashExclusivities)
                .Include(x => x.SponsoredItems)
                    .ThenInclude(x => x.SponsorshipItems)
                        .ThenInclude(x => x.DayParts)
                .FirstOrDefault(x => x.Id == sponsorship.Id);

            if (entity != null)
            {
                _mapper.Map(sponsorship, entity);
                _dbContext.Update(entity, post => post.MapTo(sponsorship), _mapper);
            }
        }

        public void Delete(string externalReferenceId)
        {
            var entity = _dbContext.Query<SponsorshipEntity>()
                .FirstOrDefault(x => x.ExternalReferenceId == externalReferenceId);

            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public void SaveChanges() =>
            _dbContext.SaveChanges();

        public bool Exists(string externalReferenceId) =>
            Get(externalReferenceId) != null;

        public Task TruncateAsync() =>
            Task.Run(Truncate);

        private void Truncate() =>
            _dbContext.Truncate<SponsorshipEntity>();
    }
}

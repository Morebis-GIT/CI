using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Sponsorships;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Extensions.AutoMapper;
using SponsorshipEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Sponsorships.Sponsorship;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class SponsorshipRepository : ISponsorshipRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;
        private readonly IMapper _mapper;

        public SponsorshipRepository(ISqlServerTenantDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache, IMapper mapper)
        {
            _dbContext = dbContext;
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
            _mapper = mapper;
        }

        public Sponsorship Get(string externalReferenceId) =>
            _mapper.Map<Sponsorship>(SponsorshipQuery()
                    .FirstOrDefault(x => x.ExternalReferenceId == externalReferenceId),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public IEnumerable<Sponsorship> GetAll() =>
            _mapper.Map<List<Sponsorship>>(SponsorshipQuery().AsNoTracking(), opts => opts.UseEntityCache(_salesAreaByIdCache));

        public void Add(Sponsorship sponsorship)
        {
            var entity =
                _mapper.Map<SponsorshipEntity>(sponsorship, opts => opts.UseEntityCache(_salesAreaByNameCache));
            _ = _dbContext.Add(entity,
                post =>
                    post.MapTo(sponsorship,
                        opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);
        }

        public void Update(Sponsorship sponsorship)
        {
            var entity = SponsorshipQuery()
                .FirstOrDefault(x => x.Id == sponsorship.Id);

            if (entity != null)
            {
                _ = _mapper.Map(sponsorship,
                    entity,
                    opts => opts.UseEntityCache(_salesAreaByNameCache));

                _ = _dbContext.Update(entity,
                    post =>
                        post.MapTo(sponsorship,
                            opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);
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

        private IQueryable<SponsorshipEntity> SponsorshipQuery()
        {
            return _dbContext.Query<SponsorshipEntity>()
                .Include(x => x.SponsoredItems)
                .ThenInclude(x => x.AdvertiserExclusivities)
                .Include(x => x.SponsoredItems)
                .ThenInclude(x => x.ClashExclusivities)
                .Include(x => x.SponsoredItems)
                .ThenInclude(x => x.SponsorshipItems)
                .ThenInclude(x => x.DayParts)
                .Include(x => x.SponsoredItems)
                .ThenInclude(x => x.SponsorshipItems)
                .ThenInclude(x => x.SalesAreas);
        }
    }
}

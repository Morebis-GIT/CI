using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.SpotBookingRules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class SpotBookingRuleRepository : ISpotBookingRuleRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public SpotBookingRuleRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IEnumerable<SpotBookingRule> GetAll() =>
            _dbContext.Query<Entities.Tenant.SpotBookingRules.SpotBookingRule>()
                .ProjectTo<SpotBookingRule>(_mapper.ConfigurationProvider)
                .ToList();

        public void AddRange(IEnumerable<SpotBookingRule> spotBookingRules) =>
            _dbContext.AddRange(_mapper.Map<Entities.Tenant.SpotBookingRules.SpotBookingRule[]>(spotBookingRules),
                post => post.MapToCollection(spotBookingRules), _mapper);

        public void SaveChanges() => _dbContext.SaveChanges();

        public void Truncate() => _dbContext.Truncate<Entities.Tenant.SpotBookingRules.SpotBookingRule>();

        public SpotBookingRule Get(int id) =>
            _mapper.Map<SpotBookingRule>(_dbContext.Find<Entities.Tenant.SpotBookingRules.SpotBookingRule>(new object[] { id },
                post => post.IncludeCollection(e => e.SalesAreas)));
    }
}

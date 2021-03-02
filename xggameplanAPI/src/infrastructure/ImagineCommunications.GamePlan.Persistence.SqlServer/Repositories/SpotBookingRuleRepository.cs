using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.SpotBookingRules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Extensions.AutoMapper;
using SpotBookingRuleEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SpotBookingRules.SpotBookingRule;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class SpotBookingRuleRepository : ISpotBookingRuleRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;
        private readonly IMapper _mapper;

        public SpotBookingRuleRepository(ISqlServerTenantDbContext dbContext, ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
          ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache, IMapper mapper)
        {
            _dbContext = dbContext;
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
            _mapper = mapper;
        }

        public IEnumerable<SpotBookingRule> GetAll()
        {
            var @spotBookingRules =  _mapper.Map< List<SpotBookingRule>>(_dbContext.Query<SpotBookingRuleEntity>()
                .Include(x => x.SalesAreas).AsNoTracking(), opts => opts.UseEntityCache(_salesAreaByIdCache));
            return @spotBookingRules;
        }

        public void AddRange(IEnumerable<SpotBookingRule> spotBookingRules) {
            var spotBookingRulesEntities = _mapper.Map<SpotBookingRuleEntity[]>(spotBookingRules,opts => opts.UseEntityCache(_salesAreaByNameCache));
            _dbContext.AddRange(spotBookingRulesEntities,
                post => post.MapToCollection(spotBookingRules, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);
        }

        public void SaveChanges() => _dbContext.SaveChanges();

        public void Truncate() => _dbContext.Truncate<SpotBookingRuleEntity>();

        public SpotBookingRule Get(int id) =>
            _mapper.Map<SpotBookingRule>(_dbContext.Find<SpotBookingRuleEntity>(new object[] { id },
                post => post.IncludeCollection(e => e.SalesAreas)), opts => opts.UseEntityCache(_salesAreaByIdCache));
    }
}

using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreaDemographics;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Extensions.AutoMapper;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class SalesAreaDemographicRepository : ISalesAreaDemographicRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;
        private readonly IMapper _mapper;

        public SalesAreaDemographicRepository(
            ISqlServerTenantDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
            _mapper = mapper;
        }

        public void AddRange(IEnumerable<SalesAreaDemographic> entities) =>
            _dbContext.AddRange(_mapper.Map<Entities.Tenant.SalesAreas.SalesAreaDemographic[]>(entities, opts => opts.UseEntityCache(_salesAreaByNameCache)),
                post => post.MapToCollection(entities, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);

        public void DeleteBySalesAreaName(string salesAreaName)
        {
            var salesAreaDemographics = _dbContext.Query<Entities.Tenant.SalesAreas.SalesAreaDemographic>()
                .Where(x => x.SalesArea.Name == salesAreaName)
                .ToArray();

            if (salesAreaDemographics.Any())
            {
                _dbContext.RemoveRange(salesAreaDemographics);
            }
        }

        public void DeleteBySalesAreaNames(IEnumerable<string> salesAreaNames)
        {
            var salesAreaDemographics = _dbContext.Query<Entities.Tenant.SalesAreas.SalesAreaDemographic>()
                .Where(x => salesAreaNames.Contains(x.SalesArea.Name))
                .ToArray();

            if (salesAreaDemographics.Any())
            {
                _dbContext.RemoveRange(salesAreaDemographics);
            }
        }

        public IEnumerable<SalesAreaDemographic> GetAll() =>
            _mapper.Map<List<SalesAreaDemographic>>(
                _dbContext.Query<Entities.Tenant.SalesAreas.SalesAreaDemographic>().AsNoTracking(),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public void SaveChanges() => _dbContext.SaveChanges();
    }
}

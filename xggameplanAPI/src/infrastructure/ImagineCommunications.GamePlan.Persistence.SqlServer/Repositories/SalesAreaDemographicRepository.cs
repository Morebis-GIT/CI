using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreaDemographics;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class SalesAreaDemographicRepository : ISalesAreaDemographicRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public SalesAreaDemographicRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void AddRange(IEnumerable<SalesAreaDemographic> entities) =>
            _dbContext.AddRange(_mapper.Map<Entities.Tenant.SalesAreas.SalesAreaDemographic[]>(entities),
                post => post.MapToCollection(entities), _mapper);

        public void DeleteBySalesAreaName(string salesAreaName)
        {
            var salesAreaDemographics = _dbContext.Query<Entities.Tenant.SalesAreas.SalesAreaDemographic>()
                .Where(x => x.SalesArea == salesAreaName)
                .ToArray();

            if (salesAreaDemographics.Any())
            {
                _dbContext.RemoveRange(salesAreaDemographics);
            }
        }

        public void DeleteBySalesAreaNames(IEnumerable<string> salesAreaNames)
        {
            var salesAreaDemographics = _dbContext.Query<Entities.Tenant.SalesAreas.SalesAreaDemographic>()
                .Where(x => salesAreaNames.Contains(x.SalesArea))
                .ToArray();

            if (salesAreaDemographics.Any())
            {
                _dbContext.RemoveRange(salesAreaDemographics);
            }
        }

        public IEnumerable<SalesAreaDemographic> GetAll() =>
            _dbContext.Query<Entities.Tenant.SalesAreas.SalesAreaDemographic>()
                .ProjectTo<SalesAreaDemographic>(_mapper.ConfigurationProvider)
                .ToList();

        public void SaveChanges() => _dbContext.SaveChanges();
    }
}

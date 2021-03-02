using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.BusinessTypes;
using ImagineCommunications.GamePlan.Domain.BusinessTypes.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class BusinessTypeRepository : IBusinessTypeRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public BusinessTypeRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IEnumerable<BusinessType> GetAll() =>
            _dbContext.Query<Entities.Tenant.BusinessTypes.BusinessType>()
            .ProjectTo<BusinessType>(_mapper.ConfigurationProvider)
            .ToList();

        public BusinessType GetByCode(string code) =>
            _dbContext.Query<Entities.Tenant.BusinessTypes.BusinessType>()
            .ProjectTo<BusinessType>(_mapper.ConfigurationProvider)
            .FirstOrDefault(b => b.Code == code);

        public bool Exists(string code) => _dbContext.Query<Entities.Tenant.BusinessTypes.BusinessType>()
            .ProjectTo<BusinessType>(_mapper.ConfigurationProvider)
            .Any(b => b.Code == code);
    }
}

using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BusinessTypes.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using xggameplan.specification.tests.Interfaces;
using BusinessTypeEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BusinessTypes.BusinessType;


namespace xggameplan.specification.tests.Infrastructure.SqlServer.DomainModelHandlers
{
    public class BusinessTypeDomainModelHandler : SimpleDomainModelMappingHandler<BusinessTypeEntity, BusinessType>
    {
        private readonly ITenantDbContext _dbContext;

        public BusinessTypeDomainModelHandler(ISqlServerTestDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
        }

        public override BusinessType Add(BusinessType model)
        {
            _dbContext.AddRange(Mapper.Map<List<BusinessTypeEntity>>(model));

            return model;
        }

        public override void AddRange(params BusinessType[] models)
        {
            var entities = models.SelectMany(x => Mapper.Map<List<BusinessTypeEntity>>(x)).ToArray();

            _dbContext.AddRange(entities);
        }

        public override IEnumerable<BusinessType> GetAll()
        {
            var entities = _dbContext.Query<BusinessTypeEntity>().ToList();

            return Mapper.Map<List<BusinessType>>(entities);
        }
    }
}

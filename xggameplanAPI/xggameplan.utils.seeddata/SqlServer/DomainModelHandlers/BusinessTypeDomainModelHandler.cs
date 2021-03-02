using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BusinessTypes;
using ImagineCommunications.GamePlan.Domain.BusinessTypes.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using BusinessTypeEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BusinessTypes.BusinessType;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class BusinessTypeDomainModelHandler : IDomainModelHandler<BusinessType>
    {
        private readonly IBusinessTypeRepository _repository;
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public BusinessTypeDomainModelHandler(IBusinessTypeRepository businessTypeRepository, ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _repository = businessTypeRepository ?? throw new ArgumentNullException(nameof(businessTypeRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;
        }

        public BusinessType Add(BusinessType model)
        {
            _ = _dbContext.Add(_mapper.Map<BusinessTypeEntity>(model), post => post.MapTo(model), _mapper);
            return model;
        }

        public void AddRange(params BusinessType[] models) =>
            _dbContext.AddRange(_mapper.Map<BusinessTypeEntity[]>(models), post => post.MapToCollection(models), _mapper);

        public int Count() => _dbContext.Query<BusinessTypeEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<BusinessTypeEntity>();

        public IEnumerable<BusinessType> GetAll() => _repository.GetAll();
    }
}

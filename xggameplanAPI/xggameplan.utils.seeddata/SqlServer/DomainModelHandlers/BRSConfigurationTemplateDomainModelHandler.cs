using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BRS;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using BRSConfigurationTemplateEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BRS.BRSConfigurationTemplate;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class BRSConfigurationTemplateDomainModelHandler : IDomainModelHandler<BRSConfigurationTemplate>
    {
        private readonly IBRSConfigurationTemplateRepository _repository;
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public BRSConfigurationTemplateDomainModelHandler(IBRSConfigurationTemplateRepository brsConfigurationTemplateRepository, ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _repository = brsConfigurationTemplateRepository ?? throw new ArgumentNullException(nameof(brsConfigurationTemplateRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;
        }

        public BRSConfigurationTemplate Add(BRSConfigurationTemplate model)
        {
            _ = _dbContext.Add(_mapper.Map<BRSConfigurationTemplateEntity>(model), post => post.MapTo(model), _mapper);
            return model;
        }

        public void AddRange(params BRSConfigurationTemplate[] models) =>
            _dbContext.AddRange(_mapper.Map<BRSConfigurationTemplateEntity[]>(models), post => post.MapToCollection(models), _mapper);

        public int Count() => _dbContext.Query<BRSConfigurationTemplateEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<BRSConfigurationTemplateEntity>();

        public IEnumerable<BRSConfigurationTemplate> GetAll() => _repository.GetAll();
    }
}

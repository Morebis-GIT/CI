using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BRS;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using KPIPriorityEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BRS.KPIPriority;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class KPIPriorityDomainModelHandler : IDomainModelHandler<KPIPriority>
    {
        private readonly IKPIPriorityRepository _repository;
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public KPIPriorityDomainModelHandler(IKPIPriorityRepository kpiPriorityRepository, ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _repository = kpiPriorityRepository ?? throw new ArgumentNullException(nameof(kpiPriorityRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;
        }

        public KPIPriority Add(KPIPriority model)
        {
            _ = _dbContext.Add(_mapper.Map<KPIPriorityEntity>(model), post => post.MapTo(model), _mapper);
            return model;
        }

        public void AddRange(params KPIPriority[] models) =>
            _dbContext.AddRange(_mapper.Map<KPIPriorityEntity[]>(models), post => post.MapToCollection(models), _mapper);

        public int Count() => _dbContext.Query<KPIPriorityEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<KPIPriorityEntity>();

        public IEnumerable<KPIPriority> GetAll() => _repository.GetAll();
    }
}

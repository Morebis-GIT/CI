using System;
using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using DemographicEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Demographic;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class DemographicDomainModelHandler : IDomainModelHandler<Demographic>
    {
        private readonly IDemographicRepository _demographicRepository;
        private readonly ISqlServerDbContext _dbContext;
        private readonly IMapper _mapper;

        public DemographicDomainModelHandler(IDemographicRepository demographicRepository,
            ISqlServerDbContext dbContext, IMapper mapper)
        {
            _demographicRepository =
                demographicRepository ?? throw new ArgumentNullException(nameof(demographicRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;
        }

        public Demographic Add(Demographic model)
        {
            _ = _dbContext.Add(_mapper.Map<DemographicEntity>(model), post => post.MapTo(model), _mapper);
            return model;
        }

        public void AddRange(params Demographic[] models) => _demographicRepository.Add(models);

        public int Count() => _demographicRepository.CountAll;

        public void DeleteAll() => _demographicRepository.Truncate();

        public IEnumerable<Demographic> GetAll() => _demographicRepository.GetAll();
    }
}

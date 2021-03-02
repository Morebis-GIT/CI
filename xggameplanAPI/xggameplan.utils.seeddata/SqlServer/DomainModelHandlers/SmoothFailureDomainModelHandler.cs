using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using SmoothFailureEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.SmoothFailure;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class SmoothFailureDomainModelHandler : IDomainModelHandler<SmoothFailure>
    {
        private readonly ISmoothFailureRepository _smoothFailureRepository;
        private readonly ISqlServerDbContext _dbContext;
        private readonly IMapper _mapper;

        public SmoothFailureDomainModelHandler(
            ISmoothFailureRepository smoothFailureRepository,
            ISqlServerDbContext dbContext,
            IMapper mapper)
        {
            _smoothFailureRepository = smoothFailureRepository ?? throw new ArgumentNullException(nameof(smoothFailureRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;
        }

        public SmoothFailure Add(SmoothFailure model)
        {
            _ = _dbContext.Add(_mapper.Map<SmoothFailureEntity>(model), post => post.MapTo(model), _mapper);
            return model;
        }

        public void AddRange(params SmoothFailure[] models) =>
                _smoothFailureRepository.AddRange(models);

        public int Count() =>
            _dbContext.Query<SmoothFailureEntity>().Count();

        public void DeleteAll() =>
            _dbContext.Truncate<SmoothFailureEntity>();

        public IEnumerable<SmoothFailure> GetAll() =>
            _dbContext.Query<SmoothFailureEntity>().ProjectTo<SmoothFailure>(_mapper.ConfigurationProvider).ToList();
    }
}

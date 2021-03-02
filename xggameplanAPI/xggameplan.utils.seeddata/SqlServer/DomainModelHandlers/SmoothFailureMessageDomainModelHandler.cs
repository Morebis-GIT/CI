using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using SmoothFailureMessageEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.SmoothFailureMessage;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class SmoothFailureMessageDomainModelHandler : IDomainModelHandler<SmoothFailureMessage>
    {
        private readonly ISmoothFailureMessageRepository _smoothFailureMessageRepository;
        private readonly ISqlServerDbContext _dbContext;
        private readonly IMapper _mapper;

        public SmoothFailureMessageDomainModelHandler(
            ISmoothFailureMessageRepository smoothFailureMessageRepository,
            ISqlServerDbContext dbContext,
            IMapper mapper)
        {
            _smoothFailureMessageRepository = smoothFailureMessageRepository ?? throw new ArgumentNullException(nameof(smoothFailureMessageRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;
        }

        public SmoothFailureMessage Add(SmoothFailureMessage model)
        {
            _ = _dbContext.Add(_mapper.Map<SmoothFailureMessageEntity>(model), post => post.MapTo(model), _mapper);
            return model;
        }

        public void AddRange(params SmoothFailureMessage[] models) =>
            _smoothFailureMessageRepository.Add(models);

        public int Count() =>
            _dbContext.Query<SmoothFailureMessageEntity>().Count();

        public void DeleteAll() =>
            _dbContext.Truncate<SmoothFailureMessageEntity>();

        public IEnumerable<SmoothFailureMessage> GetAll() =>
            _smoothFailureMessageRepository.GetAll();
    }
}

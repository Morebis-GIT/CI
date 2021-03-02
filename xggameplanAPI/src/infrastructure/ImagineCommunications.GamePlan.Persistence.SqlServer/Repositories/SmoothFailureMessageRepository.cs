using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using SmoothFailureMessageEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.SmoothFailureMessage;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class SmoothFailureMessageRepository
        : ISmoothFailureMessageRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public SmoothFailureMessageRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IEnumerable<SmoothFailureMessage> GetAll() =>
            _dbContext.Query<SmoothFailureMessageEntity>()
                .ProjectTo<SmoothFailureMessage>(_mapper.ConfigurationProvider)
                .ToList();

        public void Add(IEnumerable<SmoothFailureMessage> items)
        {
            _dbContext.AddRange(_mapper.Map<IEnumerable<SmoothFailureMessageEntity>>(items).ToArray(),
                post => post.MapToCollection(items), _mapper);
        }

        public void Truncate() => throw new NotImplementedException();

        public void SaveChanges() => throw new NotImplementedException();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using UniverseEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Universe;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class UniverseDomainModelHandler : IDomainModelHandler<Universe>
    {
        private readonly IUniverseRepository _universeRepository;
        private readonly ISqlServerDbContext _dbContext;
        private readonly IMapper _mapper;

        public UniverseDomainModelHandler(
            IUniverseRepository universeRepository,
            ISqlServerDbContext dbContext,
            IMapper mapper)
        {
            _universeRepository = universeRepository ?? throw new ArgumentNullException(nameof(universeRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;
        }

        public Universe Add(Universe model)
        {
            _ = _dbContext.Add(_mapper.Map<UniverseEntity>(model), post => post.MapTo(model), _mapper);
            return model;
        }

        public void AddRange(params Universe[] models) => _universeRepository.Insert(models);

        public int Count() => _dbContext.Query<UniverseEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<UniverseEntity>();

        public IEnumerable<Universe> GetAll() => _universeRepository.GetAll();
    }
}

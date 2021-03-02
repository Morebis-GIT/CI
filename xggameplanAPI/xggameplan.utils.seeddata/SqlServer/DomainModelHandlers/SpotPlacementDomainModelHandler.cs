using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.SpotPlacements;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using SpotPlacementEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SpotPlacement;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class SpotPlacementDomainModelHandler : IDomainModelHandler<SpotPlacement>
    {
        private readonly ISpotPlacementRepository _spotPlacementRepository;
        private readonly ISqlServerDbContext _dbContext;
        private readonly IMapper _mapper;

        public SpotPlacementDomainModelHandler(
            ISpotPlacementRepository spotPlacementRepository,
            ISqlServerDbContext dbContext,
            IMapper mapper)
        {
            _spotPlacementRepository = spotPlacementRepository ??
                                       throw new ArgumentNullException(nameof(spotPlacementRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;
        }

        public SpotPlacement Add(SpotPlacement model)
        {
            _spotPlacementRepository.Add(model);
            return model;
        }

        public void AddRange(params SpotPlacement[] models) => _spotPlacementRepository.Insert(models);

        public int Count() => _dbContext.Query<SpotPlacementEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<SpotPlacementEntity>();

        public IEnumerable<SpotPlacement> GetAll() =>
            _dbContext.Query<SpotPlacementEntity>().ProjectTo<SpotPlacement>(_mapper.ConfigurationProvider).AsEnumerable();
    }
}

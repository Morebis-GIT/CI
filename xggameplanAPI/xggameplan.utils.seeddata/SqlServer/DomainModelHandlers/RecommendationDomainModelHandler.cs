using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Recommendations;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using RecommendationEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Recommendation;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class RecommendationDomainModelHandler : IDomainModelHandler<Recommendation>
    {
        private readonly IRecommendationRepository _repository;
        private readonly ISqlServerDbContext _dbContext;
        private readonly IMapper _mapper;

        public RecommendationDomainModelHandler(
            IRecommendationRepository repository,
            ISqlServerDbContext dbContext,
            IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;
        }

        public Recommendation Add(Recommendation model)
        {
            AddRange(model);
            return model;
        }

        public void AddRange(params Recommendation[] models)
        {
            _repository.Insert(models);
        }

        public int Count() => _dbContext.Query<RecommendationEntity>().Count();
        
        public void DeleteAll() => _dbContext.Truncate<RecommendationEntity>();

        public IEnumerable<Recommendation> GetAll() =>
            _dbContext.Query<RecommendationEntity>().ProjectTo<Recommendation>(_mapper.ConfigurationProvider).AsEnumerable();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Views.Tenant;
using MySql.Data.MySqlClient;
using xggameplan.core.Interfaces;
using xggameplan.Model;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Landmark.Features.LandmarkProductRelatedCollections
{
    /// <summary>
    /// Aggregates recommendation data into collection of <see cref="RecommendationAggregateModel"/>
    /// </summary>
    /// <seealso cref="xggameplan.core.Interfaces.IRecommendationAggregator" />
    public class RecommendationAggregator : IRecommendationAggregator
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        /// <summary>Initializes a new instance of the <see cref="RecommendationAggregator" /> class.</summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="mapper">The mapper.</param>
        public RecommendationAggregator(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>Aggregates recommendation for the specified scenario identifier.</summary>
        /// <param name="scenarioId">The scenario identifier.</param>
        /// <returns></returns>
        public IReadOnlyCollection<RecommendationAggregateModel> Aggregate(Guid scenarioId)
        {
            return _dbContext.Specific
                .StoredProcedure<RecommendationAggregate>(new MySqlParameter("@ScenarioId", scenarioId))
                .ProjectTo<RecommendationAggregateModel>(_mapper.ConfigurationProvider).ToArray();
        }
    }
}

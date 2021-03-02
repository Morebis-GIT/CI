using System;
using System.Collections.Generic;
using xggameplan.Model;

namespace xggameplan.core.Interfaces
{
    /// <summary>
    /// Aggregates recommendation data into collection of <see cref="RecommendationAggregateModel"/>
    /// </summary>
    public interface IRecommendationAggregator
    {
        /// <summary>Aggregates recommendation for the specified scenario identifier.</summary>
        /// <param name="scenarioId">The scenario identifier.</param>
        /// <returns></returns>
        IReadOnlyCollection<RecommendationAggregateModel> Aggregate(Guid scenarioId);
    }
}

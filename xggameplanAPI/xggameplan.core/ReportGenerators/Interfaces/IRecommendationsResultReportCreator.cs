using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using xggameplan.Model;

namespace xggameplan.core.ReportGenerators.Interfaces
{
    /// <summary>
    /// Recommendation result report creator.
    /// </summary>
    public interface IRecommendationsResultReportCreator : IReportCreator<Recommendation, RecommendationExtendedModel>
    {
    }
}

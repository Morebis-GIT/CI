using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Views.Tenant;
using xggameplan.Model;
using RecommendationEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Recommendation;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class RecommendationProfile : Profile
    {
        public RecommendationProfile()
        {
            CreateMap<Recommendation, RecommendationEntity>()
                .ReverseMap();

            CreateMap<RecommendationEntity, RecommendationSimple>();

            CreateMap<Recommendation, RecommendationReducedModel>();
            CreateMap<RecommendationAggregate, RecommendationAggregateModel>();
        }
    }
}

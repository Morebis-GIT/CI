using AutoMapper;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures.Objects;
using ScenarioCampaignFailureEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioCampaignFailure;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class ScenarioCampaignFailureProfile : Profile
    {
        public ScenarioCampaignFailureProfile()
        {
            CreateMap<ScenarioCampaignFailureEntity, ScenarioCampaignFailure>()
                .ReverseMap();
        }
    }
}

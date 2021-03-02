using ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures.Objects;

namespace xggameplan.Profile
{
    public class ScenarioCampaignFailureProfile : AutoMapper.Profile
    {
        public ScenarioCampaignFailureProfile()
        {
            CreateMap<ScenarioCampaignFailureModel, ScenarioCampaignFailureExportModel>();
        }
    }
}

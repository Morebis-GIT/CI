using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects;
using xggameplan.CSVImporter;

namespace xggameplan.Profile
{
    public class ScenarioCampaignResultProfile : AutoMapper.Profile
    {
        public ScenarioCampaignResultProfile()
        {
            CreateMap<ScenarioCampaignLevelResultImport, ScenarioCampaignLevelResultItem>();
        }
    }
}

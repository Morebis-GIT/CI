using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using xggameplan.Model;

namespace ImagineCommunications.GamePlan.Intelligence.Configurations.Mappings
{
    public class CampaignProfile : xggameplan.Profile.CampaignProfile
    {
        public CampaignProfile()
        {
            CreateMap<PassPriority, CreatePassPriorityModel>().ReverseMap();
        }
    }
}

using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using xggameplan.Model;

namespace xggameplan.Profile
{

    internal class CampaignReferenceModelProfile : AutoMapper.Profile
    {
        public CampaignReferenceModelProfile()
        {
            CreateMap<CampaignReference, CampaignReferenceModel>().ReverseMap();
        }
    }
}

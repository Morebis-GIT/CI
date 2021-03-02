using ImagineCommunications.GamePlan.Domain.Autopilot.FlexibilityLevels;
using xggameplan.Model;

namespace xggameplan.Profile
{
    public class FlexibilityLevelProfile : AutoMapper.Profile
    {
        public FlexibilityLevelProfile()
        {
            CreateMap<FlexibilityLevel, FlexibilityLevelModel>().ReverseMap();
        }
    }
}

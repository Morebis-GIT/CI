using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Autopilot.FlexibilityLevels;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class FlexibilityLevelProfile : Profile
    {
        public FlexibilityLevelProfile()
        {
            CreateMap<FlexibilityLevel, Entities.Tenant.FlexibilityLevel>().ReverseMap();
        }
    }
}

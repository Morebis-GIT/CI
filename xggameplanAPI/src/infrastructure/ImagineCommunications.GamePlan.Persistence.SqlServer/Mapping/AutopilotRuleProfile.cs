using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class AutopilotRuleProfile : Profile
    {
        public AutopilotRuleProfile()
        {
            CreateMap<AutopilotRule, Domain.Autopilot.Rules.AutopilotRule>().ReverseMap();
        }
    }
}

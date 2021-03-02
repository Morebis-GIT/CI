using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class PassRuleProfile : AutoMapper.Profile
    {
        public PassRuleProfile()
        {
            CreateMap<PassRule, PassRuleModel>().ReverseMap();
        }
    }
}

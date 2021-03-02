using ImagineCommunications.GamePlan.Domain.BusinessRules.Rules;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class RuleProfile : AutoMapper.Profile
    {
        public RuleProfile()
        {
            CreateMap<Rule, RuleModel>().ReverseMap();
        }
    }
}

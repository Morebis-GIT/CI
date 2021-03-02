using System;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Autopilot.Rules;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class AutopilotRuleProfile : AutoMapper.Profile
    {
        public AutopilotRuleProfile()
        {
            CreateMap<AutopilotRule, AutopilotRuleModel>();

            CreateMap<Tuple<AutopilotRule, RuleModel, RuleTypeModel>, AutopilotRuleModel>()
                .ConstructUsing((tuple, context) => MapToAutopilotRuleModel(tuple.Item1, tuple.Item2, tuple.Item3, context.Mapper));
        }

        private static AutopilotRuleModel MapToAutopilotRuleModel(AutopilotRule autopilotRule, RuleModel rule, RuleTypeModel ruleType, IMapper mapper)
        {
            if (autopilotRule == null)
            {
                return null;
            }

            var autopilotRuleModel = mapper.Map<AutopilotRuleModel>(autopilotRule);
            autopilotRuleModel.Description = rule?.Description;
            autopilotRuleModel.CampaignType = rule?.CampaignType;
            autopilotRuleModel.RuleType = ruleType?.Name;

            return autopilotRuleModel;
        }
    }
}

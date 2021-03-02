using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Autopilot.Rules;
using ImagineCommunications.GamePlan.Domain.Autopilot.Settings;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class AutopilotSettingsProfile : AutoMapper.Profile
    {
        public AutopilotSettingsProfile()
        {
            CreateMap<AutopilotSettings, AutopilotSettingsModel>();

            CreateMap<Tuple<AutopilotSettings, List<AutopilotRule>, List<RuleModel>, List<RuleTypeModel>>, AutopilotSettingsModel>()
                .ConstructUsing((source, context) => MapToAutopilotSettingsModel(source, context.Mapper));
        }

        private static AutopilotSettingsModel MapToAutopilotSettingsModel(
            Tuple<AutopilotSettings, List<AutopilotRule>, List<RuleModel>, List<RuleTypeModel>> source, IMapper mapper)
        {
            var (autopilotSettings, autopilotRules, rules, ruleTypes) = source;
            if (autopilotSettings == null)
            {
                return null;
            }

            var autopilotSettingsModel = mapper.Map<AutopilotSettingsModel>(autopilotSettings);
            var indexedRules = rules.ToDictionary(r => r.UniqueRuleKey);
            var indexedRuleTypes = ruleTypes.ToDictionary(rt => rt.Id);

            autopilotSettingsModel.AutopilotRules = new List<AutopilotRuleModel>();
            foreach (var autopilotRule in autopilotRules)
            {
                _ = indexedRules.TryGetValue(autopilotRule.UniqueRuleKey, out var rule);
                _ = indexedRuleTypes.TryGetValue(autopilotRule.RuleTypeId, out var ruleType);

                autopilotSettingsModel.AutopilotRules.Add(
                    mapper.Map<AutopilotRuleModel>(Tuple.Create(autopilotRule, rule, ruleType)));
            }

            return autopilotSettingsModel;
        }
    }
}

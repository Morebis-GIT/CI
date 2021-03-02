using System;
using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Rules;
using ImagineCommunications.GamePlan.Domain.BusinessRules.RuleTypes;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class RuleTypeProfile : AutoMapper.Profile
    {
        public RuleTypeProfile()
        {
            CreateMap<RuleType, RuleTypeModel>().ReverseMap();
            CreateMap<Tuple<RuleType, List<Rule>>, RuleTypeModel>()
                .ConstructUsing((tuple, context) => MapToRuleTypeModel(tuple.Item1, tuple.Item2, context.Mapper));
        }

        private static RuleTypeModel MapToRuleTypeModel(RuleType ruleType, IList<Rule> rules, IMapper mapper)
        {
            if (ruleType == null)
            {
                return null;
            }

            var ruleTypeModel = mapper.Map<RuleTypeModel>(ruleType);

            if (rules != null)
            {
                ruleTypeModel.Rules = mapper.Map<List<RuleModel>>(rules);
            }

            return ruleTypeModel;
        }
    }
}

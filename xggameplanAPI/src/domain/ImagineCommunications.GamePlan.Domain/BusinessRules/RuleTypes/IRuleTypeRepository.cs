using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.BusinessRules.RuleTypes
{
    public interface IRuleTypeRepository
    {
        void Add(RuleType ruleType);

        void Delete(int id);

        RuleType Get(int id);

        IEnumerable<RuleType> GetAll(bool onlyAllowedAutopilot = false);

        void SaveChanges();

        void Update(RuleType ruleType);

        void Update(IEnumerable<RuleType> ruleTypes);
    }
}

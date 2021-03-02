using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.BusinessRules.Rules
{
    public interface IRuleRepository
    {
        void Add(Rule rule);

        void Delete(int id);

        IEnumerable<Rule> FindByRuleTypeId(int ruleTypeId);

        IEnumerable<Rule> FindByRuleTypeIds(IEnumerable<int> ruleTypeIds);

        Rule Get(int id);

        IEnumerable<Rule> GetAll();

        void SaveChanges();

        void Update(Rule rule);

        void Update(IEnumerable<Rule> rules);
    }
}

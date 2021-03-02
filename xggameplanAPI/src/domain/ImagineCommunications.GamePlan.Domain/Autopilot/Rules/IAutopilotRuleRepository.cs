using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Autopilot.Rules
{
    public interface IAutopilotRuleRepository
    {
        void Add(AutopilotRule autopilotRule);

        void Delete(int id);

        void Delete(IEnumerable<int> ids);

        AutopilotRule Get(int id);

        IEnumerable<AutopilotRule> GetAll();

        IEnumerable<AutopilotRule> GetByFlexibilityLevelId(int id);

        void SaveChanges();

        void Update(AutopilotRule autopilotRule);
    }
}

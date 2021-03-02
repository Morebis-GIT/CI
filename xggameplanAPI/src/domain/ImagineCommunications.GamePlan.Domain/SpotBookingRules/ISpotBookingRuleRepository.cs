using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.SpotBookingRules
{
    public interface ISpotBookingRuleRepository
    {
        IEnumerable<SpotBookingRule> GetAll();
        void AddRange(IEnumerable<SpotBookingRule> spotBookingRules);
        void SaveChanges();
        void Truncate();
        SpotBookingRule Get(int id);
    }
}

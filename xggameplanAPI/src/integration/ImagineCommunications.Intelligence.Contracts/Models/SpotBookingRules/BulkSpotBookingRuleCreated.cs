using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.SpotBookingRules;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.SpotBookingRules
{
    public class BulkSpotBookingRuleCreated : IBulkSpotBookingRuleCreated
    {
        public IEnumerable<ISpotBookingRuleCreated> Data { get; }

        public BulkSpotBookingRuleCreated(IEnumerable<SpotBookingRuleCreated> data) => Data = data;
    }
}

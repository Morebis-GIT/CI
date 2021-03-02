using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Persistence.SqlServer.BusinessLogic.BreakAvailabilityCalculator.Objects;
using xggameplan.core.RunManagement.BreakAvailabilityCalculator;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.BusinessLogic.BreakAvailabilityCalculator
{
    public class BreakAvailabilityUpdateHandler : IBreakAvailabilityUpdateHandler<BreakAvailability>
    {
        public IDictionary<Guid, IBreakAvailability> UpdatedBreaks { get; } = new Dictionary<Guid, IBreakAvailability>();

        protected void AddToUpdatedBreaks(IBreakAvailability theBreak)
        {
            if (!UpdatedBreaks.ContainsKey(theBreak.Id))
            {
                UpdatedBreaks.Add(theBreak.Id, theBreak);
            }
        }

        public void UpdateAvailability(BreakAvailability theBreak)
        {
            AddToUpdatedBreaks(theBreak);
        }

        public void UpdateOptimizerAvailability(BreakAvailability theBreak)
        {
            AddToUpdatedBreaks(theBreak);
        }
    }
}

using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Spots;

namespace ImagineCommunications.GamePlan.Process.Smooth.Interfaces
{
    public interface ICampaignClashChecker
    {
        List<Spot> GetCampaignClashesForNewSpots(
            IEnumerable<Spot> spots,
            IReadOnlyCollection<Spot> breakSpots);
    }
}

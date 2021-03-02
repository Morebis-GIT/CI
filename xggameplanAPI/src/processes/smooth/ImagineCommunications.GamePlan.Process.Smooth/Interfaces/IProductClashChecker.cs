using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Types;

namespace ImagineCommunications.GamePlan.Process.Smooth.Interfaces
{
    public interface IProductClashChecker
    {
        IReadOnlyCollection<Spot> GetProductClashesForSingleSpot(
            Spot spot,
            IReadOnlyCollection<Spot> spotsUsed,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            ClashCodeLevel clashLevel);

        IReadOnlyCollection<Spot> GetProductClashesForMultipleSpots(
            IReadOnlyCollection<Spot> spots,
            IReadOnlyCollection<Spot> breakSpots,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            ClashCodeLevel clashLevel);
    }
}

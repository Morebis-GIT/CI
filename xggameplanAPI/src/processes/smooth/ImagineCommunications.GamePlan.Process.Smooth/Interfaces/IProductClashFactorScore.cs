using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Types;

namespace ImagineCommunications.GamePlan.Process.Smooth.Interfaces
{
    public interface IProductClashFactorScore
    {
        double GetFactorScoreForProductClashes(
            IReadOnlyCollection<Spot> spotsInTheBreak,
            IReadOnlyCollection<Spot> spotsToPlace,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            IReadOnlyDictionary<string, Product> productsByExternalRef,
            IReadOnlyDictionary<string, Clash> clashesByExternalRef,
            Func<Spot, IReadOnlyCollection<Spot>, IReadOnlyDictionary<Guid, SpotInfo>, ClashCodeLevel, IReadOnlyCollection<Spot>> productClashChecker);
    }
}

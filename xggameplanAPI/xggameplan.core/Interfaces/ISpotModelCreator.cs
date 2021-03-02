using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Spots;
using xggameplan.Model;

namespace xggameplan.core.Interfaces
{
    public interface ISpotModelCreator
    {
        IEnumerable<SpotModel> Create(IReadOnlyCollection<Spot> spots);
    }
}

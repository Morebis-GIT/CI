using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Dto.Internal
{
    internal class FunctionalAreaDto : FunctionalArea
    {
        internal Dictionary<int, bool> FaultTypeSelections = new Dictionary<int, bool>();
    }
}

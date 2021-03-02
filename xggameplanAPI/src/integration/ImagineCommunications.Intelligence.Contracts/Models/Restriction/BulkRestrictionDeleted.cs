using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Restriction;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Restriction
{
    public class BulkRestrictionDeleted : IBulkRestrictionDeleted
    {
        public IEnumerable<IRestrictionDeleted> Data { get; }

        public BulkRestrictionDeleted(IEnumerable<RestrictionDeleted> data) => Data = data;
    }
}

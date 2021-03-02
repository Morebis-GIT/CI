using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Restriction;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Restriction
{
    public class BulkRestrictionCreatedOrUpdated : IBulkRestrictionCreatedOrUpdated
    {
        public BulkRestrictionCreatedOrUpdated(IEnumerable<RestrictionCreatedOrUpdated> data) => Data = data;

        public IEnumerable<IRestrictionCreatedOrUpdated> Data { get; }
    }
}

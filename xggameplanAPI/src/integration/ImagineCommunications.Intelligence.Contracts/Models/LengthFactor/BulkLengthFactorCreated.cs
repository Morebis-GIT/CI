using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.LengthFactor;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.LengthFactor
{
    public class BulkLengthFactorCreated : IBulkLengthFactorCreated
    {
        public IEnumerable<ILengthFactorCreated> Data { get; }

        public BulkLengthFactorCreated(IEnumerable<LengthFactorCreated> data) => Data = data;
    }
}

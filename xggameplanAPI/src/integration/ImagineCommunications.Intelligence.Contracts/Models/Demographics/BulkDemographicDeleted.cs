using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Demographics;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Demographics
{
    public class BulkDemographicDeleted : IBulkDemographicDeleted
    {
        public IEnumerable<IDemographicDeleted> Data { get; }

        public BulkDemographicDeleted(IEnumerable<DemographicDeleted> data) => Data = data;
    }
}

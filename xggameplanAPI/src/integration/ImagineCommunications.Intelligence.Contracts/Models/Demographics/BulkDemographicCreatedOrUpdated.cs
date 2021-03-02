using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Demographics;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Demographics
{
    public class BulkDemographicCreatedOrUpdated : IBulkDemographicCreatedOrUpdated
    {
        public IEnumerable<IDemographicCreatedOrUpdated> Data { get; }
        
        public BulkDemographicCreatedOrUpdated(IEnumerable<DemographicCreatedOrUpdated> data) => Data = data;
    }
}

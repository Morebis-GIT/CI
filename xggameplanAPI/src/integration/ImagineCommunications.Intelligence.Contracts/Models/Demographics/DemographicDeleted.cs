using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Demographics;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Demographics
{
    public class DemographicDeleted : IDemographicDeleted
    {
        public string ExternalRef { get; }

        public DemographicDeleted(string externalRef)
        {
            ExternalRef = externalRef;
        }
    }
}

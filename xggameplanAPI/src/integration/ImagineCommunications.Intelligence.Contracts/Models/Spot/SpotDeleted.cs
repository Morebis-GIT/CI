using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Spot;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Spot
{
    public class SpotDeleted : ISpotDeleted
    {
        public SpotDeleted(string externalSpotRef) => ExternalSpotRef = externalSpotRef;

        public string ExternalSpotRef { get; }
    }
}

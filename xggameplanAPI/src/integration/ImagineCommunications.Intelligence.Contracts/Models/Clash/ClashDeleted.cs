using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Clash;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Clash
{
    public class ClashDeleted : IClashDeleted
    {
        public ClashDeleted(string externalRef) => Externalref = externalRef;

        public string Externalref { get; }
    }
}

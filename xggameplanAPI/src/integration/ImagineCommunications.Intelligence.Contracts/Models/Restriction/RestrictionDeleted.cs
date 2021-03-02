using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Restriction;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Restriction
{
    public class RestrictionDeleted : IRestrictionDeleted
    {
        public RestrictionDeleted(string externalReference) => ExternalReference = externalReference;

        public string ExternalReference { get; }
    }
}

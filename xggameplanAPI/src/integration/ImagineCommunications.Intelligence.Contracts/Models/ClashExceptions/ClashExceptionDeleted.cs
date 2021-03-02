using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ClashExceptions;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.ClashExceptions
{
    public class ClashExceptionDeleted : IClashExceptionDeleted
    {
        public ClashExceptionDeleted(string externalRef) => ExternalRef = externalRef;

        public string ExternalRef { get; }
    }
}

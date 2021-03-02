using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ClashExceptions
{
    public interface IClashExceptionDeleted : IEvent
    {
        string ExternalRef { get; }
    }
}

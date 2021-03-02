using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Spot
{
    public interface ISpotDeleted : IEvent
    {
        string ExternalSpotRef { get; }
    }
}

using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Demographics
{
    public interface IDemographicDeleted : IEvent
    {
        string ExternalRef { get; }
    }
}

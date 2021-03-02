using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Demographics
{
    public interface IDemographicUpdated : IEvent
    {
        string ExternalRef { get; }
        string Name { get; }
        string ShortName { get; }
        int DisplayOrder { get; }
        bool Gameplan { get; }
    }
}

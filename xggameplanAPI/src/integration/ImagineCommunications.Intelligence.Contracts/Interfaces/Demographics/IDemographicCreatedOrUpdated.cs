using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Demographics
{
    public interface IDemographicCreatedOrUpdated : IEvent
    {
        int CustomId { get; }
        string ExternalRef { get; }
        string Name { get; }
        string ShortName { get; }
        int DisplayOrder { get; }
        bool Gameplan { get; }
    }
}

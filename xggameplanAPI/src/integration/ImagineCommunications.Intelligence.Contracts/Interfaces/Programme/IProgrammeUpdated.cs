using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Programme
{
    public interface IProgrammeUpdated : IEvent
    {
        string ExternalReference { get; }
        string ProgrammeName { get; }
        string Description { get; }
        string Classification { get; }
        bool LiveBroadcast { get; }
    }
}

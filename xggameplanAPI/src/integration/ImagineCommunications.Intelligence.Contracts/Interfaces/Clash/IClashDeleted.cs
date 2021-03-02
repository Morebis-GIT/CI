using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Clash
{
    public interface IClashDeleted : IEvent
    {
        string Externalref { get; }
    }
}

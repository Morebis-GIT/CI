using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.BreakTypes
{
    public interface IBreakTypeCreated : IEvent
    {
        string Name { get; }
    }
}

using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Restriction
{
    public interface IRestrictionDeleted : IEvent
    {
        string ExternalReference { get; }
    }
}

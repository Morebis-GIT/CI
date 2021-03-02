using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Product
{
    public interface IProductDeleted: IEvent
    {
        string Externalidentifier { get; }
    }
}

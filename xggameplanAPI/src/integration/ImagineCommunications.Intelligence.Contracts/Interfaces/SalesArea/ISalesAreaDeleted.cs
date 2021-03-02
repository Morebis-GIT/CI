using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.SalesArea
{
    public interface ISalesAreaDeleted: IEvent
    {
        string ShortName { get; }
    }
}

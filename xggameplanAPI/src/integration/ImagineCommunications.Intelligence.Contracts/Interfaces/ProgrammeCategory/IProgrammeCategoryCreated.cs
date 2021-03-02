using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ProgrammeCategory
{
    public interface IProgrammeCategoryCreated : IEvent
    {
        string Name { get; }
        string ExternalRef { get; }
        string ParentExternalRef { get; }
    }
}

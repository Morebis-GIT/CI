using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ProgrammeClassification
{
    public interface IProgrammeClassificationCreated : IEvent
    {
        int Uid { get; }

        string Code { get; }

        string Description { get; }
    }
}

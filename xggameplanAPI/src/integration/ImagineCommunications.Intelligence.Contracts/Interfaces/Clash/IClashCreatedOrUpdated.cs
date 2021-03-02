using System.Collections.Generic;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Clash;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Clash
{
    public interface IClashCreatedOrUpdated : IEvent
    {
        string ParentExternalidentifier { get; }

        string Description { get; }

        int ExposureCount { get; }

        string Externalref { get; }

        List<ClashDifference> Differences { get; }
    }
}

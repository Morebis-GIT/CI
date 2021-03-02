using System;
using System.Collections.Generic;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Programme
{
    public interface IProgrammeCreated : IEvent
    {
        string SalesArea { get; }

        string ExternalReference { get; }

        string ProgrammeName { get; }

        string Description { get; }

        string Classification { get; }

        bool LiveBroadcast { get; }

        DateTime StartDateTime { get; }

        TimeSpan Duration { get; }

        IEnumerable<string> ProgrammeCategories { get; }

        ProgrammeEpisode Episode { get; }
    }
}

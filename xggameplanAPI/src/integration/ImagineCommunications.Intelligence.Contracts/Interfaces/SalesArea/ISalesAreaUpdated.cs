using System;
using System.Collections.Generic;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.SalesArea;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.SalesArea
{
    public interface ISalesAreaUpdated : IEvent
    {
        int CustomId { get; }

        string Name { get; }

        string ShortName { get; }

        string CurrencyCode { get; }

        string BaseDemographic1 { get; }

        string BaseDemographic2 { get; }

        string TargetAreaName { get; }

        List<string> ChannelGroup { get; }

        TimeSpan StartOffset { get; }

        TimeSpan DayDuration { get; }

        List<SalesAreaDemographic> Demographics { get; }
    }
}

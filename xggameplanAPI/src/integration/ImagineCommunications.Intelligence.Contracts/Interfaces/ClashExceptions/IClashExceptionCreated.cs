using System;
using System.Collections.Generic;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.SharedModels;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ClashExceptions
{
    public interface IClashExceptionCreated : IEvent
    {
        DateTime StartDate { get; }

        DateTime? EndDate { get; }

        ClashExceptionType FromType { get; }

        ClashExceptionType ToType { get; }

        IncludeOrExclude IncludeOrExclude { get; }

        string FromValue { get; }

        string ToValue { get; }

        List<TimeAndDow> TimeAndDows { get; }

        string ExternalRef { get; }
    }
}

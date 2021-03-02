using System;
using System.Collections.Generic;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Shared.Enums;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Restriction
{
    public interface IRestrictionCreatedOrUpdated : IEvent
    {
        string ExternalIdentifier { get; }

        List<string> SalesAreas { get; }

        DateTime StartDate { get; }

        DateTime? EndDate { get; }

        TimeSpan? StartTime { get; }

        TimeSpan? EndTime { get; }

        string RestrictionDays { get; }

        IncludeOrExcludeOrEither SchoolHolidayIndicator { get; }

        IncludeOrExcludeOrEither PublicHolidayIndicator { get; }

        IncludeOrExclude? LiveProgrammeIndicator { get; }

        RestrictionType? RestrictionType { get; }

        RestrictionBasis? RestrictionBasis { get; }

        string ExternalProgRef { get; }

        string ProgrammeCategory { get; }

        string ProgrammeClassification { get; }

        IncludeOrExclude? ProgrammeClassificationIndicator { get; }

        int TimeToleranceMinsBefore { get; }

        int TimeToleranceMinsAfter { get; }

        int? IndexType { get; }

        int? IndexThreshold { get; }

        int? ProductCode { get; }

        string ClashCode { get; }

        string ClearanceCode { get; }

        string ClockNumber { get; }

        int? EpisodeNo { get; }
    }
}

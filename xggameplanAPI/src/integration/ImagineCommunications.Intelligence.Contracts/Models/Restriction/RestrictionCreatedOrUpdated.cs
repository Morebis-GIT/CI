using System;
using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Restriction;
using ImagineCommunications.Gameplan.Integration.Contracts.Shared.Enums;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Restriction
{
    public class RestrictionCreatedOrUpdated : IRestrictionCreatedOrUpdated
    {
        public RestrictionCreatedOrUpdated(string externalIdentifier, List<string> salesAreas,
            DateTime startDate, DateTime? endDate, TimeSpan? startTime, TimeSpan? endTime, string restrictionDays,
            IncludeOrExcludeOrEither schoolHolidayIndicator, IncludeOrExcludeOrEither publicHolidayIndicator,
            IncludeOrExclude? liveProgrammeIndicator, RestrictionType? restrictionType, RestrictionBasis? restrictionBasis,
            string externalProgRef, string programmeCategory, string programmeClassification, IncludeOrExclude? programmeClassificationIndicator,
            int timeToleranceMinsBefore, int timeToleranceMinsAfter, int? indexType, int? indexThreshold,
            int? productCode, string clashCode, string clearanceCode, string clockNumber, int? episodeNo)
        {
            ExternalIdentifier = externalIdentifier;
            SalesAreas = salesAreas;
            StartDate = startDate;
            EndDate = endDate;
            StartTime = startTime;
            EndTime = endTime;
            RestrictionDays = restrictionDays;
            SchoolHolidayIndicator = schoolHolidayIndicator;
            PublicHolidayIndicator = publicHolidayIndicator;
            LiveProgrammeIndicator = liveProgrammeIndicator;
            RestrictionType = restrictionType;
            RestrictionBasis = restrictionBasis;
            ExternalProgRef = externalProgRef;
            ProgrammeCategory = programmeCategory;
            ProgrammeClassification = programmeClassification;
            ProgrammeClassificationIndicator = programmeClassificationIndicator;
            TimeToleranceMinsBefore = timeToleranceMinsBefore;
            TimeToleranceMinsAfter = timeToleranceMinsAfter;
            IndexType = indexType;
            IndexThreshold = indexThreshold;
            ProductCode = productCode;
            ClashCode = clashCode;
            ClearanceCode = clearanceCode;
            ClockNumber = string.IsNullOrWhiteSpace(clockNumber) ? "0" : clockNumber;
            EpisodeNo = episodeNo;
        }

        public string ExternalIdentifier { get; }

        public List<string> SalesAreas { get; }

        public DateTime StartDate { get; }

        public DateTime? EndDate { get; }

        public TimeSpan? StartTime { get; }

        public TimeSpan? EndTime { get; }

        public string RestrictionDays { get; }

        public IncludeOrExcludeOrEither SchoolHolidayIndicator { get; }

        public IncludeOrExcludeOrEither PublicHolidayIndicator { get; }

        public IncludeOrExclude? LiveProgrammeIndicator { get; }

        public RestrictionType? RestrictionType { get; }

        public RestrictionBasis? RestrictionBasis { get; }

        public string ExternalProgRef { get; }

        public string ProgrammeCategory { get; }

        public string ProgrammeClassification { get; }

        public IncludeOrExclude? ProgrammeClassificationIndicator { get; }

        public int TimeToleranceMinsBefore { get; }

        public int TimeToleranceMinsAfter { get; }

        public int? IndexType { get; }

        public int? IndexThreshold { get; }

        public int? ProductCode { get; }

        public string ClashCode { get; }

        public string ClearanceCode { get; }

        public string ClockNumber { get; }

        public int? EpisodeNo { get; }
    }
}

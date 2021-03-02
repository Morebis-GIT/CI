using System;
using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ClashExceptions;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.SharedModels;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.ClashExceptions
{
    public class ClashExceptionCreated : IClashExceptionCreated
    {
        public ClashExceptionCreated(DateTime startDate, DateTime? endDate, ClashExceptionType fromType, ClashExceptionType toType,
            IncludeOrExclude includeOrExclude, string fromValue, string toValue, List<TimeAndDow> timeAndDows, string externalRef)
        {
            StartDate = startDate;
            EndDate = endDate;
            FromType = fromType;
            ToType = toType;
            IncludeOrExclude = includeOrExclude;
            FromValue = fromValue;
            ToValue = toValue;
            TimeAndDows = timeAndDows;
            ExternalRef = externalRef;
        }

        public DateTime StartDate { get; }

        public DateTime? EndDate { get; }

        public ClashExceptionType FromType { get; }

        public ClashExceptionType ToType { get; }

        public IncludeOrExclude IncludeOrExclude { get; }

        public string FromValue { get; }

        public string ToValue { get; }

        public List<TimeAndDow> TimeAndDows { get; }

        public string ExternalRef { get; }
    }
}

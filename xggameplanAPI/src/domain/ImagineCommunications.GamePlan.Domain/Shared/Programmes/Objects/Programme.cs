using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.SequenceCounter;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Counters;
using NodaTime;
using Raven.Imports.Newtonsoft.Json;

namespace ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects
{
    public class Programme : IProgrammePrgtNoCounter
    {
        private DateTime _startDateTime;
        private string _salesArea;
        private int? _uniqueKey;

        public Guid Id { get; set; }

        /// <summary>
        /// Unique number - program event number
        /// </summary>
        public int PrgtNo { get; set; }

        public string SalesArea
        {
            get => _salesArea;
            set
            {
                if (_salesArea != value)
                {
                    _salesArea = value;
                    _uniqueKey = null;
                }
            }
        }

        public DateTime StartDateTime
        {
            get => _startDateTime;
            set
            {
                if (_startDateTime != value)
                {
                    _startDateTime = value;
                    _uniqueKey = null;
                }
            }
        }

        public Duration Duration { get; set; }

        public string ExternalReference { get; set; }

        public string ProgrammeName { get; set; }

        public string Description { get; set; }

        public List<string> ProgrammeCategories { get; set; }

        public ProgrammeEpisode Episode { get; set; }

        public string Classification { get; set; }

        public bool LiveBroadcast { get; set; }

        public Programme Clone()
        {
            return (Programme)MemberwiseClone();
        }

        [JsonIgnore]
        int ICounterValue.Value
        {
            get => PrgtNo;
            set => PrgtNo = value;
        }

        [JsonIgnore]
        public int ScheduleUniqueKey =>
            (_uniqueKey ?? (_uniqueKey = HashCode.Combine(SalesArea.ToLowerInvariant(), StartDateTime.Date)))
            .Value;
    }
}

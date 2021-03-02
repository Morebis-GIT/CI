using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.SequenceCounter;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Counters;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Schedules;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes
{
    public class Programme : IUniqueIdentifierPrimaryKey, IProgrammePrgtNoCounter
    {
        private DateTime _startDateTime;
        private Guid _salesAreaId;
        private int? _uniqueKey;

        public Guid Id { get; set; }

        /// <summary>
        /// Unique number - program event number
        /// </summary>
        public int PrgtNo { get; set; }
        public Guid SalesAreaId
        {
            get => _salesAreaId;
            set
            {
                if (_salesAreaId != value)
                {
                    _salesAreaId = value;
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
        public TimeSpan Duration { get; set; }
        public bool LiveBroadcast { get; set; }
        public int ProgrammeDictionaryId { get; set; }
        public int? EpisodeId { get; set; }

        public ICollection<ProgrammeCategoryLink> ProgrammeCategoryLinks { get; set; } =
            new HashSet<ProgrammeCategoryLink>();
        public ProgrammeDictionary ProgrammeDictionary { get; set; }
        public ProgrammeEpisode Episode { get; set; }
        public ScheduleProgramme ScheduleProgramme { get; set; }
        public SalesArea SalesArea { get; set; }

        int ICounterValue.Value
        {
            get => PrgtNo;
            set => PrgtNo = value;
        }

        public int ScheduleUniqueKey =>
            (_uniqueKey ?? (_uniqueKey = HashCode.Combine(SalesAreaId, StartDateTime.Date)))
            .Value;
    }
}

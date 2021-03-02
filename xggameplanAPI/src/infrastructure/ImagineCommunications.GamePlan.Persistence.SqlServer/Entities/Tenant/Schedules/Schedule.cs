using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Schedules
{
    public class Schedule : IIdentityPrimaryKey, IScheduleIndex
    {
        private DateTime _date;
        private Guid _salesAreaId;
        private int? _uniqueKey;

        public int Id { get; set; }

        public DateTime Date
        {
            get => _date;
            set
            {
                if (_date != value)
                {
                    _date = value;
                    _uniqueKey = null;
                }
            }
        }

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

        public SalesArea SalesArea { get; set; }
        public ICollection<ScheduleBreak> Breaks { get; set; } = new HashSet<ScheduleBreak>();
        public ICollection<ScheduleProgramme> Programmes { get; set; } = new HashSet<ScheduleProgramme>();

        public int ScheduleUniqueKey => (_uniqueKey ?? (_uniqueKey = HashCode.Combine(SalesAreaId, Date.Date)))
            .Value;
    }
}

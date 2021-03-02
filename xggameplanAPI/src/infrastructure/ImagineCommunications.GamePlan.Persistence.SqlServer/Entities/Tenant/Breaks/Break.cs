using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Breaks
{
    public class Break : IUniqueIdentifierPrimaryKey
    {
        public Guid Id { get; set; }
        public int CustomId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime? BroadcastDate { get; set; }
        public int? ClockHour { get; set; }
        public Guid SalesAreaId { get; set; }
        public string BreakType { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan ReserveDuration { get; set; }
        public TimeSpan Avail { get; set; }
        public TimeSpan OptimizerAvail { get; set; }
        public bool Optimize { get; set; }
        [Required]
        public string ExternalBreakRef { get; set; }
        public string Description { get; set; }
        public string ExternalProgRef { get; set; }
        public int PositionInProg { get; set; }
        public double BreakPrice { get; set; }
        public double FloorRate { get; set; }
        public bool Solus { get; set; }
        public string PremiumCategory { get; set; }
        public bool AllowSplit { get; set; }
        public bool NationalRegionalSplit { get; set; }
        public bool ExcludePackages { get; set; }
        public bool BonusAllowed { get; set; }
        public bool LongForm { get; set; }

        public SalesArea SalesArea { get; set; }
        public ICollection<BreakEfficiency> BreakEfficiencies { get; set; } = new HashSet<BreakEfficiency>();
    }
}

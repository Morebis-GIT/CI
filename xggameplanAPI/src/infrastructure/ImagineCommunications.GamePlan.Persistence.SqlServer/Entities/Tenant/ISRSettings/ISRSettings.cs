using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ISRSettings
{
    public class ISRSettings : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid SalesAreaId { get; set; }
        public int DefaultEfficiencyThreshold { get; set; }
        public string BreakType { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public bool ExcludePublicHolidays { get; set; } // Dates from SalesArea.PublicHolidays
        public bool ExcludeSchoolHolidays { get; set; }	// Dates from SalesArea.SchoolHolidays

        public SortedSet<DayOfWeek> SelectableDays { get; set; } = new SortedSet<DayOfWeek>();

        public SalesArea SalesArea { get; set; }
        public List<ISRDemographicSettings> DemographicsSettings { get; set; } = new List<ISRDemographicSettings>();
    }
}

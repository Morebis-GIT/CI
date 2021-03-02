using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas
{
    public class SalesArea : IUniqueIdentifierPrimaryKey
    {
        public Guid Id { get; set; }
        public int CustomId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string CurrencyCode { get; set; }
        public string BaseDemographic1 { get; set; }
        public string BaseDemographic2 { get; set; }
        public string TargetAreaName { get; set; }
        public int UniverseId { get; set; }
        public TimeSpan StartOffset { get; set; }
        public TimeSpan DayDuration { get; set; }

        public List<SalesAreasChannelGroup> ChannelGroups { get; set; } = new List<SalesAreasChannelGroup>();
        public List<SalesAreasHoliday> Holidays { get; set; } = new List<SalesAreasHoliday>();
    }
}

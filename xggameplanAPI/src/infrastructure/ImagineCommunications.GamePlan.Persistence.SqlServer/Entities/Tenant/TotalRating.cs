using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class TotalRating : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public string SalesArea { get; set; }
        public string Demograph { get; set; }
        public DateTime Date { get; set; }
        public int DaypartGroup { get; set; }
        public int Daypart { get; set; }
        public double TotalRatings { get; set; }
    }
}

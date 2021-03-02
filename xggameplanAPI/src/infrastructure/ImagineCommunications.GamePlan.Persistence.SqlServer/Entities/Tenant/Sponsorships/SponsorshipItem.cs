using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Sponsorships
{
    public class SponsorshipItem : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int SponsoredItemId { get; set; }
        
        public ICollection<SponsorshipItemSalesArea> SalesAreas { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ProgrammeName { get; set; }
        public ICollection<SponsoredDayPart> DayParts { get; set; }
    }
}

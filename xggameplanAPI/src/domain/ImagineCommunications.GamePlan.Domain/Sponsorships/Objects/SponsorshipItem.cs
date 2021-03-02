using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Sponsorships.Objects
{
    public class SponsorshipItem
    {
        public IEnumerable<string> SalesAreas { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<SponsoredDayPart> DayParts { get; set; }
        public string ProgrammeName { get; set; }
    }
}

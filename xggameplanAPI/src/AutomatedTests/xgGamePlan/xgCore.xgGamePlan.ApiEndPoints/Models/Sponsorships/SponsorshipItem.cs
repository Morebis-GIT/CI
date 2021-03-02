﻿using System;
using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Sponsorships
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

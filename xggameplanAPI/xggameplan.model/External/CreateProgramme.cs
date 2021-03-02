using System;
using System.Collections.Generic;
using NodaTime;

namespace xggameplan.Model
{
    public class CreateProgramme
    {
        public string SalesArea { get; set; }
        public DateTime StartDateTime { get; set; }
        public Duration Duration { get; set; }
        public string ExternalReference { get; set; }
        public string ProgrammeName { get; set; }
        public string Description { get; set; }
        public List<string> ProgrammeCategories { get; set; }
        public string Classification { get; set; }
        public bool LiveBroadcast { get; set; }
    }
}

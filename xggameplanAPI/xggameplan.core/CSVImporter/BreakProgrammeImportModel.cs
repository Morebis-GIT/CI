using System;

namespace xggameplan.CSVImporter
{
    public class BreakProgrammeImportModel
    {

        public string WeekNo { get; set; }
        public string SalesArea { get; set; }
        public string Programme { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan OpenAvailability { get; set; }
        public string PositionInProgramme { get; set; }




    }
}

using CsvHelper.Configuration;

namespace xggameplan.CSVImporter
{
    public sealed class BreakMap : ClassMap<BreakProgrammeImportModel>
    {
        public BreakMap()
        {
            Map(m => m.WeekNo).Index(0);
            Map(m => m.SalesArea).Index(1);
            Map(m => m.Programme).Index(7);
            Map(m => m.StartDate).Index(4);
            Map(m => m.StartTime).Index(5);
            Map(m => m.Duration).Index(8);
            Map(m => m.OpenAvailability).Index(9);
            Map(m => m.PositionInProgramme).Index(10);
            // 6 - Type
            // 8 Duration Time
            // 9 Open Availaiblity Time
            //10 Programme Position
        }
    }
}

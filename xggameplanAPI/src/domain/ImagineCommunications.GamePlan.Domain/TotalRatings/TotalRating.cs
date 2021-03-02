using System;

namespace ImagineCommunications.GamePlan.Domain.TotalRatings
{
    public class TotalRating
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

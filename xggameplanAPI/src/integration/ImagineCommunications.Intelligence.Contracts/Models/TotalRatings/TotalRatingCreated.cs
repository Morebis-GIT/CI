using System;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.TotalRatings;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.TotalRatings
{
    public class TotalRatingCreated : ITotalRatingCreated
    {
        public string SalesArea { get; }
        public string Demograph { get; }
        public DateTime Date { get; }
        public int DaypartGroup { get; }
        public int Daypart { get; }
        public double TotalRatings { get; }

        public TotalRatingCreated(string salesArea, string demograph, DateTime date, int daypartGroup, int daypart, double totalRatings)
        {
            SalesArea = salesArea;
            Demograph = demograph;
            Date = date;
            DaypartGroup = daypartGroup;
            Daypart = daypart;
            TotalRatings = totalRatings;
        }
    }
}

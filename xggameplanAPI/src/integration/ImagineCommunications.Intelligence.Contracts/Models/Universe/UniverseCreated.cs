using System;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Universe;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Universe
{
    public class UniverseCreated : IUniverseCreated
    {
        public UniverseCreated(string salesArea, string demographic, DateTime startDate, DateTime endDate, int universeValue)
        {
            SalesArea = salesArea;
            Demographic = demographic;

            StartDate = startDate.Date;
            EndDate = endDate != default ? endDate.Date : startDate.AddYears(2).Date;

            UniverseValue = universeValue;
        }

        public string SalesArea { get; }

        public string Demographic { get; }

        public DateTime StartDate { get; }

        public DateTime EndDate { get; }

        public int UniverseValue { get; }
    }
}

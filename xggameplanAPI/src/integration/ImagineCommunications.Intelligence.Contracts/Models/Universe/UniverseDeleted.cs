using System;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Universe;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Universe
{
    public class UniverseDeleted : IUniverseDeleted
    {
        public UniverseDeleted(DateTime? startDate, DateTime? endDate, string salesArea, string demographic)
        {
            StartDate = startDate;
            EndDate = endDate;
            SalesArea = salesArea;
            Demographic = demographic;
        }

        public DateTime? StartDate { get; }

        public DateTime? EndDate { get; }

        public string SalesArea { get; }

        public string Demographic { get; }
    }
}

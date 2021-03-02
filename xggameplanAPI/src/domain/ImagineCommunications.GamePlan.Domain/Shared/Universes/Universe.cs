using System;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using Raven.Client.UniqueConstraints;

namespace ImagineCommunications.GamePlan.Domain.Shared.Universes
{
    public class Universe
    {
        [UniqueConstraint]
        public Guid Id { get; set; }

        public string SalesArea { get; set; }

        public string Demographic { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int UniverseValue { get; set; }

        public static Universe Create(Guid id,
                                    string salesArea,
                                    string demographic,
                                    DateTime startDate,
                                    DateTime endDate,
                                    int universeValue)
        {
            var universe = new Universe()
            {
                SalesArea = salesArea,
                Demographic = demographic,
                StartDate = startDate,
                EndDate = endDate,
                UniverseValue = universeValue
            };
            return universe;
        }

        public bool IsDatesOverlap(Universe universe)
        {
            return DateHelper.IsRangesOverlap(StartDate, EndDate, universe.StartDate, universe.EndDate);
        }

        public void Update(Guid id,
                                    string salesArea,
                                    string demographic,
                                    DateTime startDate,
                                    DateTime endDate,
                                    int universeValue)
        {
            Id = id;
            SalesArea = salesArea;
            Demographic = demographic;
            StartDate = startDate;
            EndDate = endDate;
            UniverseValue = universeValue;
        }
    }


}

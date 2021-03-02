using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.Pivot
{
    public class PivotProgramme
    {
        public PivotProgramme(
            IReadOnlyDictionary<string, Programme> allProgrammes,
            string externalProgrammeReference,
            string[] productIds)
        {
            Breaks = new List<PivotBreak>();
            ProductIds = productIds;
            ExternalProgrammeReference = externalProgrammeReference;

            if (allProgrammes.TryGetValue(externalProgrammeReference, out Programme programme))
            {
                Start = programme.StartDateTime;
                End = programme.StartDateTime.Add(programme.Duration.ToTimeSpan());
            }
        }

        public string ExternalProgrammeReference { get; set; }
        public string[] ProductIds { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public List<PivotBreak> Breaks { get; set; }

        public string ProductIdsStr => String.Join(",", ProductIds);

        public override string ToString() => ExternalProgrammeReference;

        public string[] BreaksExternalReference => Breaks.Select(b => b.ExternalBreakNo).ToArray();

        public PivotBreak Break(string externalBreakNo) =>
            Breaks.Find(b => String.Equals(b.ExternalBreakNo, externalBreakNo));
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.Pivot
{
    public class PivotSalesArea
    {
        public PivotSalesArea(string salesAreaName, string[] productIds)
        {
            Name = salesAreaName;
            ProductIds = productIds;
            Programmes = new List<PivotProgramme>();
        }

        public string Name { get; set; }
        public string[] ProductIds { get; set; }
        public List<PivotProgramme> Programmes { get; set; }

        public string ProductIdsStr => String.Join(",", ProductIds);

        public string[] ProgrammesExternalReference =>
            Programmes.Select(p => p.ExternalProgrammeReference).ToArray();

        public PivotProgramme Programme(string externalProgrammeReference) =>
            Programmes.Find(p => String.Equals(p.ExternalProgrammeReference, externalProgrammeReference));

        public override string ToString() => Name;
    }
}

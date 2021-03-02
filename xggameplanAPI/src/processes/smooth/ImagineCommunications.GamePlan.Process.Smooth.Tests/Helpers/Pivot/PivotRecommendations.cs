using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.Pivot
{
    public class PivotRecommendations
    {
        public Dictionary<string, List<string>> BreaksVsProductIds { get; set; }
        public Dictionary<string, int> ProductIdCountInBreak { get; set; }

        public List<PivotSalesArea> SalesAreas { get; set; }

        public PivotRecommendations(
            IReadOnlyCollection<Recommendation> recommendations,
            IReadOnlyDictionary<string, Break> allBreaks,
            IReadOnlyDictionary<string, Programme> allProgrammes)
        {
            BreaksVsProductIds = new Dictionary<string, List<string>>();
            ProductIdCountInBreak = new Dictionary<string, int>();
            SalesAreas = new List<PivotSalesArea>();

            var salesAreas = recommendations
                .GroupBy(r => r.SalesArea)
                .Select(g => new
                {
                    SalesAreaName = g.Key,
                    ProductIds = GetProductIds(g)
                });

            foreach (var salesAreaItem in salesAreas)
            {
                var salesArea = new PivotSalesArea(
                    salesAreaItem.SalesAreaName,
                    salesAreaItem.ProductIds);

                var programmes = recommendations
                    .Where(r => r.SalesArea == salesAreaItem.SalesAreaName
                                && r.ExternalProgrammeReference != null)
                    .GroupBy(r => r.ExternalProgrammeReference)
                    .Select(g => new
                    {
                        ExternalProgrammeReference = g.Key,
                        ProductIds = GetProductIds(g)
                    });

                foreach (var programmeItem in programmes)
                {
                    var pivotProgram = new PivotProgramme(
                        allProgrammes,
                        programmeItem.ExternalProgrammeReference,
                        programmeItem.ProductIds);

                    var breaks = recommendations
                        .Where(r => r.SalesArea == salesAreaItem.SalesAreaName
                            && r.ExternalProgrammeReference == programmeItem.ExternalProgrammeReference)
                        .GroupBy(r => r.ExternalBreakNo)
                        .Select(g => new
                        {
                            ExternalBreakNo = g.Key,
                            ProductIds = GetProductIds(g)
                        });

                    foreach (var br in breaks)
                    {
                        var pivotBreak = new PivotBreak(
                            allBreaks,
                            br.ExternalBreakNo,
                            br.ProductIds);

                        if (!BreaksVsProductIds.ContainsKey(br.ExternalBreakNo))
                        {
                            {
                                BreaksVsProductIds.Add(br.ExternalBreakNo, new List<string>());
                            }

                            BreaksVsProductIds[br.ExternalBreakNo].AddRange(br.ProductIds);
                        }

                        var recommendationsForEachBreak = recommendations
                        .Where(r => r.ExternalBreakNo == br.ExternalBreakNo);

                        foreach (var recommendation in recommendationsForEachBreak)
                        {
                            var productIdCountInBreakKey = $"{br.ExternalBreakNo}_{recommendation.Product}";
                            if (ProductIdCountInBreak.ContainsKey(productIdCountInBreakKey))
                            {
                                ProductIdCountInBreak[productIdCountInBreakKey]++;
                            }
                            else
                            {
                                ProductIdCountInBreak.Add(productIdCountInBreakKey, 1);
                            }
                            var pivotSpot = new PivotSpot(recommendation);
                            pivotBreak.Spots.Add(pivotSpot);
                        }

                        pivotProgram.Breaks.Add(pivotBreak);
                    }

                    salesArea.Programmes.Add(pivotProgram);
                }

                SalesAreas.Add(salesArea);
            }
        }

        public PivotSalesArea SalesArea(string salesAreaName) =>
            SalesAreas.Find(s => string.Equals(s.Name, salesAreaName));

        public string[] SalesAreaByNames() =>
            SalesAreas.Select(s => s.Name).ToArray();

        private string[] GetProductIds(IGrouping<string, Recommendation> g) =>
            g.Select(x => x.Product).Distinct().OrderBy(i => i).ToArray();

        public int GetCountOfProductInABreak(string br, IEnumerable<string> productsIds)
        {
            int count = 0;

            foreach (var productExternalId in productsIds)
            {
                if (ProductIdCountInBreak.TryGetValue($"{br}_{productExternalId}", out int value))
                {
                    count += value;
                }
            }

            return count;
        }
    }
}

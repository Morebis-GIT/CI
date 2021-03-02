using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using NodaTime;

namespace ImagineCommunications.GamePlan.Process.Smooth.Models
{
    public class SmoothSponsorshipRunningTotals
    {
        // <clashCode or Advertiser Identifier, ProductId>
        public Dictionary<string, List<ProductExternalReference>> AllProductIdsInClashOrAdvertiserCode { get; }
            = new Dictionary<string, List<ProductExternalReference>>();

        // clashCode or Advertiser Identifier, restriction value
        public Dictionary<string, int> AllRestrictionValues { get; }
        = new Dictionary<string, int>();

        //productId, value (count or duration)
        public Dictionary<ProductExternalReference, double> AlreadyPlacedCompetitors { get; }
            = new Dictionary<ProductExternalReference, double>();

        //productId, count or duration
        public Dictionary<ProductExternalReference, double> SponsoredProducts { get; }
            = new Dictionary<ProductExternalReference, double>();

        public IEnumerable<ProductExternalReference> GetAllCompetitors()
        {
            var competitors = new List<ProductExternalReference>();
            foreach (var item in AllProductIdsInClashOrAdvertiserCode)
            {
                var productIds = item.Value;
                competitors.AddRange(productIds);
            }

            competitors.AddRange(AllRestrictionValues.Keys
                .Select(k => (ProductExternalReference)k));

            return competitors.Distinct();
        }

        public Dictionary<ProductExternalReference, int> GetRestrictionValuesForCompetitors()
        {
            var competitorsAndRestrictionValues = new Dictionary<ProductExternalReference, int>();
            foreach (var item in AllProductIdsInClashOrAdvertiserCode)
            {
                var exclusivityCode = item.Key;
                if (!AllRestrictionValues.ContainsKey(exclusivityCode))
                {
                    continue;
                }
                var productIds = item.Value;
                var restrictionValue = AllRestrictionValues[exclusivityCode];
                foreach (var productId in productIds)
                {
                    if (competitorsAndRestrictionValues.ContainsKey(productId))
                    {
                        var existingValue = competitorsAndRestrictionValues[productId];
                        if (restrictionValue < existingValue)
                        {
                            competitorsAndRestrictionValues[productId] = restrictionValue;
                        }
                    }
                    else
                    {
                        competitorsAndRestrictionValues.Add(productId, restrictionValue);
                    }
                }
            }

            foreach (var restriction in AllRestrictionValues)
            {
                competitorsAndRestrictionValues
                    .Add(
                    restriction.Key,
                    restriction.Value);
            }

            return competitorsAndRestrictionValues;
        }

        public void AddCompetitorToClashCode(
            ProductExternalReference productExternalRefrence,
            string clashCode)
        {
            AddCompetitorToExclusivityCode(
                AllProductIdsInClashOrAdvertiserCode,
                productExternalRefrence,
                clashCode);
        }

        public void AddCompetitorToAdvertiserIdentifier(
           ProductExternalReference productExternalRefrence,
           string clashCode)
        {
            AddCompetitorToExclusivityCode(
                AllProductIdsInClashOrAdvertiserCode,
                productExternalRefrence,
                clashCode);
        }

        public void SetRestrictionValueForClashCode(
           string clashCode,
           int value)
        {
            SetRestrictionValueForExclusivityCode(
                AllRestrictionValues,
                clashCode,
                value);
        }

        public void SetRestrictionValueForAdvertiserIdentifier(
           string advertiserIdentifier,
           int value)
        {
            SetRestrictionValueForExclusivityCode(
                AllRestrictionValues,
                advertiserIdentifier,
                value);
        }

        public void AddCompetitorToSpotByCount(
            ProductExternalReference productExternalRefrence,
            int count)
        {
            AddProductToSpot(
                AlreadyPlacedCompetitors,
                productExternalRefrence,
                count
                );
        }

        public void AddCompetitorToSpotByDuration(
            ProductExternalReference productExternalRefrence,
            Duration duration)
        {
            AddProductToSpot(
                AlreadyPlacedCompetitors,
                productExternalRefrence,
                duration.TotalSeconds
                );
        }

        public void AddSponsoredProductToSpotByCount(
            ProductExternalReference productExternalRefrence,
            int count)
        {
            AddProductToSpot(
                SponsoredProducts,
                productExternalRefrence,
                count
                );
        }

        public void AddSponsoredProductToSpotByDuration(
            ProductExternalReference productExternalRefrence,
            Duration duration)
        {
            AddProductToSpot(
                SponsoredProducts,
                productExternalRefrence,
                duration.TotalSeconds
                );
        }

        public void RemoveCompetitorToSpotByCount(
            ProductExternalReference productExternalRefrence,
            int count)
        {
            RemoveProductToSpot(
                AlreadyPlacedCompetitors,
                productExternalRefrence,
                count
                );
        }

        public void RemoveCompetitorToSpotByDuration(
            ProductExternalReference productExternalRefrence,
            Duration duration)
        {
            RemoveProductToSpot(
                AlreadyPlacedCompetitors,
                productExternalRefrence,
                duration.TotalSeconds
                );
        }

        public void RemoveSponsoredProductToSpotByCount(
            ProductExternalReference productExternalRefrence,
            int count)
        {
            RemoveProductToSpot(
                SponsoredProducts,
                productExternalRefrence,
                count
                );
        }

        public void RemoveSponsoredProductToSpotByDuration(
            ProductExternalReference productExternalRefrence,
            Duration duration)
        {
            RemoveProductToSpot(
                SponsoredProducts,
                productExternalRefrence,
                duration.TotalSeconds
                );
        }

        private void AddCompetitorToExclusivityCode(
          Dictionary<string, List<ProductExternalReference>> dictionary,
          string productExternalRefrence,
          string exclusivityCode)
        {
            if (!dictionary.ContainsKey(exclusivityCode))
            {
                dictionary.Add(exclusivityCode, new List<ProductExternalReference>());
            }
            if (!dictionary[exclusivityCode].Contains(productExternalRefrence))
            {
                dictionary[exclusivityCode].Add(productExternalRefrence);
            }
        }

        private void SetRestrictionValueForExclusivityCode(
            Dictionary<string, int> dictionary,
            string exclusivityCode,
            int value)
        {
            if (!dictionary.ContainsKey(exclusivityCode))
            {
                dictionary.Add(exclusivityCode, value);
            }
            else
            {
                if (dictionary[exclusivityCode] > value)
                {
                    dictionary[exclusivityCode] = value;
                }
            }
        }

        private void AddProductToSpot(
            Dictionary<ProductExternalReference, double> dictioinary,
            string productExternalRefrence,
            double value)
        {
            if (!dictioinary.ContainsKey(productExternalRefrence))
            {
                dictioinary.Add(productExternalRefrence, value);
            }
            else
            {
                dictioinary[productExternalRefrence] += value;
            }
        }

        private void RemoveProductToSpot(
            Dictionary<ProductExternalReference, double> dictioinary,
            string productExternalRefrence,
            double value)
        {
            if (dictioinary.ContainsKey(productExternalRefrence))
            {
                dictioinary[productExternalRefrence] -= value;
                if (dictioinary[productExternalRefrence] <= 0)
                {
                    _ = dictioinary.Remove(productExternalRefrence);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Types;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    public static class SponsorshipLimitsCalculator
    {
        ///<inheritdoc/>
        [Pure]
        public static SmoothSponsorshipRestrictionLimits CalculateRestrictionLimits(
                SmoothSponsorshipRunningTotals input,
                SponsorshipCalculationType calculationType,
                SponsorshipApplicability applicability
                )
        {
            switch (calculationType)
            {
                case SponsorshipCalculationType.Flat:
                    switch (applicability)
                    {
                        case SponsorshipApplicability.AllCompetitors:
                            return CalculateRestrictionLimitsForAllCompetitorsWhenCalculationTypeIsFlat(input);

                        case SponsorshipApplicability.EachCompetitor:
                            return CalculateRestrictionLimitsForEachCompetitorWhenCalculationTypeIsFlat(input);
                    }
                    break;

                case SponsorshipCalculationType.Percentage:
                    switch (applicability)
                    {
                        case SponsorshipApplicability.AllCompetitors:
                            return CalculateRestrictionLimitsForAllCompetitorsWhenCalculationTypeIsPercentage(input);

                        case SponsorshipApplicability.EachCompetitor:
                            return CalculateRestrictionLimitsForEachCompetitorWhenCalculationTypeIsPercentage(input);
                    }
                    break;
            }
            return null;
        }

        private static SmoothSponsorshipRestrictionLimits CalculateRestrictionLimitsForEachCompetitorWhenCalculationTypeIsPercentage(
            SmoothSponsorshipRunningTotals input)
        {
            var totalSponsoredProduct = input.SponsoredProducts.Sum(x => x.Value);
            if (totalSponsoredProduct == 0)
            {
                return AllCompetitorsWithUnlimitedAvailibility(input);
            }
            var output = new SmoothSponsorshipRestrictionLimits();
            foreach (var item in input.AllRestrictionValues)
            {
                var productId = item.Key;
                var alreadyPlacedValue = 0.0;
                if (input.AllProductIdsInClashOrAdvertiserCode
                  .TryGetValue(productId, out List<ProductExternalReference> products))
                {
                    foreach (var product in products)
                    {
                        if (input.AlreadyPlacedCompetitors.ContainsKey(product))
                        {
                            alreadyPlacedValue += input.AlreadyPlacedCompetitors[product];
                        }
                    }
                }

                var maximumAllowedValue = CalculateRestrictionValueForPercentage(item.Value, totalSponsoredProduct);
                var availValueRemaining = maximumAllowedValue - alreadyPlacedValue;
                if (availValueRemaining > 0)
                {
                    output.AvailabilitiesForCompetitors.Add(productId, availValueRemaining);
                }
            }
            return output;
        }

        private static SmoothSponsorshipRestrictionLimits CalculateRestrictionLimitsForAllCompetitorsWhenCalculationTypeIsPercentage(SmoothSponsorshipRunningTotals input)
        {
            var totalSponsoredProduct = input.SponsoredProducts.Sum(x => x.Value);
            if (totalSponsoredProduct == 0)
            {
                return AllCompetitorsWithUnlimitedAvailibility(input);
            }

            var output = new SmoothSponsorshipRestrictionLimits();
            var competitorsAndRestrictionValues = input.GetRestrictionValuesForCompetitors();
            var alreadyBookedCompetitorsInTotal = input.AlreadyPlacedCompetitors.Sum(x => x.Value);
            foreach (var item in competitorsAndRestrictionValues)
            {
                var productId = item.Key;
                var maximumAllowedValue = CalculateRestrictionValueForPercentage(item.Value, totalSponsoredProduct);
                var availValueRemaining = maximumAllowedValue - alreadyBookedCompetitorsInTotal;
                if (availValueRemaining > 0)
                {
                    output.AvailabilitiesForCompetitors.Add(productId, availValueRemaining);
                }
            }
            return output;
        }

        private static SmoothSponsorshipRestrictionLimits CalculateRestrictionLimitsForEachCompetitorWhenCalculationTypeIsFlat(SmoothSponsorshipRunningTotals input)
        {
            var output = new SmoothSponsorshipRestrictionLimits();
            foreach (var item in input.AllRestrictionValues)
            {
                var productId = item.Key;
                var alreadyPlacedValue = 0.0;
                if (input.AllProductIdsInClashOrAdvertiserCode
                  .TryGetValue(productId, out List<ProductExternalReference> products))
                {
                    foreach (var product in products)
                    {
                        if (input.AlreadyPlacedCompetitors.ContainsKey(product))
                        {
                            alreadyPlacedValue += input.AlreadyPlacedCompetitors[product];
                        }
                    }
                }

                var availValueRemaining = item.Value - alreadyPlacedValue;
                if (availValueRemaining > 0)
                {
                    output.AvailabilitiesForCompetitors.Add(productId, availValueRemaining);
                }
            }
            return output;
        }

        private static SmoothSponsorshipRestrictionLimits CalculateRestrictionLimitsForAllCompetitorsWhenCalculationTypeIsFlat(SmoothSponsorshipRunningTotals input)
        {
            var output = new SmoothSponsorshipRestrictionLimits();
            var competitorsAndRestrictionValues = input.GetRestrictionValuesForCompetitors();
            var alreadyBookedCompetitorsInTotal = input.AlreadyPlacedCompetitors.Sum(x => x.Value);
            foreach (var item in competitorsAndRestrictionValues)
            {
                var productId = item.Key;
                var availValueRemaining = item.Value - alreadyBookedCompetitorsInTotal;

                if (availValueRemaining > 0)
                {
                    output.AvailabilitiesForCompetitors.Add(productId, availValueRemaining);
                }
            }
            return output;
        }

        private static double CalculateRestrictionValueForPercentage(int restrictionValueInPercentage, double totalSponsoredProduct)
        {
            if (restrictionValueInPercentage == 0 || totalSponsoredProduct.CompareTo(0d) == 0)
            {
                return 0d;
            }

            var result = totalSponsoredProduct * restrictionValueInPercentage / 100;

            if (result.CompareTo(1d) == -1)
            {
                return 0;
            }

            return Math.Round(result, MidpointRounding.AwayFromZero);
        }

        private static SmoothSponsorshipRestrictionLimits AllCompetitorsWithUnlimitedAvailibility(SmoothSponsorshipRunningTotals input)
        {
            return new SmoothSponsorshipRestrictionLimits
            {
                AvailabilitiesForCompetitors = input
                    .GetAllCompetitors()
                    .ToDictionary(x => x, _ => double.MaxValue)
            };
        }
    }
}

using System.Linq;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using Xunit;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    [Collection("Smooth")]
    [Trait("Smooth", "SponsorshipLimits Calculator Test")]
    public class SponsorshipLimitsCalculatorTest
    {
        [Fact(DisplayName = "Given CalculationType is set to Flat, The breaks Availability are calculated properly for EachCompetitor based on input data")]
        internal void GivenCalculationTypeIsSetToFlat_TheBreaksAvailabilityAreCalculatedProperlyForEachCompetitorBasedOnInputData()
        {
            var input = GetSampleDataForEachCompetitorWhenCalculationTypeIsFlat();
            var output = SponsorshipLimitsCalculator.CalculateRestrictionLimits(input,
                SponsorshipCalculationType.Flat,
                SponsorshipApplicability.EachCompetitor);
            Assert.Equal(new ProductExternalReference[] { "P330", "P220", "A110", "A20" }, output.AvailabilitiesForCompetitors.Keys.ToArray());
            Assert.Equal(8, output.AvailabilitiesForCompetitors["P330"]);
            Assert.Equal(9, output.AvailabilitiesForCompetitors["P220"]);
            Assert.Equal(2, output.AvailabilitiesForCompetitors["A110"]);
            Assert.Equal(2, output.AvailabilitiesForCompetitors["A20"]);

            input.AddCompetitorToSpotByCount("C3", 1);
            output = SponsorshipLimitsCalculator.CalculateRestrictionLimits(input,
                SponsorshipCalculationType.Flat,
                SponsorshipApplicability.EachCompetitor);
            Assert.Equal(new ProductExternalReference[] { "P330", "P220", "A110", "A20" }, output.AvailabilitiesForCompetitors.Keys.ToArray());
            Assert.Equal(7, output.AvailabilitiesForCompetitors["P330"]);
            Assert.Equal(8, output.AvailabilitiesForCompetitors["P220"]);
            Assert.Equal(2, output.AvailabilitiesForCompetitors["A110"]);
            Assert.Equal(2, output.AvailabilitiesForCompetitors["A20"]);

            input.AddCompetitorToSpotByCount("C1", 2);
            output = SponsorshipLimitsCalculator.CalculateRestrictionLimits(input,
                SponsorshipCalculationType.Flat,
                SponsorshipApplicability.EachCompetitor);
            Assert.Equal(new ProductExternalReference[] { "P330", "P220" }, output.AvailabilitiesForCompetitors.Keys.ToArray());
            Assert.Equal(5, output.AvailabilitiesForCompetitors["P330"]);
            Assert.Equal(6, output.AvailabilitiesForCompetitors["P220"]);
        }

        [Fact(DisplayName = "Given CalculationType is set to Flat, The breaks Availability are calculated properly for AllCompetitors based on input data")]
        internal void GivenCalculationTypeIsSetToFlat_TheBreaksAvailabilityAreCalculatedProperlyForAllCompetitorsBasedOnInputData()
        {
            var input = GetSampleDataForAllCompetitorsWhenCalculationTypeIsFlat();
            var output = SponsorshipLimitsCalculator.CalculateRestrictionLimits(input,
                SponsorshipCalculationType.Flat,
                SponsorshipApplicability.AllCompetitors);
            Assert.Equal(new ProductExternalReference[] { "C1", "C2", "C3", "C4", "C5", "C6", "C7", "P330", "P220", "A110", "A20" }, output.AvailabilitiesForCompetitors.Keys.ToArray());
            Assert.Equal(2, output.AvailabilitiesForCompetitors["C1"]);
            Assert.Equal(2, output.AvailabilitiesForCompetitors["C2"]);
            Assert.Equal(2, output.AvailabilitiesForCompetitors["C3"]);
            Assert.Equal(2, output.AvailabilitiesForCompetitors["C4"]);
            Assert.Equal(2, output.AvailabilitiesForCompetitors["C5"]);
            Assert.Equal(2, output.AvailabilitiesForCompetitors["C6"]);
            Assert.Equal(2, output.AvailabilitiesForCompetitors["C7"]);
            Assert.Equal(2, output.AvailabilitiesForCompetitors["P330"]);
            Assert.Equal(2, output.AvailabilitiesForCompetitors["P220"]);
            Assert.Equal(2, output.AvailabilitiesForCompetitors["A110"]);
            Assert.Equal(2, output.AvailabilitiesForCompetitors["A20"]);

            input.AddCompetitorToSpotByCount("C1", 1);
            output = SponsorshipLimitsCalculator.CalculateRestrictionLimits(input,
                SponsorshipCalculationType.Flat,
                SponsorshipApplicability.AllCompetitors);
            Assert.Equal(new ProductExternalReference[] { "C1", "C2", "C3", "C4", "C5", "C6", "C7", "P330", "P220", "A110", "A20" }, output.AvailabilitiesForCompetitors.Keys.ToArray());
            Assert.Equal(1, output.AvailabilitiesForCompetitors["C1"]);
            Assert.Equal(1, output.AvailabilitiesForCompetitors["C2"]);
            Assert.Equal(1, output.AvailabilitiesForCompetitors["C3"]);
            Assert.Equal(1, output.AvailabilitiesForCompetitors["C4"]);
            Assert.Equal(1, output.AvailabilitiesForCompetitors["C5"]);
            Assert.Equal(1, output.AvailabilitiesForCompetitors["C6"]);
            Assert.Equal(1, output.AvailabilitiesForCompetitors["C7"]);
            Assert.Equal(1, output.AvailabilitiesForCompetitors["P330"]);
            Assert.Equal(1, output.AvailabilitiesForCompetitors["P220"]);
            Assert.Equal(1, output.AvailabilitiesForCompetitors["A110"]);
            Assert.Equal(1, output.AvailabilitiesForCompetitors["A20"]);

            input.AddCompetitorToSpotByCount("C3", 1);
            output = SponsorshipLimitsCalculator.CalculateRestrictionLimits(input,
                SponsorshipCalculationType.Flat,
                SponsorshipApplicability.AllCompetitors);
            Assert.Empty(output.AvailabilitiesForCompetitors);
        }

        [Fact(DisplayName = "Given CalculationType is set to Percentage when the applicability is set to AllCompetitors and there is no sponsored product, The breaks availability for each competitors is set to maximum value possible")]
        internal void GivenCalculationTypeIsSetToPercentageWhenTheApplicabilityIsSetToAllCompetitorsAndThereIsNoSponsoredProduct_TheBreaksAvailabilityForEachCompetitorsIsSetToMaximumValuePossible()
        {
            var input = GetSampleDataForAllCompetitorsWithNoSponsoredWhenCalculationTypeIsPercentage();
            var output = SponsorshipLimitsCalculator.CalculateRestrictionLimits(input,
                SponsorshipCalculationType.Percentage,
                SponsorshipApplicability.AllCompetitors);
            Assert.Equal(new ProductExternalReference[] { "C1", "C2", "C3", "C4", "C5", "C6", "C7", "P330", "P220", "A110", "A20" }, output.AvailabilitiesForCompetitors.Keys.ToArray());
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C1"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C2"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C3"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C4"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C5"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C6"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C7"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["P330"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["P220"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["A110"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["A20"]);

            input.AddCompetitorToSpotByCount("C1", 1);
            output = SponsorshipLimitsCalculator.CalculateRestrictionLimits(input,
                SponsorshipCalculationType.Percentage,
                SponsorshipApplicability.AllCompetitors);
            Assert.Equal(new ProductExternalReference[] { "C1", "C2", "C3", "C4", "C5", "C6", "C7", "P330", "P220", "A110", "A20" }, output.AvailabilitiesForCompetitors.Keys.ToArray());
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C1"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C2"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C3"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C4"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C5"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C6"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C7"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["P330"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["P220"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["A110"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["A20"]);

            input.AddCompetitorToSpotByCount("C3", 1);
            output = SponsorshipLimitsCalculator.CalculateRestrictionLimits(input,
                SponsorshipCalculationType.Percentage,
                SponsorshipApplicability.AllCompetitors);
            Assert.Equal(new ProductExternalReference[] { "C1", "C2", "C3", "C4", "C5", "C6", "C7", "P330", "P220", "A110", "A20" }, output.AvailabilitiesForCompetitors.Keys.ToArray());
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C1"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C2"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C3"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C4"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C5"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C6"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C7"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["P330"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["P220"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["A110"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["A20"]);
        }

        [Fact(DisplayName = "Given CalculationType is set to Percentage when the applicability is set to EachCompetitor and there is no sponsored product, The breaks availability for each competitors is set to maximum value possible")]
        internal void GivenCalculationTypeIsSetToPercentageWhenTheApplicabilityIsSetToEachCompetitorAndThereIsNoSponsoredProduct_TheBreaksAvailabilityForEachCompetitorsIsSetToMaximumValuePossible()
        {
            var input = GetSampleDataForEachCompetitorWithNoSponsoredWhenCalculationTypeIsPercentage();
            var output = SponsorshipLimitsCalculator.CalculateRestrictionLimits(input,
                SponsorshipCalculationType.Percentage,
                SponsorshipApplicability.EachCompetitor);
            Assert.Equal(new ProductExternalReference[] { "C1", "C2", "C3", "C4", "C5", "C6", "C7", "P330", "P220", "A110", "A20" }, output.AvailabilitiesForCompetitors.Keys.ToArray());
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C1"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C2"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C3"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C4"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C5"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C6"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C7"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["P330"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["P220"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["A110"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["A20"]);

            input.AddCompetitorToSpotByCount("C1", 1);
            output = SponsorshipLimitsCalculator.CalculateRestrictionLimits(input,
                SponsorshipCalculationType.Percentage,
                SponsorshipApplicability.EachCompetitor);
            Assert.Equal(new ProductExternalReference[] { "C1", "C2", "C3", "C4", "C5", "C6", "C7", "P330", "P220", "A110", "A20" }, output.AvailabilitiesForCompetitors.Keys.ToArray());
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C1"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C2"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C3"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C4"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C5"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C6"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C7"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["P330"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["P220"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["A110"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["A20"]);

            input.AddCompetitorToSpotByCount("C3", 1);
            output = SponsorshipLimitsCalculator.CalculateRestrictionLimits(input,
                SponsorshipCalculationType.Percentage,
                SponsorshipApplicability.EachCompetitor);
            Assert.Equal(new ProductExternalReference[] { "C1", "C2", "C3", "C4", "C5", "C6", "C7", "P330", "P220", "A110", "A20" }, output.AvailabilitiesForCompetitors.Keys.ToArray());
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C1"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C2"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C3"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C4"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C5"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C6"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["C7"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["P330"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["P220"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["A110"]);
            Assert.Equal(double.MaxValue, output.AvailabilitiesForCompetitors["A20"]);
        }

        [Fact(DisplayName = "Given CalculationType is set to Percentage when the applicability is set to AllCompetitors, The breaks availability for each competitors is calculated properly comparing to sponsored product")]
        internal void GivenCalculationTypeIsSetToPercentageWhenTheApplicabilityIsSetToAllCompetitors_TheBreaksAvailabilityForAllCompetitorsIsCalculatedProperlyComparingToSponsoredProduct()
        {
            var input = GetSampleDataForAllCompetitorsWhenCalculationTypeIsPercentage();

            var output = SponsorshipLimitsCalculator.CalculateRestrictionLimits(input,
                SponsorshipCalculationType.Percentage,
                SponsorshipApplicability.AllCompetitors);
            Assert.Equal(new ProductExternalReference[] { "C1", "C2", "P330" }, output.AvailabilitiesForCompetitors.Keys.ToArray());
            Assert.Equal(3, output.AvailabilitiesForCompetitors["C1"]);
            Assert.Equal(3, output.AvailabilitiesForCompetitors["C2"]);
            Assert.Equal(3, output.AvailabilitiesForCompetitors["P330"]);

            input.AddCompetitorToSpotByCount("C1", 1);
            output = SponsorshipLimitsCalculator.CalculateRestrictionLimits(input,
                SponsorshipCalculationType.Percentage,
                SponsorshipApplicability.AllCompetitors);
            Assert.Equal(new ProductExternalReference[] { "C1", "C2", "P330" }, output.AvailabilitiesForCompetitors.Keys.ToArray());
            Assert.Equal(2, output.AvailabilitiesForCompetitors["C1"]);
            Assert.Equal(2, output.AvailabilitiesForCompetitors["C2"]);
            Assert.Equal(2, output.AvailabilitiesForCompetitors["P330"]);

            input.AddCompetitorToSpotByCount("C2", 1);
            output = SponsorshipLimitsCalculator.CalculateRestrictionLimits(input,
                SponsorshipCalculationType.Percentage,
                SponsorshipApplicability.AllCompetitors);
            Assert.Equal(new ProductExternalReference[] { "C1", "C2", "P330" }, output.AvailabilitiesForCompetitors.Keys.ToArray());
            Assert.Equal(1, output.AvailabilitiesForCompetitors["C1"]);
            Assert.Equal(1, output.AvailabilitiesForCompetitors["C2"]);
            Assert.Equal(1, output.AvailabilitiesForCompetitors["P330"]);

            input.AddCompetitorToSpotByCount("C1", 1);
            output = SponsorshipLimitsCalculator.CalculateRestrictionLimits(input,
                SponsorshipCalculationType.Percentage,
                SponsorshipApplicability.AllCompetitors);
            Assert.Empty(output.AvailabilitiesForCompetitors);
        }

        [Fact(DisplayName = "Given CalculationType is set to Percentage when the applicability is set to EachCompetitor, The breaks availability for each competitors is calculated properly comparing to sponsored product")]
        internal void GivenCalculationTypeIsSetToPercentageWhenTheApplicabilityIsSetToEachCompetitor_TheBreaksAvailabilityForEachCompetitorsIsCalculatedProperlyComparingToSponsoredProduct()
        {
            var input = GetSampleDataForEachCompetitorWithLessThanOneAvailibityForCompetitorWhenCalculationTypeIsPercentage();

            var output = SponsorshipLimitsCalculator.CalculateRestrictionLimits(input,
                SponsorshipCalculationType.Percentage,
                SponsorshipApplicability.EachCompetitor);
            Assert.Empty(output.AvailabilitiesForCompetitors);
        }

        [Fact(DisplayName = "Given CalculationType is set to Percentage when the applicability is set to AllCompetitors but the allowed number of the competitors is lees than one, then No competitor will be allowed")]
        internal void GivenCalculationTypeIsSetToPercentageWhenTheApplicabilityIsSetToAllCompetitorsButTheAllowedNumberOfTheCompetitorsIsLeesThanOne_ThenNoCompetitorWillBeAllowed()
        {
            var input = GetSampleDataForAllCompetitorsWithLessThanOneAvailibityForCompetitorsWhenCalculationTypeIsPercentage();
            var output = SponsorshipLimitsCalculator.CalculateRestrictionLimits(input,
                SponsorshipCalculationType.Percentage,
                SponsorshipApplicability.AllCompetitors);
            Assert.Empty(output.AvailabilitiesForCompetitors);
        }

        [Fact(DisplayName = "Given CalculationType is set to Percentage when the applicability is set to EachCompetitor, The breaks availability for each competitors is calculated By Rounding up any value to the closest round number")]
        internal void GivenCalculationTypeIsSetToPercentageWhenTheApplicabilityIsSetToEachCompetitor_TheBreaksAvailabilityForEachCompetitorsIsCalculatedByRoundingUpAnyValueToTheClosestRoundNumber()
        {
            var input = GetSampleDataForEachCompetitorWithCompetitorsAvalibityInDecimalWhenCalculationTypeIsPercentage();

            var output = SponsorshipLimitsCalculator.CalculateRestrictionLimits(input,
                SponsorshipCalculationType.Percentage,
                SponsorshipApplicability.EachCompetitor);
            Assert.Equal(new ProductExternalReference[] { "P2", "P3", "P4", "P5", "P6", "P7", "P8", "P9", "P10", "P11", "P12", "P13", }, output.AvailabilitiesForCompetitors.Keys.ToArray());
            Assert.Equal(1, output.AvailabilitiesForCompetitors["P2"]);
            Assert.Equal(1, output.AvailabilitiesForCompetitors["P3"]);
            Assert.Equal(1, output.AvailabilitiesForCompetitors["P4"]);
            Assert.Equal(1, output.AvailabilitiesForCompetitors["P5"]);
            Assert.Equal(1, output.AvailabilitiesForCompetitors["P6"]);
            Assert.Equal(2, output.AvailabilitiesForCompetitors["P7"]);
            Assert.Equal(2, output.AvailabilitiesForCompetitors["P8"]);
            Assert.Equal(2, output.AvailabilitiesForCompetitors["P9"]);
            Assert.Equal(2, output.AvailabilitiesForCompetitors["P10"]);
            Assert.Equal(2, output.AvailabilitiesForCompetitors["P11"]);
            Assert.Equal(2, output.AvailabilitiesForCompetitors["P12"]);
            Assert.Equal(2, output.AvailabilitiesForCompetitors["P13"]);
        }

        [Fact(DisplayName = "Given CalculationType is set to Percentage when the applicability is set to EachCompetitor but the allowed number of the competitor is lees than one, Then the competitor will be removed from the list of competitors with availability")]
        internal void GivenCalculationTypeIsSetToPercentageWhenTheApplicabilityIsSetToEachCompetitorButTheAllowedNumberOfTheCompetitorIsLessThanOne_ThenTheCompetitorWillBeRemovedFromTheListOfCompetitorsWithAvailability()
        {
            var input = GetSampleDataForEachCompetitorWhenCalculationTypeIsPercentage();

            var output = SponsorshipLimitsCalculator.CalculateRestrictionLimits(input,
                SponsorshipCalculationType.Percentage,
                SponsorshipApplicability.EachCompetitor);

            Assert.Equal(new ProductExternalReference[] { "P330", "P220" }, output.AvailabilitiesForCompetitors.Keys.ToArray());
            Assert.Equal(4, output.AvailabilitiesForCompetitors["P330"]);
            Assert.Equal(9, output.AvailabilitiesForCompetitors["P220"]);

            input.AddCompetitorToSpotByCount("C1", 1);
            output = SponsorshipLimitsCalculator.CalculateRestrictionLimits(input,
                SponsorshipCalculationType.Percentage,
                SponsorshipApplicability.EachCompetitor);
            Assert.Equal(new ProductExternalReference[] { "P330", "P220" }, output.AvailabilitiesForCompetitors.Keys.ToArray());
            Assert.Equal(3, output.AvailabilitiesForCompetitors["P330"]);
            Assert.Equal(9, output.AvailabilitiesForCompetitors["P220"]);

            input.AddCompetitorToSpotByCount("C2", 1);
            output = SponsorshipLimitsCalculator.CalculateRestrictionLimits(input,
                SponsorshipCalculationType.Percentage,
                SponsorshipApplicability.EachCompetitor);
            Assert.Equal(new ProductExternalReference[] { "P330", "P220" }, output.AvailabilitiesForCompetitors.Keys.ToArray());
            Assert.Equal(3, output.AvailabilitiesForCompetitors["P330"]);
            Assert.Equal(8, output.AvailabilitiesForCompetitors["P220"]);

            input.AddCompetitorToSpotByCount("C1", 3);
            output = SponsorshipLimitsCalculator.CalculateRestrictionLimits(input,
                SponsorshipCalculationType.Percentage,
                SponsorshipApplicability.EachCompetitor);
            Assert.Equal(new ProductExternalReference[] { "P220" }, output.AvailabilitiesForCompetitors.Keys.ToArray());
            Assert.Equal(8, output.AvailabilitiesForCompetitors["P220"]);

            input.AddCompetitorToSpotByCount("C2", 8);
            output = SponsorshipLimitsCalculator.CalculateRestrictionLimits(input,
                SponsorshipCalculationType.Percentage,
                SponsorshipApplicability.EachCompetitor);
            Assert.Empty(output.AvailabilitiesForCompetitors);
        }

        private SmoothSponsorshipRunningTotals GetSampleDataForAllCompetitorsWhenCalculationTypeIsFlat()
        {
            var input = new SmoothSponsorshipRunningTotals();

            input.AddCompetitorToClashCode("C1", "P330");
            input.AddCompetitorToClashCode("C2", "P330");
            input.AddCompetitorToClashCode("C3", "P330");

            input.AddCompetitorToClashCode("C1", "P220");
            input.AddCompetitorToClashCode("C2", "P220");
            input.AddCompetitorToClashCode("C3", "P220");

            input.AddCompetitorToAdvertiserIdentifier("C1", "A110");
            input.AddCompetitorToAdvertiserIdentifier("C4", "A110");
            input.AddCompetitorToAdvertiserIdentifier("C5", "A110");

            input.AddCompetitorToAdvertiserIdentifier("C1", "A20");
            input.AddCompetitorToAdvertiserIdentifier("C6", "A20");
            input.AddCompetitorToAdvertiserIdentifier("C7", "A20");

            input.SetRestrictionValueForClashCode("P330", 5);
            input.SetRestrictionValueForClashCode("P220", 5);
            input.SetRestrictionValueForAdvertiserIdentifier("A110", 5);
            input.SetRestrictionValueForAdvertiserIdentifier("A20", 5);

            input.AddCompetitorToSpotByCount("C1", 1);
            input.AddCompetitorToSpotByCount("C5", 2);

            input.AddSponsoredProductToSpotByCount("SP1", 1);
            input.AddSponsoredProductToSpotByCount("SP2", 1);
            input.AddSponsoredProductToSpotByCount("SP3", 1);

            return input;
        }

        private SmoothSponsorshipRunningTotals GetSampleDataForEachCompetitorWhenCalculationTypeIsFlat()
        {
            var input = new SmoothSponsorshipRunningTotals();

            input.AddCompetitorToClashCode("C1", "P330");
            input.AddCompetitorToClashCode("C2", "P330");
            input.AddCompetitorToClashCode("C3", "P330");

            input.AddCompetitorToClashCode("C1", "P220");
            input.AddCompetitorToClashCode("C2", "P220");
            input.AddCompetitorToClashCode("C3", "P220");

            input.AddCompetitorToAdvertiserIdentifier("C1", "A110");
            input.AddCompetitorToAdvertiserIdentifier("C4", "A110");
            input.AddCompetitorToAdvertiserIdentifier("C5", "A110");

            input.AddCompetitorToAdvertiserIdentifier("C1", "A20");
            input.AddCompetitorToAdvertiserIdentifier("C6", "A20");
            input.AddCompetitorToAdvertiserIdentifier("C7", "A20");

            input.SetRestrictionValueForClashCode("P330", 10);
            input.SetRestrictionValueForClashCode("P220", 11);
            input.SetRestrictionValueForAdvertiserIdentifier("A110", 5);
            input.SetRestrictionValueForAdvertiserIdentifier("A20", 3);

            input.AddCompetitorToSpotByCount("C1", 1);
            input.AddCompetitorToSpotByCount("C2", 1);
            input.AddCompetitorToSpotByCount("C5", 2);

            input.AddSponsoredProductToSpotByCount("SP1", 1);
            input.AddSponsoredProductToSpotByCount("SP2", 1);
            input.AddSponsoredProductToSpotByCount("SP3", 1);

            return input;
        }

        private SmoothSponsorshipRunningTotals GetSampleDataForAllCompetitorsWithNoSponsoredWhenCalculationTypeIsPercentage()
        {
            var input = new SmoothSponsorshipRunningTotals();

            input.AddCompetitorToClashCode("C1", "P330");
            input.AddCompetitorToClashCode("C2", "P330");
            input.AddCompetitorToClashCode("C3", "P330");

            input.AddCompetitorToClashCode("C1", "P220");
            input.AddCompetitorToClashCode("C2", "P220");
            input.AddCompetitorToClashCode("C3", "P220");

            input.AddCompetitorToAdvertiserIdentifier("C1", "A110");
            input.AddCompetitorToAdvertiserIdentifier("C4", "A110");
            input.AddCompetitorToAdvertiserIdentifier("C5", "A110");

            input.AddCompetitorToAdvertiserIdentifier("C1", "A20");
            input.AddCompetitorToAdvertiserIdentifier("C6", "A20");
            input.AddCompetitorToAdvertiserIdentifier("C7", "A20");

            input.SetRestrictionValueForClashCode("P330", 5);
            input.SetRestrictionValueForClashCode("P220", 5);
            input.SetRestrictionValueForAdvertiserIdentifier("A110", 5);
            input.SetRestrictionValueForAdvertiserIdentifier("A20", 5);

            input.AddCompetitorToSpotByCount("C1", 1);
            input.AddCompetitorToSpotByCount("C5", 2);

            return input;
        }

        private SmoothSponsorshipRunningTotals GetSampleDataForEachCompetitorWithNoSponsoredWhenCalculationTypeIsPercentage()
        {
            var input = new SmoothSponsorshipRunningTotals();

            input.AddCompetitorToClashCode("C1", "P330");
            input.AddCompetitorToClashCode("C2", "P330");
            input.AddCompetitorToClashCode("C3", "P330");

            input.AddCompetitorToClashCode("C1", "P220");
            input.AddCompetitorToClashCode("C2", "P220");
            input.AddCompetitorToClashCode("C3", "P220");

            input.AddCompetitorToAdvertiserIdentifier("C1", "A110");
            input.AddCompetitorToAdvertiserIdentifier("C4", "A110");
            input.AddCompetitorToAdvertiserIdentifier("C5", "A110");

            input.AddCompetitorToAdvertiserIdentifier("C1", "A20");
            input.AddCompetitorToAdvertiserIdentifier("C6", "A20");
            input.AddCompetitorToAdvertiserIdentifier("C7", "A20");

            input.SetRestrictionValueForClashCode("P330", 10);
            input.SetRestrictionValueForClashCode("P220", 11);
            input.SetRestrictionValueForAdvertiserIdentifier("A110", 5);
            input.SetRestrictionValueForAdvertiserIdentifier("A20", 3);

            input.AddCompetitorToSpotByCount("C1", 1);
            input.AddCompetitorToSpotByCount("C2", 1);
            input.AddCompetitorToSpotByCount("C5", 2);

            return input;
        }

        private SmoothSponsorshipRunningTotals GetSampleDataForAllCompetitorsWhenCalculationTypeIsPercentage()
        {
            var input = new SmoothSponsorshipRunningTotals();

            input.AddCompetitorToClashCode("C1", "P330");
            input.AddCompetitorToClashCode("C2", "P330");

            input.SetRestrictionValueForClashCode("P330", 10);

            input.AddCompetitorToSpotByCount("C1", 1);
            input.AddCompetitorToSpotByCount("C2", 1);

            input.AddSponsoredProductToSpotByCount("P1", 20);
            input.AddSponsoredProductToSpotByCount("P2", 30);

            return input;
        }

        private SmoothSponsorshipRunningTotals GetSampleDataForEachCompetitorWhenCalculationTypeIsPercentage()
        {
            var input = new SmoothSponsorshipRunningTotals();

            input.AddCompetitorToClashCode("C1", "P330");
            input.AddCompetitorToClashCode("C2", "P220");

            input.SetRestrictionValueForClashCode("P330", 10);
            input.SetRestrictionValueForClashCode("P220", 20);

            input.AddCompetitorToSpotByCount("C1", 1);
            input.AddCompetitorToSpotByCount("C2", 1);

            input.AddSponsoredProductToSpotByCount("P1", 20);
            input.AddSponsoredProductToSpotByCount("P2", 30);

            return input;
        }

        private SmoothSponsorshipRunningTotals GetSampleDataForAllCompetitorsWithLessThanOneAvailibityForCompetitorsWhenCalculationTypeIsPercentage()
        {
            var input = new SmoothSponsorshipRunningTotals();

            input.AddCompetitorToClashCode("C1", "P330");
            input.AddCompetitorToClashCode("C2", "P200");

            input.SetRestrictionValueForClashCode("P330", 9);
            input.SetRestrictionValueForClashCode("P200", 9);

            input.AddSponsoredProductToSpotByCount("P1", 10);

            return input;
        }

        private SmoothSponsorshipRunningTotals GetSampleDataForEachCompetitorWithLessThanOneAvailibityForCompetitorWhenCalculationTypeIsPercentage()
        {
            var input = new SmoothSponsorshipRunningTotals();

            input.AddCompetitorToClashCode("C1", "P330");
            input.AddCompetitorToClashCode("C2", "P200");

            input.SetRestrictionValueForClashCode("P330", 9);
            input.SetRestrictionValueForClashCode("P200", 4);

            input.AddSponsoredProductToSpotByCount("P1", 10);

            return input;
        }

        private SmoothSponsorshipRunningTotals GetSampleDataForEachCompetitorWithCompetitorsAvalibityInDecimalWhenCalculationTypeIsPercentage()
        {
            var input = new SmoothSponsorshipRunningTotals();

            input.AddCompetitorToClashCode("C1", "P1");
            input.AddCompetitorToClashCode("C2", "P2");
            input.AddCompetitorToClashCode("C3", "P3");
            input.AddCompetitorToClashCode("C4", "P4");
            input.AddCompetitorToClashCode("C5", "P5");
            input.AddCompetitorToClashCode("C6", "P6");
            input.AddCompetitorToClashCode("C7", "P7");
            input.AddCompetitorToClashCode("C8", "P8");
            input.AddCompetitorToClashCode("C9", "P9");
            input.AddCompetitorToClashCode("C10", "P10");
            input.AddCompetitorToClashCode("C11", "P11");
            input.AddCompetitorToClashCode("C12", "P12");
            input.AddCompetitorToClashCode("C13", "P13");

            input.SetRestrictionValueForClashCode("P1", 9);
            input.SetRestrictionValueForClashCode("P2", 10);
            input.SetRestrictionValueForClashCode("P3", 11);
            input.SetRestrictionValueForClashCode("P4", 12);
            input.SetRestrictionValueForClashCode("P5", 13);
            input.SetRestrictionValueForClashCode("P6", 14);
            input.SetRestrictionValueForClashCode("P7", 15);
            input.SetRestrictionValueForClashCode("P8", 16);
            input.SetRestrictionValueForClashCode("P9", 17);
            input.SetRestrictionValueForClashCode("P10", 18);
            input.SetRestrictionValueForClashCode("P11", 19);
            input.SetRestrictionValueForClashCode("P12", 20);
            input.SetRestrictionValueForClashCode("P13", 21);

            input.AddSponsoredProductToSpotByCount("SponsoredProduct", 10);

            return input;
        }
    }
}

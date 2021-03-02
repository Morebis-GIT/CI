using System.Linq;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using NodaTime;
using Xunit;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    [Collection(nameof(SmoothCollectionDefinition))]
    [Trait("Smooth", "SmoothSponsorship Running Totals Test")]
    public class SmoothSponsorshipRunningTotalsTest
    {
        [Fact(DisplayName = "Given ProductIds are added to some Clash and Advertiser codes, The AllProductIdsInClashOrAdvertiserCode will contains all the values")]
        internal void GivenProductIdsAreAddedToSomeClashesAndAdvertisersCodes_TheAllProductIdsInClashOrAdvertiserCodeWillContainsAllTheValues()
        {
            var runningTotal = new SmoothSponsorshipRunningTotals();
            runningTotal.AddCompetitorToClashCode("p1", "c1");
            runningTotal.AddCompetitorToClashCode("p2", "c1");
            runningTotal.AddCompetitorToClashCode("p1", "c2");
            runningTotal.AddCompetitorToAdvertiserIdentifier("p1", "a1");
            runningTotal.AddCompetitorToAdvertiserIdentifier("p2", "a1");
            runningTotal.AddCompetitorToAdvertiserIdentifier("p1", "a2");

            Assert.Equal(new string[] { "c1", "c2", "a1", "a2" }, runningTotal.AllProductIdsInClashOrAdvertiserCode.Keys.ToArray());
            Assert.Equal(new ProductExternalReference[] { "p1", "p2" }, runningTotal.AllProductIdsInClashOrAdvertiserCode["c1"].ToArray());
            Assert.Equal(new ProductExternalReference[] { "p1", "p2" }, runningTotal.AllProductIdsInClashOrAdvertiserCode["a1"].ToArray());
            Assert.Equal(new ProductExternalReference[] { "p1" }, runningTotal.AllProductIdsInClashOrAdvertiserCode["c2"].ToArray());
            Assert.Equal(new ProductExternalReference[] { "p1" }, runningTotal.AllProductIdsInClashOrAdvertiserCode["a2"].ToArray());
        }

        [Fact(DisplayName = "Given restriction values are added to some Clash and Advertiser codes, The AllRestrictionValues will contains all the values")]
        internal void GivenRestrictionValuesAreAddedToSomeClashAndAdvertiserCodes_TheAllRestrictionValuesWillContainsAllTheValues()
        {
            var runningTotal = new SmoothSponsorshipRunningTotals();
            runningTotal.SetRestrictionValueForClashCode("c1", 10);
            runningTotal.SetRestrictionValueForClashCode("c2", 20);
            runningTotal.SetRestrictionValueForClashCode("c1", 1);
            runningTotal.SetRestrictionValueForClashCode("c2", 2);
            runningTotal.SetRestrictionValueForClashCode("a1", 100);
            runningTotal.SetRestrictionValueForClashCode("a2", 200);
            runningTotal.SetRestrictionValueForClashCode("a1", 10);
            runningTotal.SetRestrictionValueForClashCode("a2", 20);

            Assert.Equal(new string[] { "c1", "c2", "a1", "a2" }, runningTotal.AllRestrictionValues.Keys.ToArray());
            Assert.Equal(1, runningTotal.AllRestrictionValues["c1"]);
            Assert.Equal(2, runningTotal.AllRestrictionValues["c2"]);
            Assert.Equal(10, runningTotal.AllRestrictionValues["a1"]);
            Assert.Equal(20, runningTotal.AllRestrictionValues["a2"]);
        }

        [Fact(DisplayName = "Given Competitors are being added and removed from the spots by count, The AlreadyPlacedCompetitors will contains all competitors values in the spots")]
        internal void GivenCompetitorsAreBeingAddedAndRemovedFromTheSpotsByCount_TheAlreadyPlacedCompetitorsWillContainsAllCompetitorsValuesInTheSpots()
        {
            var runningTotal = new SmoothSponsorshipRunningTotals();
            runningTotal.AddCompetitorToSpotByCount("p1", 1);
            runningTotal.AddCompetitorToSpotByCount("p2", 1);
            runningTotal.AddCompetitorToSpotByCount("p3", 1);
            runningTotal.AddCompetitorToSpotByCount("p1", 10);
            runningTotal.AddCompetitorToSpotByCount("p2", 1);
            runningTotal.RemoveCompetitorToSpotByCount("p1", 3);
            runningTotal.RemoveCompetitorToSpotByCount("p3", 1);
            runningTotal.RemoveCompetitorToSpotByCount("p4", 1);

            Assert.Equal(new ProductExternalReference[] { "p1", "p2" }, runningTotal.AlreadyPlacedCompetitors.Keys.ToArray());
            Assert.Equal(8, runningTotal.AlreadyPlacedCompetitors["p1"]);
            Assert.Equal(2, runningTotal.AlreadyPlacedCompetitors["p2"]);
        }

        [Fact(DisplayName = "Given Competitors are being added and removed from the spots by duration, The AlreadyPlacedCompetitors will contains all competitors values in the spots")]
        internal void GivenCompetitorsAreBeingAddedAndRemovedFromTheSpotsByDuration_TheAlreadyPlacedCompetitorsWillContainsAllCompetitorsValuesInTheSpots()
        {
            var runningTotal = new SmoothSponsorshipRunningTotals();
            runningTotal.AddCompetitorToSpotByDuration("p1", Duration.FromMinutes(1));
            runningTotal.AddCompetitorToSpotByDuration("p2", Duration.FromMinutes(2));
            runningTotal.AddCompetitorToSpotByDuration("p3", Duration.FromMinutes(5));
            runningTotal.AddCompetitorToSpotByDuration("p1", Duration.FromMinutes(3));
            runningTotal.AddCompetitorToSpotByDuration("p2", Duration.FromMinutes(1));
            runningTotal.RemoveCompetitorToSpotByDuration("p1", Duration.FromMinutes(2));
            runningTotal.RemoveCompetitorToSpotByDuration("p3", Duration.FromMinutes(5));
            runningTotal.RemoveCompetitorToSpotByDuration("p4", Duration.FromMinutes(5));

            Assert.Equal(new ProductExternalReference[] { "p1", "p2" }, runningTotal.AlreadyPlacedCompetitors.Keys.ToArray());
            Assert.Equal(120, runningTotal.AlreadyPlacedCompetitors["p1"]);
            Assert.Equal(180, runningTotal.AlreadyPlacedCompetitors["p2"]);
        }

        [Fact(DisplayName = "Given SponsoredProducts are being added and removed from the spots by count, The SponsoredProducts will contains all sponsored products values in the spots")]
        internal void GivenSponsoredProductsAreBeingAddedAndRemovedFromTheSpotsByCount_TheSponsoredProductsWillContainsAllCompetitorsValuesInTheSpots()
        {
            var runningTotal = new SmoothSponsorshipRunningTotals();
            runningTotal.AddSponsoredProductToSpotByCount("p1", 1);
            runningTotal.AddSponsoredProductToSpotByCount("p2", 1);
            runningTotal.AddSponsoredProductToSpotByCount("p3", 1);
            runningTotal.AddSponsoredProductToSpotByCount("p1", 10);
            runningTotal.AddSponsoredProductToSpotByCount("p2", 1);
            runningTotal.RemoveSponsoredProductToSpotByCount("p1", 3);
            runningTotal.RemoveSponsoredProductToSpotByCount("p3", 1);
            runningTotal.RemoveSponsoredProductToSpotByCount("p4", 1);

            Assert.Equal(new ProductExternalReference[] { "p1", "p2" }, runningTotal.SponsoredProducts.Keys.ToArray());
            Assert.Equal(8, runningTotal.SponsoredProducts["p1"]);
            Assert.Equal(2, runningTotal.SponsoredProducts["p2"]);
        }

        [Fact(DisplayName = "Given SponsoredProducts are being added and removed from the spots by duration, The SponsoredProducts will contains all sponsored products values in the spots")]
        internal void GivenSponsoredProductsAreBeingAddedAndRemovedFromTheSpotsByDuration_TheSponsoredProductsWillContainsAllCompetitorsValuesInTheSpots()
        {
            var runningTotal = new SmoothSponsorshipRunningTotals();
            runningTotal.AddSponsoredProductToSpotByDuration("p1", Duration.FromMinutes(1));
            runningTotal.AddSponsoredProductToSpotByDuration("p2", Duration.FromMinutes(2));
            runningTotal.AddSponsoredProductToSpotByDuration("p3", Duration.FromMinutes(5));
            runningTotal.AddSponsoredProductToSpotByDuration("p1", Duration.FromMinutes(3));
            runningTotal.AddSponsoredProductToSpotByDuration("p2", Duration.FromMinutes(1));
            runningTotal.RemoveSponsoredProductToSpotByDuration("p1", Duration.FromMinutes(2));
            runningTotal.RemoveSponsoredProductToSpotByDuration("p3", Duration.FromMinutes(5));
            runningTotal.RemoveSponsoredProductToSpotByDuration("p4", Duration.FromMinutes(5));

            Assert.Equal(new ProductExternalReference[] { "p1", "p2" }, runningTotal.SponsoredProducts.Keys.ToArray());
            Assert.Equal(120, runningTotal.SponsoredProducts["p1"]);
            Assert.Equal(180, runningTotal.SponsoredProducts["p2"]);
        }

        [Fact(DisplayName = "Given the Products are assigned to Clash codes and AdvertiserIdentifier, when the restriction values are set for each code Then the products with more restriction values assign to them  get the minimum restriction value")]
        internal void GivenTheProductsAreAssignedToClashCodesAndAdvertiserIdentifierWhenTheRestrictionValuesAreSetForEachCode_ThenTheProductsWithMoreRestrictionValuesAssignToThemGetTheMinimumRestrictionValue()
        {
            var runningTotal = new SmoothSponsorshipRunningTotals();
            runningTotal.AddCompetitorToClashCode("p1", "c1");
            runningTotal.AddCompetitorToClashCode("p2", "c1");
            runningTotal.AddCompetitorToClashCode("p1", "c2");
            runningTotal.AddCompetitorToAdvertiserIdentifier("p1", "a1");
            runningTotal.AddCompetitorToAdvertiserIdentifier("p2", "a1");
            runningTotal.AddCompetitorToAdvertiserIdentifier("p1", "a2");

            runningTotal.SetRestrictionValueForClashCode("c1", 10);
            runningTotal.SetRestrictionValueForClashCode("c2", 20);
            runningTotal.SetRestrictionValueForClashCode("c1", 1);
            runningTotal.SetRestrictionValueForClashCode("c2", 2);
            runningTotal.SetRestrictionValueForClashCode("a1", 100);
            runningTotal.SetRestrictionValueForClashCode("a2", 200);
            runningTotal.SetRestrictionValueForClashCode("a1", 10);
            runningTotal.SetRestrictionValueForClashCode("a2", 20);

            var restrictionValuesForCompetitors = runningTotal.GetRestrictionValuesForCompetitors();
            Assert.Equal(new ProductExternalReference[] { "p1", "p2", "c1", "c2", "a1", "a2" }, restrictionValuesForCompetitors.Keys.ToArray());
            Assert.Equal(1, restrictionValuesForCompetitors["p1"]);
            Assert.Equal(1, restrictionValuesForCompetitors["p2"]);
            Assert.Equal(1, restrictionValuesForCompetitors["c1"]);
            Assert.Equal(2, restrictionValuesForCompetitors["c2"]);
            Assert.Equal(10, restrictionValuesForCompetitors["a1"]);
            Assert.Equal(20, restrictionValuesForCompetitors["a2"]);
        }

        [Fact(DisplayName = "Given no Products are assigned to Clash codes and AdvertiserIdentifier, when the restriction values are set for each code clash and advertiser, Then the clashes and advertisers will be in the restriction values")]
        internal void GivenNoProductsAreAssignedToClashCodesAndAdvertiserIdentifierWhenTheRestrictionValuesAreSetForEachCodeClashAndAdvertiserThenTheClashesAndAdvertisersWillBeInTheRrestrictionValue()
        {
            var runningTotal = new SmoothSponsorshipRunningTotals();
            runningTotal.SetRestrictionValueForClashCode("c1", 10);
            runningTotal.SetRestrictionValueForClashCode("c2", 20);
            runningTotal.SetRestrictionValueForClashCode("c1", 1);
            runningTotal.SetRestrictionValueForClashCode("c2", 2);
            runningTotal.SetRestrictionValueForClashCode("a1", 100);
            runningTotal.SetRestrictionValueForClashCode("a2", 200);
            runningTotal.SetRestrictionValueForClashCode("a1", 10);
            runningTotal.SetRestrictionValueForClashCode("a2", 20);

            var restrictionValuesForCompetitors = runningTotal.GetRestrictionValuesForCompetitors();
            Assert.Equal(new ProductExternalReference[] { "c1", "c2", "a1", "a2" }, restrictionValuesForCompetitors.Keys.ToArray());
            Assert.Equal(1, restrictionValuesForCompetitors["c1"]);
            Assert.Equal(2, restrictionValuesForCompetitors["c2"]);
            Assert.Equal(10, restrictionValuesForCompetitors["a1"]);
            Assert.Equal(20, restrictionValuesForCompetitors["a2"]);
        }

        [Fact(DisplayName = "Given the Products are assigned to Clash codes and AdvertiserIdentifier, when the GetAllCompetitors is called, then a list of all unique products is returned")]
        internal void GivenTheProductsAreAssignedToClashCodesAndAdvertiserIdentifierWhenTheGetAllCompetitorsIsCalled_ThenAListOfAllUniqueProductsIsReturned()
        {
            var runningTotal = new SmoothSponsorshipRunningTotals();
            runningTotal.AddCompetitorToClashCode("p1", "c1");
            runningTotal.AddCompetitorToClashCode("p2", "c1");
            runningTotal.AddCompetitorToClashCode("p1", "c2");
            runningTotal.AddCompetitorToAdvertiserIdentifier("p1", "a1");
            runningTotal.AddCompetitorToAdvertiserIdentifier("p2", "a1");
            runningTotal.AddCompetitorToAdvertiserIdentifier("p1", "a2");
            runningTotal.AddCompetitorToAdvertiserIdentifier("p3", "a3");

            var competitors = runningTotal.GetAllCompetitors();
            Assert.Equal(new ProductExternalReference[] { "p1", "p2", "p3" }, competitors);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.Sponsorships;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.CollectionFixtures;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.Pivot;
using Xunit;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    /// <summary>
    /// Base class for sponsorship restriction tests.
    /// </summary>
    public abstract class SponsorshipRestrictionsTests
        : DataDrivenTests
    {
        private readonly SmoothFixture _smoothFixture;

        /// <summary>
        /// Initialize a new instance of the
        /// <see cref="SponsorshipRestrictionsTests"/> class.
        /// </summary>
        /// <param name="loadAllCoreData">
        /// When <see langword="true"/>, load all core data, otherwise only load
        /// settings and metadata.
        /// </param>
        protected SponsorshipRestrictionsTests(
            SmoothFixture smoothFixture,
            ITestOutputHelper output,
            bool loadAllCoreData = true)
            : base(smoothFixture, output, loadAllCoreData)
        {
            _smoothFixture = smoothFixture;
        }

        protected void DeleteAllSponsorshipRestrictionsForThisTest()
        {
            using var scope = RepositoryFactory.BeginRepositoryScope();
            var repo = scope.CreateRepository<ISponsorshipRepository>();
            _ = repo.TruncateAsync();
            repo.SaveChanges();
        }

        private static DateTimeFormatInfo
        CultureForTestDataDateTimes
        { get; } = new CultureInfo("en-GB").DateTimeFormat;

        protected Sponsorship GetTestCaseSponsorshipObject(
            string calculationType,
            string restrictionLevel,
            string applicability,
            string programmeName,
            string clashExternalRef,
            string advertiserIdentifier,
            string salesArea,
            string startDate,
            string endDate,
            string startTime,
            string endTime,
            string daysOfWeek,
            string sponsoredProduct
            )
        {
            return new Sponsorship
            {
                ExternalReferenceId = Guid.NewGuid().ToString(),
                RestrictionLevel = (SponsorshipRestrictionLevel)Enum.Parse(typeof(SponsorshipRestrictionLevel), restrictionLevel),
                SponsoredItems = new List<SponsoredItem>{
                    new SponsoredItem{
                        Applicability = string.IsNullOrEmpty(applicability)
                            ? null
                            : (SponsorshipApplicability?)Enum.Parse(typeof(SponsorshipApplicability), applicability),
                        CalculationType = (SponsorshipCalculationType)Enum.Parse(typeof(SponsorshipCalculationType), calculationType),
                        Products = sponsoredProduct.Split('|'),
                        AdvertiserExclusivities = string.IsNullOrEmpty(advertiserIdentifier)
                            ? null
                            : new List<AdvertiserExclusivity>
                            {
                                new AdvertiserExclusivity
                                {
                                    AdvertiserIdentifier = advertiserIdentifier
                                }
                            },
                        ClashExclusivities = string.IsNullOrEmpty(clashExternalRef)
                            ? null
                            : new List<ClashExclusivity>
                            {
                                new ClashExclusivity
                                {
                                    ClashExternalRef = clashExternalRef
                                }
                            },
                        SponsorshipItems = new List<SponsorshipItem>{
                            new SponsorshipItem{
                                SalesAreas = new List<string>{ salesArea },
                                StartDate = DateTime.Parse(startDate, CultureForTestDataDateTimes),
                                EndDate = DateTime.Parse(endDate, CultureForTestDataDateTimes),
                                DayParts = new List<SponsoredDayPart>{
                                    new SponsoredDayPart{
                                        StartTime = TimeSpan.Parse(startTime, CultureForTestDataDateTimes),
                                        EndTime = TimeSpan.Parse(endTime, CultureForTestDataDateTimes),
                                        DaysOfWeek = new string[]{ daysOfWeek }
                                    }
                                },
                                ProgrammeName = programmeName
                            }
                        }
                    }
                }
            };
        }

        protected Sponsorship GetTestCaseSponsorshipObject(
            string calculationType,
            string restrictionLevel,
            string sponsoredItemRestrictionType,
            string sponsoredItemRestrictionValue,
            string applicability,
            string programmeName,
            string clashExternalRef,
            string clashRestrictionType,
            string clashRestrictionValue,
            string advertiserIdentifier,
            string advertiserRestrictionType,
            string advertiserRestrictionValue,
            string salesArea,
            string startDate,
            string endDate,
            string startTime,
            string endTime,
            string daysOfWeek,
            string sponsoredProduct
            )
        {
            Sponsorship result = GetTestCaseSponsorshipObject(
                calculationType,
                restrictionLevel,
                applicability,
                programmeName,
                clashExternalRef,
                advertiserIdentifier,
                salesArea,
                startDate,
                endDate,
                startTime,
                endTime,
                daysOfWeek,
                sponsoredProduct);

            SponsoredItem sponsoredItem = result.SponsoredItems[0];

            sponsoredItem.RestrictionType = GetRestrictionType(sponsoredItemRestrictionType);
            sponsoredItem.RestrictionValue = GetRestrictionValue(sponsoredItemRestrictionValue);

            if (sponsoredItem.ClashExclusivities != null)
            {
                sponsoredItem.ClashExclusivities[0].RestrictionType = GetRestrictionType(clashRestrictionType);
                sponsoredItem.ClashExclusivities[0].RestrictionValue = GetRestrictionValue(clashRestrictionValue);
            }

            if (sponsoredItem.AdvertiserExclusivities != null)
            {
                sponsoredItem.AdvertiserExclusivities[0].RestrictionType = GetRestrictionType(advertiserRestrictionType);
                sponsoredItem.AdvertiserExclusivities[0].RestrictionValue = GetRestrictionValue(advertiserRestrictionValue);
            }

            return result;
        }

        /// <summary>
        /// Always the sponsored spots MUST have a Preemptlevel value that is
        /// smaller than the non-sponsored spots
        /// </summary>
        /// <param name="sponsoredProduct"></param>
        /// <param name="expectedBreaks"></param>
        protected void UpdateSpotsPreemptlevelValuesForSponsoredProductToComplySponsorshipObject(
            string sponsoredProduct,
            string expectedBreaks)
        {
            var firstExpectedBreaks = string.Empty;
            if (!string.IsNullOrEmpty(expectedBreaks))
            {
                firstExpectedBreaks = expectedBreaks.Split('|')[0];
            }
            if (!string.IsNullOrEmpty(firstExpectedBreaks))
            {
                using var scope = RepositoryFactory.BeginRepositoryScope();
                var repo = scope.CreateRepository<ISpotRepository>();
                var sponsoredProductInTheFirstBreak = repo.GetAll().FirstOrDefault(spot => string.Equals(spot.BreakRequest, firstExpectedBreaks) && sponsoredProduct.Contains(spot.Product));
                if (sponsoredProductInTheFirstBreak != null)
                {
                    sponsoredProductInTheFirstBreak.Preemptable = true;
                    sponsoredProductInTheFirstBreak.Preemptlevel = 1;
                    sponsoredProductInTheFirstBreak.Sponsored = true;

                    var otherSponsoredSpotsIds = repo.GetAll()
                        .Where(spot => expectedBreaks.Contains(spot.BreakRequest)
                        && sponsoredProduct.Contains(spot.Product)
                        && spot.Uid != sponsoredProductInTheFirstBreak.Uid)
                            .Select(spot => spot.Uid);
                    repo.Delete(otherSponsoredSpotsIds);
                    repo.SaveChanges();

                    var allOtherSpots = repo.GetAll()
                        .Where(spot => spot.Uid != sponsoredProductInTheFirstBreak.Uid);

                    foreach (var spot in allOtherSpots)
                    {
                        spot.Sponsored = false;
                    }

                    repo.SaveChanges();
                }
            }
        }

        protected void AddSponsorshipRestrictionsForThisTest(Sponsorship sponsorship)
        {
            using var scope = RepositoryFactory.BeginRepositoryScope();
            var repo = scope.CreateRepository<ISponsorshipRepository>();
            repo.Add(sponsorship);
            repo.SaveChanges();
        }

        protected void AssertRecommendationsAreSetWithNoSponsorshipRules(
            IReadOnlyCollection<Recommendation> recommendations,
            string code = null)
        {
            Assert.NotNull(_smoothFixture);

            ref readonly ProgrammeResult[] expectedResult =
                ref _smoothFixture.RecommendationsAreSetWithNoSponsorshipRulesExpectedResults();

            var expectedSpots = expectedResult
                .SelectMany(programme => programme.Breaks)
                .SelectMany(aBreak => aBreak.SpotsInPositionOrder);

            var pivotRecommendations = recommendations.Pivot(RepositoryFactory);
            var testCode = $"TestCode:{code}";

            _ = recommendations
                .Where(r => r.ExternalProgrammeReference != null)
                .Should()
                .HaveSameCount(expectedSpots, testCode, becauseArgs: null);

            _ = pivotRecommendations.SalesAreas
                    .Should()
                    .ContainSingle(testCode, becauseArgs: null);

            const string expectedSalesArea = "NWS91";

            _ = pivotRecommendations.SalesAreaByNames()
                    .Should()
                    .BeEquivalentTo(expectedSalesArea);

            PivotSalesArea pivotSalesArea = pivotRecommendations
                .SalesArea(expectedSalesArea);

            _ = pivotSalesArea.Programmes
                    .Should()
                    .HaveSameCount(expectedResult, testCode, becauseArgs: null);

            var expectedProgrammesExternalReference = expectedResult
                    .Select(x => x.ProgrammeExternalReference)
                    .ToArray();

            _ = pivotSalesArea.ProgrammesExternalReference
                    .Should()
                    .BeEquivalentTo(expectedProgrammesExternalReference);

            foreach (var result in expectedResult)
            {
                _ = pivotSalesArea.ProgrammesExternalReference
                        .Should()
                        .Contain(result.ProgrammeExternalReference, becauseArgs: null);

                PivotProgramme pivotProgramme = pivotSalesArea
                    .Programme(result.ProgrammeExternalReference);

                _ = pivotProgramme.Breaks
                        .Should()
                        .HaveSameCount(result.Breaks, testCode, becauseArgs: null);

                _ = pivotProgramme.BreaksExternalReference
                        .Should()
                        .BeEquivalentTo(result.Breaks.Select(x => x.ExternalReference).ToArray(), testCode);

                foreach (var aBreak in result.Breaks)
                {
                    PivotBreak pivotBreak = pivotProgramme
                        .Break(aBreak.ExternalReference);

                    _ = pivotBreak.Spots
                        .Should()
                        .HaveSameCount(aBreak.SpotsInPositionOrder, testCode, becauseArgs: null);

                    _ = pivotBreak.ProductIds
                        .Should()
                        .BeEquivalentTo(aBreak.Products);

                    _ = pivotBreak.SpotByExternalReference
                        .Should()
                        .BeEquivalentTo(aBreak.SpotsInPositionOrder);

                    int position = 0;
                    foreach (var spot in aBreak.SpotsInPositionOrder)
                    {
                        position++;

                        _ = pivotBreak.Spot(spot)
                            .ShouldBePlaced()
                            .AtActualPositionInBreak(position);
                    }
                }
            }
        }

        /// <summary>
        /// <para>Sponsorship.CalculationType: None</para>
        /// <para>Looking at the breaks:</para>
        /// <list type="bullet">
        /// <item>
        /// <description>
        /// If any sponsoredProduct products booked into any break first, the
        /// competitors cannot be booked into that break
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// If any competitor products booked into any break first, the
        /// sponsoredProducts cannot be booked into that break
        /// </description>
        /// </item>
        /// </list>
        /// There can be Breaks which is not booked with an sponsoredProduct or competitors.
        /// </summary>
        /// <param name="recommendations"></param>
        /// <param name="expectedBreaks"></param>
        /// <param name="sponsoredProduct"></param>
        /// <param name="competitors"></param>
        /// <param name="code"></param>
        protected void AssertRecommendationsAreSetWithSponsorshipRulesForCalculationTypeSetToNone(
            IReadOnlyCollection<Recommendation> recommendations,
            string expectedBreaks,
            string sponsoredProduct,
            string competitors,
            string code)
        {
            var pivotRecommendations = recommendations.Pivot(RepositoryFactory);
            var expectedBreakArray = expectedBreaks.Split('|');
            var competitorsArray = competitors.Split('|');
            var sponsoredProductArray = sponsoredProduct.Split('|');

            var breakVsProductId = pivotRecommendations.BreaksVsProductIds;
            _ = breakVsProductId.Should().HaveCount(9, becauseArgs: null);

            foreach (var br in expectedBreakArray)
            {
                Assert.True(breakVsProductId.ContainsKey(br));

                var allProductsBooked = breakVsProductId[br];

                var breakContainingAnySponsoredProduct = allProductsBooked
                    .Any(el => sponsoredProductArray.Contains(el));
                var breakContainingAnyCompetitorsProduct = allProductsBooked
                    .Any(el => competitorsArray.Contains(el));

                Assert.False(
                    breakContainingAnySponsoredProduct && breakContainingAnyCompetitorsProduct,
                    $"TestCode {code} failed. '{br}' contains both SponsoredProduct and Competitors.");
            }
        }

        /// <summary>
        /// Sponsorship.CalculationType: Exclusive No competitor products should
        /// be booked into any sponsored break
        /// </summary>
        /// <param name="recommendations"></param>
        /// <param name="expectedBreaks"></param>
        /// <param name="competitors"></param>
        /// <param name="code"></param>
        protected void AssertRecommendationsAreSetWithSponsorshipRulesForCalculationTypeSetToExclusive(
            IReadOnlyCollection<Recommendation> recommendations,
            string expectedBreaks,
            string competitors,
            string code)
        {
            var pivotRecommendations = recommendations.Pivot(RepositoryFactory);
            var expectedBreakArray = expectedBreaks.Split('|');
            var competitorsArray = competitors.Split('|');

            foreach (var br in expectedBreakArray)
            {
                Assert.True(pivotRecommendations.BreaksVsProductIds.ContainsKey(br));

                var breakContainingAnyCompetitorsProduct = pivotRecommendations
                    .BreaksVsProductIds[br]
                    .Any(el => competitorsArray.Contains(el));

                Assert.False(
                    breakContainingAnyCompetitorsProduct,
                    $"TestCode {code} failed. '{br}' contains Competitors.");
            }
        }

        protected void AssertRecommendationsAreSetWithSponsorshipRulesFor_Percentage_AllCompetitors_SpotCount(
            IReadOnlyCollection<Recommendation> recommendations,
            string expectedBreaks,
            string sponsoredProduct,
            string competitors,
            string code)
        {
            var pivotRecommendations = recommendations.Pivot(RepositoryFactory);
            var expectedBreakArray = expectedBreaks.Split('|');
            var competitorsArray = GetCompetitorsAndExpectedValues(competitors);
            var sponsoredProductArray = sponsoredProduct.Split('|');
            var competitorsTotalSpotCount = 0;
            var sponsoredProductsTotalSpotCount = 0;

            AssertGoldenRuleOfSponsorship(
                pivotRecommendations,
                expectedBreakArray,
                sponsoredProductArray,
                competitorsArray,
                code);

            foreach (var br in expectedBreakArray)
            {
                var allProductsBookedInTheBreak = pivotRecommendations.BreaksVsProductIds[br];

                var competitorsProductInTheBreak = allProductsBookedInTheBreak.Intersect(competitorsArray.Keys);
                competitorsTotalSpotCount += pivotRecommendations.GetCountOfProductInABreak(br, competitorsProductInTheBreak);

                var sponsoredProductInTheBreak = allProductsBookedInTheBreak.Intersect(sponsoredProductArray);
                sponsoredProductsTotalSpotCount += pivotRecommendations.GetCountOfProductInABreak(br, sponsoredProductInTheBreak);
            }

            var restrictionValueInSponsorshipRules = competitorsArray.Values.First().Value;
            var maxSpotCountForCompetitors = GetMaximumAllowedValueForCompetitors(
                restrictionValueInSponsorshipRules,
                sponsoredProductsTotalSpotCount);

            if (maxSpotCountForCompetitors.HasValue)
            {
                Assert.True(competitorsTotalSpotCount <= maxSpotCountForCompetitors.Value,
                    $"TestCode {code} failed. Total spots booked for competitors are exceeding the Sponsorship maximum " +
                    $"value. The maximum value set in Sponsorship is {restrictionValueInSponsorshipRules} percent of sponsored " +
                    $"products for AllCompetitors. The total spot count of the sponsored products is {sponsoredProductsTotalSpotCount}. " +
                    $"The actual number of spots booked for competitors is {competitorsTotalSpotCount}");
            }
        }

        protected void AssertRecommendationsAreSetWithSponsorshipRulesFor_Percentage_AllCompetitors_SpotDuration(
            IReadOnlyCollection<Recommendation> recommendations,
            string expectedBreaks,
            string sponsoredProduct,
            string competitors,
            string code)
        {
            var pivotRecommendations = recommendations.Pivot(RepositoryFactory);
            var expectedBreakArray = expectedBreaks.Split('|');
            var competitorsArray = GetCompetitorsAndExpectedValues(competitors);
            var sponsoredProductArray = sponsoredProduct.Split('|');
            const int spotDurationInSeconds = 15;
            var competitorsTotalSpotDuration = 0;
            var sponsoredProductsTotalSpotDuration = 0;

            AssertGoldenRuleOfSponsorship(pivotRecommendations, expectedBreakArray, sponsoredProductArray,
                competitorsArray, code);

            foreach (var br in expectedBreakArray)
            {
                var allProductsBookedInTheBreak = pivotRecommendations.BreaksVsProductIds[br];
                var competitorsProductInTheBreak = allProductsBookedInTheBreak.Intersect(competitorsArray.Keys);
                var sponsoredProductInTheBreak = allProductsBookedInTheBreak.Intersect(sponsoredProductArray);
                competitorsTotalSpotDuration += pivotRecommendations.GetCountOfProductInABreak(br, competitorsProductInTheBreak) * spotDurationInSeconds;
                sponsoredProductsTotalSpotDuration += pivotRecommendations.GetCountOfProductInABreak(br, sponsoredProductInTheBreak) * spotDurationInSeconds;
            }
            var restrictionValueInSponsorshipRules = competitorsArray.Values.First().Value;
            var maxSpotDurationForCompetitors = GetMaximumAllowedValueForCompetitors(restrictionValueInSponsorshipRules,
                sponsoredProductsTotalSpotDuration);
            if (maxSpotDurationForCompetitors.HasValue)
            {
                Assert.True(competitorsTotalSpotDuration <= maxSpotDurationForCompetitors.Value,
                    $"TestCode {code} failed. Total spots duration booked for competitors are exceeding the Sponsorship " +
                    $"maximum duration value. The maximum duration value set in Sponsorship is {restrictionValueInSponsorshipRules} " +
                    "percent of sponsored product duration for AllCompetitors. The total spot duration of the sponsored products is " +
                    $"{sponsoredProductsTotalSpotDuration} seconds.  The actual number of spots duration booked for competitors is " +
                    $"{competitorsTotalSpotDuration} seconds.");
            }
        }

        protected void AssertRecommendationsAreSetWithSponsorshipRulesFor_Percentage_EachCompetitor_SpotCount(
            IReadOnlyCollection<Recommendation> recommendations,
            string expectedBreaks,
            string sponsoredProduct,
            string competitors,
            string code)
        {
            var pivotRecommendations = recommendations.Pivot(RepositoryFactory);
            var expectedBreakArray = expectedBreaks.Split('|');
            var competitorsArray = GetCompetitorsAndExpectedValues(competitors);
            var sponsoredProductArray = sponsoredProduct.Split('|');

            AssertGoldenRuleOfSponsorship(
                pivotRecommendations,
                expectedBreakArray,
                sponsoredProductArray,
                competitorsArray,
                code);

            foreach (var item in competitorsArray)
            {
                var restrictionValueInSponsorshipRules = item.Value;
                var competitor = item.Key;
                int competitorTotalSpotCount = 0;
                int sponsoredProductsTotalSpotCount = 0;

                foreach (var br in expectedBreakArray)
                {
                    var allProductsBookedInTheBreak = pivotRecommendations.BreaksVsProductIds[br];

                    competitorTotalSpotCount += allProductsBookedInTheBreak.Count(x => x == competitor);

                    sponsoredProductsTotalSpotCount += pivotRecommendations.GetCountOfProductInABreak(br, allProductsBookedInTheBreak.Intersect(sponsoredProductArray));
                }

                var maxSpotCountForCompetitors = GetMaximumAllowedValueForCompetitors(
                    restrictionValueInSponsorshipRules.Value,
                    sponsoredProductsTotalSpotCount);

                if (maxSpotCountForCompetitors.HasValue)
                {
                    Assert.True(competitorTotalSpotCount <= maxSpotCountForCompetitors.Value,
                        $"TestCode {code} failed. Total spots booked for competitor '{competitor}' are exceeding the Sponsorship maximum value. " +
                        $"The maximum value set in Sponsorship is {restrictionValueInSponsorshipRules}  percent of sponsored products count for " +
                        $"EachCompetitors. The total spot count of the sponsored products is {sponsoredProductsTotalSpotCount}. The actual number of " +
                        $"spots count booked for competitors is {competitorTotalSpotCount}.");
                }
            }
        }

        protected void AssertRecommendationsAreSetWithSponsorshipRulesFor_Percentage_EachCompetitor_SpotDuration(
            IReadOnlyCollection<Recommendation> recommendations,
            string expectedBreaks,
            string sponsoredProduct,
            string competitors,
            string code)
        {
            var pivotRecommendations = recommendations.Pivot(RepositoryFactory);
            var expectedBreakArray = expectedBreaks.Split('|');
            var competitorsArray = GetCompetitorsAndExpectedValues(competitors);
            var sponsoredProductArray = sponsoredProduct.Split('|');

            AssertGoldenRuleOfSponsorship(pivotRecommendations, expectedBreakArray, sponsoredProductArray,
                competitorsArray, code);

            foreach (var item in competitorsArray)
            {
                var restrictionValueInSponsorshipRules = item.Value;
                var competitor = item.Key;
                const int spotDurationInSeconds = 15;
                var competitorTotalSpotDuration = 0;
                var sponsoredProductsTotalSpotDuration = 0;
                foreach (var br in expectedBreakArray)
                {
                    var allProductsBookedInTheBreak = pivotRecommendations.BreaksVsProductIds[br];
                    var competitorInTheBreak = allProductsBookedInTheBreak.FindAll(x => x == competitor);
                    var sponsoredProductInTheBreak = allProductsBookedInTheBreak.Intersect(sponsoredProductArray);

                    competitorTotalSpotDuration += pivotRecommendations.GetCountOfProductInABreak(br, competitorInTheBreak) * spotDurationInSeconds;
                    sponsoredProductsTotalSpotDuration += pivotRecommendations.GetCountOfProductInABreak(br, sponsoredProductInTheBreak) * spotDurationInSeconds;
                }

                var maxSpotDuartionForCompetitors = GetMaximumAllowedValueForCompetitors(restrictionValueInSponsorshipRules.Value,
                    sponsoredProductsTotalSpotDuration);
                if (maxSpotDuartionForCompetitors.HasValue)
                {
                    Assert.True(competitorTotalSpotDuration <= maxSpotDuartionForCompetitors.Value,
                        $"TestCode {code} failed. Total spots duration booked for competitor '{competitor}' are exceeding the Sponsorship maximum " +
                        $"value. The maximum value set in Sponsorship is {restrictionValueInSponsorshipRules}  percent of sponsored product duration " +
                        $"for AllCompetitors. The total spot duration of the sponsored products is {sponsoredProductsTotalSpotDuration} seconds. The " +
                        $"actual number of spots duration booked for competitors is {competitorTotalSpotDuration} seconds.");
                }
            }
        }

        protected void AssertRecommendationsAreSetWithSponsorshipRulesFor_Flat_AllCompetitors_SpotCount(
            IReadOnlyCollection<Recommendation> recommendations,
            string expectedBreaks,
            string sponsoredProduct,
            string competitors,
            string code)
        {
            var pivotRecommendations = recommendations.Pivot(RepositoryFactory);
            var expectedBreakArray = expectedBreaks.Split('|');
            var competitorsArray = GetCompetitorsAndExpectedValues(competitors);
            var sponsoredProductArray = sponsoredProduct.Split('|');

            AssertGoldenRuleOfSponsorship(pivotRecommendations, expectedBreakArray,
                sponsoredProductArray, competitorsArray, code);

            int competitorsTotalSpotCount = 0;
            foreach (var br in expectedBreakArray)
            {
                var allProductsBookedInTheBreak = pivotRecommendations.BreaksVsProductIds[br];
                var competitorsProductInTheBreak = allProductsBookedInTheBreak.Intersect(competitorsArray.Keys);
                competitorsTotalSpotCount += pivotRecommendations.GetCountOfProductInABreak(br, competitorsProductInTheBreak);
            }
            var maxSpotCountSetInSponsorshipRules = competitorsArray.Values.First().Value;
            Assert.True(competitorsTotalSpotCount <= maxSpotCountSetInSponsorshipRules,
                $"TestCode {code} failed. Total spots booked for competitors are exceeding the Sponsorship maximum value. The maximum value " +
                $"set in Sponsorship is {maxSpotCountSetInSponsorshipRules} for AllCompetitors. Actual Number of spots booked for competitors is " +
                $"{competitorsTotalSpotCount}");
        }

        protected void AssertRecommendationsAreSetWithSponsorshipRulesFor_Flat_AllCompetitors_SpotDuration(
            IReadOnlyCollection<Recommendation> recommendations,
            string expectedBreaks,
            string sponsoredProduct,
            string competitors,
            string code)
        {
            var pivotRecommendations = recommendations.Pivot(RepositoryFactory);
            var expectedBreakArray = expectedBreaks.Split('|');
            var competitorsArray = GetCompetitorsAndExpectedValues(competitors);
            var sponsoredProductArray = sponsoredProduct.Split('|');
            const int spotDurationInSeconds = 15;

            AssertGoldenRuleOfSponsorship(pivotRecommendations, expectedBreakArray, sponsoredProductArray,
                competitorsArray, code);

            int competitorsTotalSpotDuration = 0;
            foreach (var br in expectedBreakArray)
            {
                var allProductsBookedInTheBreak = pivotRecommendations.BreaksVsProductIds[br];
                var competitorsProductInTheBreak = allProductsBookedInTheBreak.Intersect(competitorsArray.Keys);
                competitorsTotalSpotDuration += pivotRecommendations.GetCountOfProductInABreak(br, competitorsProductInTheBreak) * spotDurationInSeconds;
            }
            var maxSpotDucrationSetInSponsorshipRules = competitorsArray.Values.First().Value;
            Assert.True(competitorsTotalSpotDuration <= maxSpotDucrationSetInSponsorshipRules,
                $"TestCode {code} failed. Total spots duration booked for competitors are exceeding the Sponsorship maximum value. The maximum value " +
                $"set in Sponsorship is {maxSpotDucrationSetInSponsorshipRules} seconds for AllCompetitors. The actual Number of spot duration booked " +
                $"for competitors is {competitorsTotalSpotDuration} seconds.");
        }

        protected void AssertRecommendationsAreSetWithSponsorshipRulesFor_Flat_EachCompetitor_SpotCount(
            IReadOnlyCollection<Recommendation> recommendations,
            string expectedBreaks,
            string sponsoredProduct,
            string competitors,
            string code)
        {
            var pivotRecommendations = recommendations.Pivot(RepositoryFactory);
            var expectedBreakArray = expectedBreaks.Split('|');
            var competitorsArray = GetCompetitorsAndExpectedValues(competitors);
            var sponsoredProductArray = sponsoredProduct.Split('|');

            AssertGoldenRuleOfSponsorship(pivotRecommendations, expectedBreakArray, sponsoredProductArray,
                competitorsArray, code);

            foreach (var item in competitorsArray)
            {
                var maxSpotCountForThisCompetiotrInSponsorshipRules = item.Value;
                var competitor = item.Key;
                int competitorTotalSpotCount = 0;
                foreach (var br in expectedBreakArray)
                {
                    var allProductsBookedInTheBreak = pivotRecommendations.BreaksVsProductIds[br];
                    var competitorInTheBreak = allProductsBookedInTheBreak.FindAll(x => x == competitor);
                    competitorTotalSpotCount += pivotRecommendations.GetCountOfProductInABreak(br, competitorInTheBreak);
                }
                Assert.True(competitorTotalSpotCount <= maxSpotCountForThisCompetiotrInSponsorshipRules,
                    $"TestCode {code} failed. Total spots booked for competitor '{competitor}' are exceeding the Sponsorship maximum value. The maximum value " +
                    $"set in Sponsorship is {maxSpotCountForThisCompetiotrInSponsorshipRules} for EachCompetitor. The actual number of spots booked for these " +
                    $"competitors is {competitorTotalSpotCount}.");
            }
        }

        protected void AssertRecommendationsAreSetWithSponsorshipRulesFor_Flat_EachCompetitor_SpotDuration(
            IReadOnlyCollection<Recommendation> recommendations,
            string expectedBreaks,
            string sponsoredProduct,
            string competitors,
            string code)
        {
            var pivotRecommendations = recommendations.Pivot(RepositoryFactory);
            var expectedBreakArray = expectedBreaks.Split('|');
            var competitorsArray = GetCompetitorsAndExpectedValues(competitors);
            var sponsoredProductArray = sponsoredProduct.Split('|');
            const int spotDurationInSeconds = 15;

            AssertGoldenRuleOfSponsorship(pivotRecommendations, expectedBreakArray, sponsoredProductArray,
                competitorsArray, code);

            foreach (var item in competitorsArray)
            {
                var maxSpotDurationForThisCompetiotrInSponsorshipRules = item.Value;
                var competitor = item.Key;
                int competitorTotalSpotDuration = 0;
                foreach (var br in expectedBreakArray)
                {
                    var allProductsBookedInTheBreak = pivotRecommendations.BreaksVsProductIds[br];
                    var competitorInTheBreak = allProductsBookedInTheBreak.FindAll(x => x == competitor);

                    competitorTotalSpotDuration += pivotRecommendations.GetCountOfProductInABreak(br, competitorInTheBreak) * spotDurationInSeconds;
                }
                Assert.True(competitorTotalSpotDuration <= maxSpotDurationForThisCompetiotrInSponsorshipRules,
                    $"TestCode {code} failed. Total spots duration booked for competitor {competitor} are exceeding the Sponsorship maximum value. " +
                    $"The maximum value set in Sponsorship is {maxSpotDurationForThisCompetiotrInSponsorshipRules} seconds for EachCompetitor. The actual " +
                    $"Number of spot duration booked for this competitor is {competitorTotalSpotDuration} seconds.");
            }
        }

        /// <summary>
        /// If there is a sponsored product in a break no competitor should be
        /// in that break.
        /// </summary>
        /// <param name="pivotRecommendations"></param>
        /// <param name="expectedBreakArray"></param>
        /// <param name="sponsoredProductArray"></param>
        /// <param name="competitorsArray"></param>
        /// <param name="code"></param>
        private void AssertGoldenRuleOfSponsorship(
            PivotRecommendations pivotRecommendations,
            string[] expectedBreakArray,
            string[] sponsoredProductArray,
            Dictionary<string, int?> competitorsArray,
            string code)
        {
            foreach (var br in expectedBreakArray)
            {
                Assert.True(pivotRecommendations.BreaksVsProductIds.ContainsKey(br));

                var allProductsBookedInTheBreak = pivotRecommendations.BreaksVsProductIds[br];
                var sponsoredProductInTheBreak = allProductsBookedInTheBreak.Intersect(sponsoredProductArray);
                var competitorsProductInTheBreak = allProductsBookedInTheBreak.Intersect(competitorsArray.Keys);

                Assert.False(
                    competitorsProductInTheBreak.Any() && sponsoredProductInTheBreak.Any(),
                    $"TestCode {code} failed. '{br}' contains both SponsoredProduct and Competitors.");
            }
        }

        private Dictionary<string, int?> GetCompetitorsAndExpectedValues(
            string competitors)
        {
            var result = new Dictionary<string, int?>();

            foreach (var competitorValuePair in competitors.Split('|'))
            {
                string competitor;
                int? value;

                if (competitorValuePair.Contains(":"))
                {
                    string[] valuePair = competitorValuePair.Split(':');
                    competitor = valuePair[0];
                    var valueStr = valuePair[1];

                    value = Int32.TryParse(valueStr, out int k)
                        ? k :
                        (int?)null;
                }
                else
                {
                    competitor = competitorValuePair;
                    value = null;
                }

                result.Add(competitor, value);
            }

            return result;
        }

        /// <summary>
        /// <para>
        /// Calculation to test Competitors air Time based on the portion of the
        /// sponsored product air time.
        /// </para>
        /// <para>Examples</para>
        /// <list type="bullet">
        /// <item>
        /// <description>
        /// Example 1: if we have booked 20 sponsored products and the
        /// restriction value is 10%, the maximum number of (all/each)
        /// competitors is 2. (20*10/100 = 2)
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// Example 2: if we have booked 19 sponsored products and the
        /// restriction value is 10%, the maximum number of (all/each)
        /// competitors is 2. (19*10/100 = 1.9 = round to closest int = 2)
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// Example 3: if we have booked 14 sponsored products and the
        /// restriction value is 10%, the maximum number of (all/each)
        /// competitors is 1. (14*10/100 = 1.4 = round to closest int = 1)
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// Example 4: if we have booked 9 sponsored products and the
        /// restriction value is 10%, the maximum number of (all/each)
        /// competitors is 0. (9*10/100 = 0.9 = Less than one will be zero = 0)
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// Example 5: if we have booked no sponsored products and the
        /// restriction value is 10%, the maximum number of (all/each)
        /// competitors could be anything. (0*10/100 = 0 = because there is no
        /// sponsored product, there is no rules)
        /// </description>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="restrictionValue">
        /// The percentage of the Sponsored product unit that can be assigned to competitors
        /// </param>
        /// <param name="sponsoredProducts">
        /// the unit is either Spots count or duration of the sponsored product
        /// </param>
        /// <returns></returns>
        private int? GetMaximumAllowedValueForCompetitors(int restrictionValue, int sponsoredProducts)
        {
            //no sponsoredProduct, we don't limit the competitors at all
            if (sponsoredProducts == 0)
            {
                return null;
            }

            if (restrictionValue == 0 || sponsoredProducts == 0)
            {
                return 0;
            }

            double x = sponsoredProducts * (double)restrictionValue / 100;
            if (x.CompareTo(1d) == -1)
            {
                return 0;
            }

            //Rounds a value to the nearest integer
            return (int)Math.Round(x, MidpointRounding.AwayFromZero);
        }

        private int? GetRestrictionValue(string restrictionValue)
        {
            int result;
            if (!string.IsNullOrEmpty(restrictionValue) && Int32.TryParse(restrictionValue, out result))
            {
                return result;
            }
            return null;
        }

        private SponsorshipRestrictionType? GetRestrictionType(string restrictionType)
        {
            SponsorshipRestrictionType result;
            if (!string.IsNullOrEmpty(restrictionType) && Enum.TryParse<SponsorshipRestrictionType>(restrictionType, out result))
            {
                return result;
            }
            return null;
        }
    }
}

﻿using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.CollectionFixtures;
using Xunit;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    [Collection("Smooth - Sponsorship - Flat - Timeband - Each competitor - Spot count")]
    [Trait("Smooth", "XG10/11 Sponsorship - Flat - Timeband - Each competitor - Spot count")]
    public class SponsorshipFlatTimebandEachCompetitorSpotCountTests : SponsorshipRestrictionsTests
    {
        public SponsorshipFlatTimebandEachCompetitorSpotCountTests(
            SmoothFixture smoothFixture,
            ITestOutputHelper output)
            : base(smoothFixture, output, loadAllCoreData: false)
        { }

        [SkippableTheory]
        [MemberData(nameof(SponsorshipTestCasesFor_Flat_TimeBand_EachCompetitor_SpotCount_PositiveTest))]
        internal void GivenTheSponsorshipDataIsProvidedWithCalculationTypeSetToFlatAndRestrictionLevelTimeBandForEachCompetitorWithRestrictionTypeSetToSpotCountSmoothWillPlaceAllSpotsToMatchWithSponsorshipRules_PositiveTests(
            bool skipped,
            string code,
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
            string expectedBreaks,
            string sponsoredProduct,
            string competitors)
        {
            Skip.If(skipped, reason: "Use case marked as skip in data source");

            var sponsorship = GetTestCaseSponsorshipObject(calculationType,
            restrictionLevel, sponsoredItemRestrictionType,
            sponsoredItemRestrictionValue, applicability, programmeName,
            clashExternalRef, clashRestrictionType, clashRestrictionValue,
            advertiserIdentifier, advertiserRestrictionType,
            advertiserRestrictionValue, salesArea, startDate, endDate, startTime,
            endTime, daysOfWeek, sponsoredProduct);

            DataLoader.LoadTestSpecificRepositories(RepositoryFactory,
            Path.Combine("SponsorshipExclusivity", "SharedData"));

            UpdateSpotsPreemptlevelValuesForSponsoredProductToComplySponsorshipObject(sponsoredProduct, expectedBreaks);
            DeleteAllSponsorshipRestrictionsForThisTest();
            AddSponsorshipRestrictionsForThisTest(sponsorship);

            WriteTestCode(code);

            //Act
            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                DumpRecommentationsToDebug(testResults.Recommendations);

                AssertRecommendationsAreSetWithSponsorshipRulesFor_Flat_EachCompetitor_SpotCount(testResults.Recommendations, expectedBreaks, sponsoredProduct, competitors, code);
            }
        }

        [SkippableTheory]
        [MemberData(nameof(SponsorshipTestCasesFor_Flat_TimeBand_EachCompetitor_SpotCount_NegativeTest))]
        internal void GivenTheSponsorshipDataIsProvidedWithCalculationTypeSetToFlatAndRestrictionLevelTimeBandForEachCompetitorWithRestrictionTypeSetToSpotCountSmoothWillPlaceAllSpotsToMatchWithSponsorshipRules_NegativeTests(
          bool skipped,
          string code,
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
          string sponsoredProduct)
        {
            Skip.If(skipped, reason: "Use case marked as skip in data source");

            var sponsorship = GetTestCaseSponsorshipObject(calculationType, restrictionLevel, sponsoredItemRestrictionType, sponsoredItemRestrictionValue,
            applicability, programmeName, clashExternalRef, clashRestrictionType, clashRestrictionValue, advertiserIdentifier, advertiserRestrictionType,
            advertiserRestrictionValue, salesArea, startDate, endDate, startTime, endTime, daysOfWeek, sponsoredProduct);

            DataLoader.LoadTestSpecificRepositories(RepositoryFactory,
            Path.Combine(
            "SponsorshipExclusivity",
            "SharedData"
            )
            );

            DeleteAllSponsorshipRestrictionsForThisTest();
            AddSponsorshipRestrictionsForThisTest(sponsorship);

            WriteTestCode(code);

            //Act
            ActThen(CheckResults);

            // Assert
            void CheckResults(SmoothTestResults testResults)
            {
                DumpRecommentationsToDebug(testResults.Recommendations);

                AssertRecommendationsAreSetWithNoSponsorshipRules(testResults.Recommendations, code);
            }
        }

        public static IEnumerable<object[]> SponsorshipTestCasesFor_Flat_TimeBand_EachCompetitor_SpotCount_NegativeTest =>
            SmoothTest.LoadTestSpecificCsvFile(
                Path.Combine(
                "Sponsorship",
                "TestCaseData-Flat-TimeBand-EachCompetitor-SpotCount-NegativeTest.csv"
                ));

        public static IEnumerable<object[]> SponsorshipTestCasesFor_Flat_TimeBand_EachCompetitor_SpotCount_PositiveTest =>
            SmoothTest.LoadTestSpecificCsvFile(
                Path.Combine(
                "Sponsorship",
                "TestCaseData-Flat-TimeBand-EachCompetitor-SpotCount-PositiveTest.csv"
                ));
    }
}

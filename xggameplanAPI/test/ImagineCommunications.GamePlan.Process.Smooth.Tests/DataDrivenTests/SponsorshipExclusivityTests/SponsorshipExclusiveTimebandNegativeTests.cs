﻿using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.CollectionFixtures;
using Xunit;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    [Collection(nameof(SmoothSponsorshipExclusiveTimebandNegativeCollectionDefinition))]
    [Trait("Smooth", "XG10/11 Sponsorship - Exclusive - Timeband")]
    public class SponsorshipExclusiveTimebandNegativeTests
        : SponsorshipRestrictionsTests
    {
        public SponsorshipExclusiveTimebandNegativeTests(
            SmoothFixture smoothFixture,
            ITestOutputHelper output)
            : base(smoothFixture, output, loadAllCoreData: false)
        { }

        [SkippableTheory]
        [MemberData(nameof(SponsorshipTestCasesFor_CalculationTypeExclusive_RestrictionLevelTimeBand_NegativeTests))]
        internal void
            GivenTheSponsorshipDataIsProvidedWithCalculationTypeSetToExclusiveAndRestrictionLevelTimeBandSmoothWillPlaceAllSpotsToMatchWithSponsorshipRules_NegativeTests(
            bool skipped,
            string code,
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
            string sponsoredProduct)
        {
            Skip.If(skipped, reason: "Use case marked as skip in data source");

            var sponsorship = GetTestCaseSponsorshipObject(
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

        public static IEnumerable<object[]> SponsorshipTestCasesFor_CalculationTypeExclusive_RestrictionLevelTimeBand_NegativeTests =>
            SmoothTest.LoadTestSpecificCsvFile(
                Path.Combine(
                    "Sponsorship",
                    "TestCaseData-Exclusive-TimeBand-NegativeTest.csv"
            ));
    }
}

using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;

namespace xggameplan.core.Services
{
    /// <summary>
    /// Creates SmoothConfiguration instance. It converts the data/functions from the old DefaultSmoothConfiguration class in to just data
    /// so that it can be serialized to the database.
    ///
    /// TODO: Remove this class later. We should make SmoothConfiguration seed data.
    /// </summary>
    public class SmoothConfigurationBuilder
    {
        public SmoothConfiguration Create()
        {
            var smoothConfiguration = new SmoothConfiguration()
            {
                Id = SmoothConfiguration.DefaultId,
                Version = "1.0.0",
                RestrictionCheckEnabled = true,
                ClashExceptionCheckEnabled = true,
                ExternalCampaignRefsToExclude = new List<string>() { "GRID" },
                RecommendationsForExcludedCampaigns = true,
                SmoothFailuresForExcludedCampaigns = false,
                Passes = GetPasses(),
                IterationRecords = GetSmoothPassIterationRecords(),
                BestBreakFactorGroupRecords = GetBestBreakFactorGroupRecords()
            };
            return smoothConfiguration;
        }

        /// <summary>
        /// Returns passes
        /// </summary>
        /// <returns></returns>
        private List<SmoothPass> GetPasses()
        {
            List<SmoothPass> smoothPasses = new List<SmoothPass>();

            // Add default pass 1
            smoothPasses.Add(new SmoothPassDefault(1, null, null, false, null, null, null, null));

            // Add default pass 2
            smoothPasses.Add(new SmoothPassDefault(2, null, null, false, null, null, null, null));

            // Add default pass 3
            smoothPasses.Add(new SmoothPassDefault(3, null, null, false, null, null, null, null));

            // Add default pass 4
            smoothPasses.Add(new SmoothPassDefault(4, null, null, false, null, null, null, null));

            // Add default pass 5 (Product clash)
            smoothPasses.Add(new SmoothPassDefault(5, null, null, false, null, null, true, null));

            // Add default pass 6 (Campaign clash)
            smoothPasses.Add(new SmoothPassDefault(6, null, null, false, null, null, null, null));

            // Add unplaced spot pass 7
            smoothPasses.Add(new SmoothPassUnplaced(7));

            return smoothPasses;
        }

        private List<SmoothPassIterationRecord> GetSmoothPassIterationRecords()
        {
            List<SmoothPassIterationRecord> records = new List<SmoothPassIterationRecord>();

            // Pass 1 (Sponsored)
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 1 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = true, HasBreakRequest = null, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(1001, true, true, ProductClashRules.NoClashes, SpotPositionRules.Exact, SpotPositionRules.Exact, true, true)
            });
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 1 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = true, HasBreakRequest = null, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(1002, true, true, ProductClashRules.NoClashes, SpotPositionRules.Exact, SpotPositionRules.Near, true, true)
            });
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 1 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = true, HasBreakRequest = null, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(1003, true, true, ProductClashRules.NoClashes, SpotPositionRules.Exact, SpotPositionRules.Anywhere, true, true)
            });
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 1 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = true, HasBreakRequest = null, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(1004, true, true, ProductClashRules.LimitOnExposureCount, SpotPositionRules.Exact, SpotPositionRules.Exact, true, true)
            });
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 1 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = true, HasBreakRequest = null, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(1005, true, true, ProductClashRules.LimitOnExposureCount, SpotPositionRules.Exact, SpotPositionRules.Near, true, true)
            });
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 1 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = true, HasBreakRequest = null, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(1006, true, true, ProductClashRules.LimitOnExposureCount, SpotPositionRules.Exact, SpotPositionRules.Anywhere, true, true)
            });

            // Pass 1 (Not sponsored, break request)
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 1 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = false, HasBreakRequest = true, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(1007, true, true, ProductClashRules.NoClashes, SpotPositionRules.Exact, SpotPositionRules.Exact, true, true)
            });
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 1 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = false, HasBreakRequest = true, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(1008, true, true, ProductClashRules.NoClashes, SpotPositionRules.Exact, SpotPositionRules.Near, true, true)
            });
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 1 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = false, HasBreakRequest = true, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(1009, true, true, ProductClashRules.NoClashes, SpotPositionRules.Exact, SpotPositionRules.Anywhere, true, true)
            });

            // Pass 1 (Not sponsored, no break request)
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 1 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = false, HasBreakRequest = false, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(1010, true, true, ProductClashRules.NoClashes, SpotPositionRules.Exact, SpotPositionRules.Exact, true, true)
            });
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 1 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = false, HasBreakRequest = false, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(1011, true, true, ProductClashRules.NoClashes, SpotPositionRules.Exact, SpotPositionRules.Near, true, true)
            });
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 1 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = false, HasBreakRequest = false, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(1012, true, true, ProductClashRules.NoClashes, SpotPositionRules.Exact, SpotPositionRules.Anywhere, true, true)
            });
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 1 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = false, HasBreakRequest = false, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(1013, true, true, ProductClashRules.LimitOnExposureCount, SpotPositionRules.Exact, SpotPositionRules.Exact, true, true)
            });
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 1 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = false, HasBreakRequest = false, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(1014, true, true, ProductClashRules.LimitOnExposureCount, SpotPositionRules.Exact, SpotPositionRules.Near, true, true)
            });
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 1 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = false, HasBreakRequest = false, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(1015, true, true, ProductClashRules.LimitOnExposureCount, SpotPositionRules.Exact, SpotPositionRules.Anywhere, true, true)
            });

            // Pass 2
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 2 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(2001, true, true, ProductClashRules.LimitOnExposureCount, SpotPositionRules.Near, SpotPositionRules.Exact, true, true)
            });
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 2 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(2002, true, true, ProductClashRules.LimitOnExposureCount, SpotPositionRules.Near, SpotPositionRules.Near, true, true)
            });
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 2 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(2003, true, true, ProductClashRules.LimitOnExposureCount, SpotPositionRules.Near, SpotPositionRules.Anywhere, true, true)
            });

            // Pass 3
            // Respect spot times but any break
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 3 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(3001, true, true, ProductClashRules.LimitOnExposureCount, SpotPositionRules.Anywhere, SpotPositionRules.Exact, true, true)
            });
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 3 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(3002, true, true, ProductClashRules.LimitOnExposureCount, SpotPositionRules.Anywhere, SpotPositionRules.Near, true, true)
            });
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 3 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(3003, true, true, ProductClashRules.LimitOnExposureCount, SpotPositionRules.Anywhere, SpotPositionRules.Anywhere, true, true)
            });

            // Exact break requested
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 3 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(3004, false, true, ProductClashRules.LimitOnExposureCount, SpotPositionRules.Exact, SpotPositionRules.Exact, true, true)
            });
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 3 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(3005, false, true, ProductClashRules.LimitOnExposureCount, SpotPositionRules.Exact, SpotPositionRules.Near, true, true)
            });
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 3 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(3006, false, true, ProductClashRules.LimitOnExposureCount, SpotPositionRules.Exact, SpotPositionRules.Anywhere, true, true)
            });

            // Near to exact break requested
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 3 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(3007, false, true, ProductClashRules.LimitOnExposureCount, SpotPositionRules.Near, SpotPositionRules.Exact, true, true)
            });
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 3 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(3008, false, true, ProductClashRules.LimitOnExposureCount, SpotPositionRules.Near, SpotPositionRules.Near, true, true)
            });
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 3 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(3009, false, true, ProductClashRules.LimitOnExposureCount, SpotPositionRules.Near, SpotPositionRules.Anywhere, true, true)
            });

            // Pass 4
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 4 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(4001, false, true, ProductClashRules.LimitOnExposureCount, SpotPositionRules.Anywhere, SpotPositionRules.Exact, true, true)
            });
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 4 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(4002, false, true, ProductClashRules.LimitOnExposureCount, SpotPositionRules.Anywhere, SpotPositionRules.Near, true, true)
            });
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 4 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(4003, false, true, ProductClashRules.LimitOnExposureCount, SpotPositionRules.Anywhere, SpotPositionRules.Anywhere, true, true)
            });

            // Pass 5 (Product clash interations)
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 5 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(5001, true, true, ProductClashRules.NoClashLimit, SpotPositionRules.Anywhere, SpotPositionRules.Anywhere, true, false)
            });
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 5 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(5002, false, true, ProductClashRules.NoClashLimit, SpotPositionRules.Anywhere, SpotPositionRules.Anywhere, true, false)
            });

            // Pass 6 (Campaign clash iterations)
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 6 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(6001, true, false, ProductClashRules.LimitOnExposureCount, SpotPositionRules.Anywhere, SpotPositionRules.Anywhere, true, false)
            });
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 6 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(6002, false, false, ProductClashRules.LimitOnExposureCount, SpotPositionRules.Anywhere, SpotPositionRules.Anywhere, true, false)
            });
            records.Add(new SmoothPassIterationRecord()   // Relax campaign & product clash rules rather than just one of them
            {
                PassSequences = new List<int>() { 6 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = null },
                PassDefaultIteration = new SmoothPassDefaultIteration(6003, false, false, ProductClashRules.NoClashLimit, SpotPositionRules.Anywhere, SpotPositionRules.Anywhere, true, false)
            });

            // Pass 7 (Unplaced spots)
            records.Add(new SmoothPassIterationRecord()
            {
                PassSequences = new List<int>() { 7 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = null },
                PassUnplacedIteration = new SmoothPassUnplacedIteration(7001, false, false, ProductClashRules.NoClashLimit, true, false)
            });

            return records;
        }

        private List<BestBreakFactorGroupRecord> GetBestBreakFactorGroupRecords()
        {
            List<BestBreakFactorGroupRecord> records = new List<BestBreakFactorGroupRecord>();

            // If no break request and no other spots for sponsor then place in first break
            records.Add(new BestBreakFactorGroupRecord()
            {
                PassSequences = new List<int>() { 1, 2, 3, 4 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = true, HasBreakRequest = null, HasFIBORLIBRequests = null },
                BestBreakFactorGroup = new BestBreakFactorGroup(1001, "1A", BestBreakFactorGroupEvaluation.MaxScore,
                            SameBreakGroupScoreActions.UseSingleBreakFactorIfBestScoreIsNonZero, new BestBreakFactor(1, BestBreakFactors.BreaksInAscendingPosition),
                        new List<BestBreakFactorGroupItem>()
                        {
                            new BestBreakFactorGroupItem(BestBreakFactorItemEvaluation.TotalScore, true, true,
                            new List<BestBreakFactor>() {
                                new BestBreakFactor(1, BestBreakFactors.IsSponsoredSpotBeingPlacedWithNoBreakPosition),
                                new BestBreakFactor(2, BestBreakFactors.IsNoBreaksHaveSpotsForSameSponsor),
                                new BestBreakFactor(3, BestBreakFactors.IsFirstBreak)
                            },
                            new List<BestBreakFactor>() {
                                new BestBreakFactor(1, BestBreakFactors.SameNonZeroScoreForAllBreaks)     // Dummy, ensures that total score is non-zero
                            } )
                        })
            });

            // If no break request on spot and other spots for sponsor then place in last break
            records.Add(new BestBreakFactorGroupRecord()
            {
                PassSequences = new List<int>() { 1, 2, 3, 4 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = true, HasBreakRequest = null, HasFIBORLIBRequests = null },
                BestBreakFactorGroup = new BestBreakFactorGroup(1002, "1B", BestBreakFactorGroupEvaluation.MaxScore,
                          SameBreakGroupScoreActions.UseSingleBreakFactorIfBestScoreIsNonZero, new BestBreakFactor(1, BestBreakFactors.BreaksInDescendingPosition),
                      new List<BestBreakFactorGroupItem>()
                      {
                            new BestBreakFactorGroupItem(BestBreakFactorItemEvaluation.TotalScore, true, true,
                            new List<BestBreakFactor>() {
                                new BestBreakFactor(1, BestBreakFactors.IsSponsoredSpotBeingPlacedWithNoBreakPosition),
                                new BestBreakFactor(2, BestBreakFactors.IsAnyBreaksHaveSpotsForSameSponsor),
                                new BestBreakFactor(3, BestBreakFactors.IsLastBreak)
                            },
                            new List<BestBreakFactor>() {
                                new BestBreakFactor(1, BestBreakFactors.SameNonZeroScoreForAllBreaks)     // Dummy, ensures that total score is non-zero
                            } )
                      })
            });

            // If no break request and couldn't be placed above (E.g. Insufficient room in first break) then place in last break. We could actually ommit
            // group 1B above but we'll leave it in for diagnostics.
            records.Add(new BestBreakFactorGroupRecord()
            {
                PassSequences = new List<int>() { 1, 2, 3, 4 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = true, HasBreakRequest = null, HasFIBORLIBRequests = null },
                BestBreakFactorGroup = new BestBreakFactorGroup(1003, "1C", BestBreakFactorGroupEvaluation.MaxScore,
                          SameBreakGroupScoreActions.UseSingleBreakFactorIfBestScoreIsNonZero, new BestBreakFactor(1, BestBreakFactors.BreaksInAscendingPosition),
                      new List<BestBreakFactorGroupItem>()
                      {
                            new BestBreakFactorGroupItem(BestBreakFactorItemEvaluation.TotalScore, true, true,
                            new List<BestBreakFactor>() {
                                new BestBreakFactor(1, BestBreakFactors.IsSponsoredSpotBeingPlacedWithNoBreakPosition),
                                new BestBreakFactor(2, BestBreakFactors.IsLastBreak)
                            },
                            new List<BestBreakFactor>() {
                                new BestBreakFactor(1, BestBreakFactors.SameNonZeroScoreForAllBreaks)     // Dummy, ensures that total score is non-zero
                            } )
                      })
            });

            // Has FIB/LIB requests, try and even them out
            records.Add(new BestBreakFactorGroupRecord()
            {
                PassSequences = new List<int>() { 1, 2, 3, 4 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = true },
                BestBreakFactorGroup = new BestBreakFactorGroup(2001, "2A", BestBreakFactorGroupEvaluation.MaxScore,
                         SameBreakGroupScoreActions.UseSingleBreakFactorIfBestScoreIsNonZero, new BestBreakFactor(1, BestBreakFactors.RandomBreak),
                     new List<BestBreakFactorGroupItem>()
                     {
                            new BestBreakFactorGroupItem(BestBreakFactorItemEvaluation.TotalScore, true, true,
                            new List<BestBreakFactor>() {
                                new BestBreakFactor(1, BestBreakFactors.IsLeaves15SecBreakMultipleDurations)
                            },
                            new List<BestBreakFactor>() {
                                new BestBreakFactor(1, BestBreakFactors.FewestRequestedPositionInBreakRequestsFirstOrLastOnly),
                                new BestBreakFactor(2, BestBreakFactors.FewestCampaignAndProductClashes),
                                new BestBreakFactor(3, BestBreakFactors.SpotDurationBalance),
                                new BestBreakFactor(4, BestBreakFactors.FewestSpotsInBreak)
                            } )
                     })
            });

            records.Add(new BestBreakFactorGroupRecord()
            {
                PassSequences = new List<int>() { 1, 2, 3, 4 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = true },
                BestBreakFactorGroup = new BestBreakFactorGroup(2002, "2B", BestBreakFactorGroupEvaluation.MaxScore,
                        SameBreakGroupScoreActions.UseSingleBreakFactorIfBestScoreIsNonZero, new BestBreakFactor(1, BestBreakFactors.RandomBreak),
                    new List<BestBreakFactorGroupItem>()
                    {
                            new BestBreakFactorGroupItem(BestBreakFactorItemEvaluation.TotalScore, true, true,
                            new List<BestBreakFactor>() {
                                new BestBreakFactor(1, BestBreakFactors.IsFillsBreakDuration)
                            },
                            new List<BestBreakFactor>() {
                                new BestBreakFactor(1, BestBreakFactors.FewestRequestedPositionInBreakRequestsFirstOrLastOnly),
                                new BestBreakFactor(2, BestBreakFactors.FewestCampaignAndProductClashes),
                                new BestBreakFactor(3, BestBreakFactors.SpotDurationBalance),
                                new BestBreakFactor(4, BestBreakFactors.FewestSpotsInBreak)
                            } )
                    })
            });

            records.Add(new BestBreakFactorGroupRecord()
            {
                PassSequences = new List<int>() { 1, 2, 3, 4 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = true },
                BestBreakFactorGroup = new BestBreakFactorGroup(2003, "2C", BestBreakFactorGroupEvaluation.MaxScore,
                        SameBreakGroupScoreActions.UseSingleBreakFactorIfBestScoreIsNonZero, new BestBreakFactor(1, BestBreakFactors.RandomBreak),
                    new List<BestBreakFactorGroupItem>()
                    {
                            new BestBreakFactorGroupItem(BestBreakFactorItemEvaluation.TotalScore, true, true,
                            new List<BestBreakFactor>() { },
                            new List<BestBreakFactor>() {
                                new BestBreakFactor(1, BestBreakFactors.FewestRequestedPositionInBreakRequestsFirstOrLastOnly),
                                new BestBreakFactor(2, BestBreakFactors.FewestCampaignAndProductClashes),
                                new BestBreakFactor(3, BestBreakFactors.SpotDurationBalance),
                                new BestBreakFactor(4, BestBreakFactors.FewestSpotsInBreak)
                            } )
                    })
            });

            // Add other groups (No FIB/LIB request)
            records.Add(new BestBreakFactorGroupRecord()
            {
                PassSequences = new List<int>() { 1, 2, 3, 4 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = false },
                BestBreakFactorGroup = new BestBreakFactorGroup(3001, "3A", BestBreakFactorGroupEvaluation.MaxScore,
                     SameBreakGroupScoreActions.UseSingleBreakFactorIfBestScoreIsNonZero, new BestBreakFactor(1, BestBreakFactors.RandomBreak),
                 new List<BestBreakFactorGroupItem>()
                 {
                            new BestBreakFactorGroupItem(BestBreakFactorItemEvaluation.TotalScore, true, true,
                            new List<BestBreakFactor>() {
                                new BestBreakFactor(1, BestBreakFactors.IsLeavesDefaultBreakMultipleDurations),
                            },
                            new List<BestBreakFactor>() {
                                new BestBreakFactor(1, BestBreakFactors.FewestCampaignAndProductClashes),
                                new BestBreakFactor(2, BestBreakFactors.SpotDurationBalance),
                                new BestBreakFactor(3, BestBreakFactors.FewestSpotsInBreak)
                            } )
                 })
            });

            records.Add(new BestBreakFactorGroupRecord()
            {
                PassSequences = new List<int>() { 1, 2, 3, 4 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = false },
                BestBreakFactorGroup = new BestBreakFactorGroup(3002, "3B", BestBreakFactorGroupEvaluation.MaxScore,
                    SameBreakGroupScoreActions.UseSingleBreakFactorIfBestScoreIsNonZero, new BestBreakFactor(1, BestBreakFactors.RandomBreak),
                new List<BestBreakFactorGroupItem>()
                {
                            new BestBreakFactorGroupItem(BestBreakFactorItemEvaluation.TotalScore, true, true,
                            new List<BestBreakFactor>() {
                                new BestBreakFactor(1, BestBreakFactors.IsFillsBreakDuration),
                            },
                            new List<BestBreakFactor>() {
                                new BestBreakFactor(1, BestBreakFactors.FewestCampaignAndProductClashes),
                                new BestBreakFactor(2, BestBreakFactors.SpotDurationBalance),
                                new BestBreakFactor(3, BestBreakFactors.FewestSpotsInBreak)
                            } )
                })
            });

            records.Add(new BestBreakFactorGroupRecord()
            {
                PassSequences = new List<int>() { 1, 2, 3, 4 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = false },
                BestBreakFactorGroup = new BestBreakFactorGroup(3003, "3C", BestBreakFactorGroupEvaluation.MaxScore,
                   SameBreakGroupScoreActions.UseSingleBreakFactorIfBestScoreIsNonZero, new BestBreakFactor(1, BestBreakFactors.RandomBreak),
               new List<BestBreakFactorGroupItem>()
               {
                            new BestBreakFactorGroupItem(BestBreakFactorItemEvaluation.TotalScore, true, true,
                            new List<BestBreakFactor>() { },
                            new List<BestBreakFactor>() {
                                new BestBreakFactor(1, BestBreakFactors.FewestCampaignAndProductClashes),
                                new BestBreakFactor(2, BestBreakFactors.SpotDurationBalance),
                                new BestBreakFactor(3, BestBreakFactors.FewestSpotsInBreak)
                            } )
               })
            });

            // Pass 5 (Product clash)
            records.Add(new BestBreakFactorGroupRecord()
            {
                PassSequences = new List<int>() { 5 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = false },
                BestBreakFactorGroup = new BestBreakFactorGroup(4001, "4A", BestBreakFactorGroupEvaluation.MaxScore,
                  SameBreakGroupScoreActions.UseSingleBreakFactorIfBestScoreIsNonZero, new BestBreakFactor(1, BestBreakFactors.RandomBreak),
              new List<BestBreakFactorGroupItem>()
              {
                            new BestBreakFactorGroupItem(BestBreakFactorItemEvaluation.TotalScore, true, true,
                            new List<BestBreakFactor>() {
                                new BestBreakFactor(1, BestBreakFactors.IsLeavesDefaultBreakMultipleDurations),
                            },
                            new List<BestBreakFactor>() {
                                new BestBreakFactor(1, BestBreakFactors.FewestProductClashesAtChildLevel),
                                new BestBreakFactor(2, BestBreakFactors.SpotDurationBalance),
                                new BestBreakFactor(3, BestBreakFactors.FewestSpotsInBreak)
                            } )
              })
            });

            records.Add(new BestBreakFactorGroupRecord()
            {
                PassSequences = new List<int>() { 5 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = false },
                BestBreakFactorGroup = new BestBreakFactorGroup(4002, "4B", BestBreakFactorGroupEvaluation.MaxScore,
                SameBreakGroupScoreActions.UseSingleBreakFactorIfBestScoreIsNonZero, new BestBreakFactor(1, BestBreakFactors.RandomBreak),
                new List<BestBreakFactorGroupItem>()
                {
                            new BestBreakFactorGroupItem(BestBreakFactorItemEvaluation.TotalScore, true, true,
                            new List<BestBreakFactor>() {
                                new BestBreakFactor(1, BestBreakFactors.IsFillsBreakDuration),
                            },
                            new List<BestBreakFactor>() {
                                new BestBreakFactor(1, BestBreakFactors.FewestProductClashesAtChildLevel),
                                new BestBreakFactor(2, BestBreakFactors.SpotDurationBalance),
                                new BestBreakFactor(3, BestBreakFactors.FewestSpotsInBreak)
                            } )
                })
            });

            records.Add(new BestBreakFactorGroupRecord()
            {
                PassSequences = new List<int>() { 5 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = false },
                BestBreakFactorGroup = new BestBreakFactorGroup(4003, "4C", BestBreakFactorGroupEvaluation.MaxScore,
                SameBreakGroupScoreActions.UseSingleBreakFactorIfBestScoreIsNonZero, new BestBreakFactor(1, BestBreakFactors.RandomBreak),
                new List<BestBreakFactorGroupItem>()
                {
                                new BestBreakFactorGroupItem(BestBreakFactorItemEvaluation.TotalScore, true, true,
                                new List<BestBreakFactor>() { },
                                new List<BestBreakFactor>() {
                                    new BestBreakFactor(1, BestBreakFactors.FewestProductClashesAtChildLevel),
                                    new BestBreakFactor(2, BestBreakFactors.SpotDurationBalance),
                                    new BestBreakFactor(3, BestBreakFactors.FewestSpotsInBreak)
                                } )
                })
            });

            // Pass 6 (Campaign clash)
            records.Add(new BestBreakFactorGroupRecord()
            {
                PassSequences = new List<int>() { 6 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = false },
                BestBreakFactorGroup = new BestBreakFactorGroup(5001, "5A", BestBreakFactorGroupEvaluation.MaxScore,
               SameBreakGroupScoreActions.UseSingleBreakFactorIfBestScoreIsNonZero, new BestBreakFactor(1, BestBreakFactors.RandomBreak),
               new List<BestBreakFactorGroupItem>()
               {
                                new BestBreakFactorGroupItem(BestBreakFactorItemEvaluation.TotalScore, true, true,
                                new List<BestBreakFactor>() {
                                    new BestBreakFactor(1, BestBreakFactors.IsLeavesDefaultBreakMultipleDurations),
                                },
                                new List<BestBreakFactor>() {
                                    new BestBreakFactor(1, BestBreakFactors.FewestCampaignAndProductClashes),   // We're relaxing campaign & product rules for some iterations
                                    new BestBreakFactor(2, BestBreakFactors.SpotDurationBalance),
                                    new BestBreakFactor(3, BestBreakFactors.FewestSpotsInBreak)
                                } )
               })
            });

            records.Add(new BestBreakFactorGroupRecord()
            {
                PassSequences = new List<int>() { 6 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = false },
                BestBreakFactorGroup = new BestBreakFactorGroup(5002, "5B", BestBreakFactorGroupEvaluation.MaxScore,
                  SameBreakGroupScoreActions.UseSingleBreakFactorIfBestScoreIsNonZero, new BestBreakFactor(1, BestBreakFactors.RandomBreak),
                  new List<BestBreakFactorGroupItem>()
                  {
                                    new BestBreakFactorGroupItem(BestBreakFactorItemEvaluation.TotalScore, true, true,
                                    new List<BestBreakFactor>() {
                                        new BestBreakFactor(1, BestBreakFactors.IsFillsBreakDuration),
                                    },
                                    new List<BestBreakFactor>() {
                                        new BestBreakFactor(1, BestBreakFactors.FewestCampaignAndProductClashes),   // We're relaxing campaign & product rules for some iterations
                                        new BestBreakFactor(2, BestBreakFactors.SpotDurationBalance),
                                        new BestBreakFactor(3, BestBreakFactors.FewestSpotsInBreak)
                                    } )
                  })
            });

            records.Add(new BestBreakFactorGroupRecord()
            {
                PassSequences = new List<int>() { 6 },
                SpotsCriteria = new SpotsCriteria() { HasSponsoredSpots = null, HasBreakRequest = null, HasFIBORLIBRequests = false },
                BestBreakFactorGroup = new BestBreakFactorGroup(5003, "5C", BestBreakFactorGroupEvaluation.MaxScore,
                  SameBreakGroupScoreActions.UseSingleBreakFactorIfBestScoreIsNonZero, new BestBreakFactor(1, BestBreakFactors.RandomBreak),
                  new List<BestBreakFactorGroupItem>()
                  {
                                    new BestBreakFactorGroupItem(BestBreakFactorItemEvaluation.TotalScore, true, true,
                                    new List<BestBreakFactor>() { },
                                    new List<BestBreakFactor>() {
                                        new BestBreakFactor(1, BestBreakFactors.FewestCampaignAndProductClashes),   // We're relaxing campaign & product rules for some iterations
                                        new BestBreakFactor(2, BestBreakFactors.SpotDurationBalance),
                                        new BestBreakFactor(3, BestBreakFactors.FewestSpotsInBreak)
                                    } )
                  })
            });

            return records;
        }
    }
}

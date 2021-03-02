using System.Collections.Generic;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using NUnit.Framework;
using xggameplan.KPIProcessing.KPICalculation;

namespace xggameplan.core.tests.KPIProcessing
{
    [TestFixture]
    public class AnalysisGroupCampaignKPIProcessingTests
    {
        private const string BookedItem = "B";
        private const string CancelledItem = "C";
        private List<Campaign> _campaigns;
        private List<Recommendation> _recommendations;

        [SetUp]
        public void Init()
        {
            _campaigns = new List<Campaign>
            {
                new Campaign
                {
                    ExternalId = "ExternalId1",
                    RevenueBudget = 100,
                    TargetRatings = 1000,
                    ActualRatings = 100,
                    SalesAreaCampaignTarget = new List<SalesAreaCampaignTarget>() {
                        new SalesAreaCampaignTarget() {
                            CampaignTargets = new List<CampaignTarget>(){
                                new CampaignTarget() {
                                    StrikeWeights =new List<StrikeWeight>() {
                                        new StrikeWeight() {
                                            DayParts =new List<DayPart>() {
                                                new DayPart() {
                                                    TotalSpotCount = 50,
                                                    ZeroRatedSpotCount = 0,
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    RevenueBooked = 100
                },
                new Campaign
                {
                    ExternalId = "ExternalId2",
                    RevenueBudget = 200,
                    TargetRatings = 2000,
                    ActualRatings = 200,
                    SalesAreaCampaignTarget = new List<SalesAreaCampaignTarget>() {
                        new SalesAreaCampaignTarget() {
                            CampaignTargets = new List<CampaignTarget>(){
                                new CampaignTarget() {
                                    StrikeWeights =new List<StrikeWeight>() {
                                        new StrikeWeight() {
                                            DayParts =new List<DayPart>() {
                                                new DayPart() {
                                                    TotalSpotCount = 100,
                                                    ZeroRatedSpotCount = 0,
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    RevenueBooked = 50
                },
            };

            _recommendations = new List<Recommendation>
            {
                new Recommendation
                {
                    ExternalCampaignNumber = _campaigns[0].ExternalId,
                    SpotRating = 100,
                    Action = BookedItem,
                    NominalPrice = 100
                },
                new Recommendation
                {
                    ExternalCampaignNumber = _campaigns[0].ExternalId,
                    SpotRating = 50,
                    Action = CancelledItem,
                    NominalPrice = 50
                },
                new Recommendation
                {
                    ExternalCampaignNumber = _campaigns[1].ExternalId,
                    SpotRating = 50,
                    Action = BookedItem,
                    NominalPrice = 200
                },
            };
        }

        [Test]
        [Description("Perform analysis group campaign KPI calculation should return AnalysisGroupCampaignKPI")]
        public void PerformAnalysisGroupCampaignKPICalculation_ShouldReturnAnalysisGroupCampaignKPI()
        {
            //Arrange
            var expected = new AnalysisGroupCampaignKPI
            {
                RatingsDelivery = 400,
                DeliveryPercentage = 13.33,
                RevenueBooked = 400,
                PoolValue = 100,
                Spots = 151,
                ZeroRatedSpots = 0
            };

            //Act
            var result = GenerateKPIs();

            //Assert
            _ = result.Should().BeOfType<AnalysisGroupCampaignKPI>();
            result.Should().BeEquivalentTo(expected, becauseArgs: null);
        }

        [Test]
        [Description("Perform analysis group campaign KPI calculation and some recommendations are zero rated should return AnalysisGroupCampaignKPI with zero rated spots")]
        public void PerformAnalysisGroupCampaignKPIsCalculation_AndSomeRecommendationsAreZeroRated_ShouldReturnAnalysisGroupCampaignKPI_WithZeroRatedSpots()
        {
            //Arrange
            _recommendations[0].SpotRating = 0;
            var expected = new AnalysisGroupCampaignKPI
            {
                RatingsDelivery = 300,
                DeliveryPercentage = 10.00,
                RevenueBooked = 400,
                PoolValue = 100,
                Spots = 151,
                ZeroRatedSpots = 1
            };

            //Act
            var result = GenerateKPIs();

            //Assert
            result.Should().BeEquivalentTo(expected, becauseArgs: null);
        }

        [Test]
        [Description("Perform analysis group campaign KPI calculation and all recommendations are booked should return AnalysisGroupCampaignKPI with increased spots")]
        public void PerformAnalysisGroupCampaignKPIsCalculation_AndAllRecommendationsAreBooked_ShouldReturnAnalysisGroupCampaignKPI_WithIncreasedSpots()
        {
            //Arrange
            _recommendations[1].Action = BookedItem;
            var expected = new AnalysisGroupCampaignKPI
            {
                RatingsDelivery = 500,
                DeliveryPercentage = 16.67,
                RevenueBooked = 500,
                PoolValue = 200,
                Spots = 153,
                ZeroRatedSpots = 0
            };

            //Act
            var result = GenerateKPIs();

            //Assert
            result.Should().BeEquivalentTo(expected, becauseArgs: null);
        }

        private AnalysisGroupCampaignKPI GenerateKPIs() => AnalysisGroupKPIsCalculator.CalculateAnalysisGroupCampaignKPIs(_campaigns, _recommendations);
    }
}

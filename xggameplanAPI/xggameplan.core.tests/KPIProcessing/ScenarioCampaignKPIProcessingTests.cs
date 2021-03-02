using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignMetrics.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using NUnit.Framework;
using xggameplan.KPIProcessing.KPICalculation;

namespace xggameplan.core.tests.KPIProcessing
{
    [TestFixture]
    public class ScenarioCampaignKPIProcessingTests
    {
        private const string BookedItem = "B";
        private List<CampaignReducedModel> _campaigns;
        private List<Recommendation> _recommendations;
        private List<Spot> _spots;

        [SetUp]
        public void Init()
        {
            _campaigns = new List<CampaignReducedModel>
            {
                new CampaignReducedModel
                {
                    ExternalId = "ExternalId1",
                    RevenueBudget = 1000
                },
                new CampaignReducedModel
                {
                    ExternalId = "ExternalId2",
                    RevenueBudget = 2000
                },
                new CampaignReducedModel
                {
                    ExternalId = "ExternalId3",
                    RevenueBudget = 3000
                }
            };

            _recommendations = new List<Recommendation>
            {
                new Recommendation
                {
                    ExternalCampaignNumber = _campaigns[0].ExternalId,
                    SpotRating = 10,
                    Action = BookedItem,
                    NominalPrice = 1000
                },
                new Recommendation
                {
                    ExternalCampaignNumber = _campaigns[0].ExternalId,
                    SpotRating = 10,
                    Action = BookedItem,
                    NominalPrice = 3000
                },
                new Recommendation
                {
                    ExternalCampaignNumber = _campaigns[1].ExternalId,
                    SpotRating = 20,
                    Action = BookedItem,
                    NominalPrice = 10000
                },
                new Recommendation
                {
                    ExternalCampaignNumber = _campaigns[2].ExternalId,
                    SpotRating = 15,
                    Action = BookedItem,
                    NominalPrice = 20000
                }
            };

            _spots = new List<Spot>
            {
                new Spot
                {
                    ExternalCampaignNumber = _campaigns[0].ExternalId,
                    ExternalBreakNo = "ExternalBreakNo1",
                    ClientPicked = false,
                    NominalPrice = 2000
                },
                new Spot
                {
                    ExternalCampaignNumber = _campaigns[1].ExternalId,
                    ExternalBreakNo = "ExternalBreakNo2",
                    ClientPicked = false,
                    NominalPrice = 3000
                },
                new Spot
                {
                    ExternalCampaignNumber = _campaigns[2].ExternalId,
                    ExternalBreakNo = "ExternalBreakNo3",
                    ClientPicked = false,
                    NominalPrice = 1000
                },
                new Spot
                {
                    ExternalCampaignNumber = _campaigns[2].ExternalId,
                    ExternalBreakNo = "ExternalBreakNo4",
                    ClientPicked = false,
                    NominalPrice = 4000
                }
            };
        }

        [Test]
        [Description("Perform Scenario Campaign KPIs calculation should return list of Scenario Campaign Metric items")]
        public void PerformScenarioCampaignKPIsCalculation_ShouldReturnListOfScenarioCampaignMetricItems()
        {
            //Arrange
            var expected = new List<ScenarioCampaignMetricItem>
            {
                new ScenarioCampaignMetricItem
                {
                    CampaignExternalId = _campaigns[0].ExternalId,
                    TotalSpots = 2,
                    ZeroRatedSpots = 0,
                    NominalValue = 4000,
                    TotalNominalValue = 6000,
                    DifferenceValueDelivered = 5000,
                    DifferenceValueDeliveredPercentage = 5
                },
                new ScenarioCampaignMetricItem
                {
                    CampaignExternalId = _campaigns[1].ExternalId,
                    TotalSpots = 1,
                    ZeroRatedSpots = 0,
                    NominalValue = 10000,
                    TotalNominalValue = 13000,
                    DifferenceValueDelivered = 11000,
                    DifferenceValueDeliveredPercentage = 5.5
                },
                new ScenarioCampaignMetricItem
                {
                    CampaignExternalId = _campaigns[2].ExternalId,
                    TotalSpots = 1,
                    ZeroRatedSpots = 0,
                    NominalValue = 20000,
                    TotalNominalValue = 25000,
                    DifferenceValueDelivered = 22000,
                    DifferenceValueDeliveredPercentage = 7.33
                }
            };

            //Act
            var result = GenerateCampaignKPIs();

            //Assert
            Assert.IsTrue(CheckIfEqualLists(expected, result));
        }

        [Test]
        [Description("Perform Scenario Campaign KPIs calculation and some Spots are Unplaced should return list of Scenario Campaign Metric items without Unplaced Spots")]
        public void PerformScenarioCampaignKPIsCalculation_AndSomeSpotsAreUnplaced_ShouldReturnListOfScenarioCampaignMetricItemsWithoutUnplacedSpots()
        {
            //Arrange
            _spots[2].ExternalBreakNo = Globals.UnplacedBreakString;

            var expected = new List<ScenarioCampaignMetricItem>
            {
                new ScenarioCampaignMetricItem
                {
                    CampaignExternalId = _campaigns[0].ExternalId,
                    TotalSpots = 2,
                    ZeroRatedSpots = 0,
                    NominalValue = 4000,
                    TotalNominalValue = 6000,
                    DifferenceValueDelivered = 5000,
                    DifferenceValueDeliveredPercentage = 5
                },
                new ScenarioCampaignMetricItem
                {
                    CampaignExternalId = _campaigns[1].ExternalId,
                    TotalSpots = 1,
                    ZeroRatedSpots = 0,
                    NominalValue = 10000,
                    TotalNominalValue = 13000,
                    DifferenceValueDelivered = 11000,
                    DifferenceValueDeliveredPercentage = 5.5
                },
                new ScenarioCampaignMetricItem
                {
                    CampaignExternalId = _campaigns[2].ExternalId,
                    TotalSpots = 1,
                    ZeroRatedSpots = 0,
                    NominalValue = 20000,
                    TotalNominalValue = 24000,
                    DifferenceValueDelivered = 21000,
                    DifferenceValueDeliveredPercentage = 7
                }
            };

            //Act
            var result = GenerateCampaignKPIs();

            //Assert
            Assert.IsTrue(CheckIfEqualLists(expected, result));
        }

        [Test]
        [Description("Perform Scenario Campaign KPIs calculation and some Spots are Zero Rated should return list of Scenario Campaign Metric items with Zero Rated Spots")]
        public void PerformScenarioCampaignKPIsCalculation_AndSomeSpotsAreZeroRated_ShouldReturnListOfScenarioCampaignMetricItemsWithZeroRatedSpots()
        {
            //Arrange
            _recommendations[1].SpotRating = 0;

            var expected = new List<ScenarioCampaignMetricItem>
            {
                new ScenarioCampaignMetricItem
                {
                    CampaignExternalId = _campaigns[0].ExternalId,
                    TotalSpots = 2,
                    ZeroRatedSpots = 1,
                    NominalValue = 4000,
                    TotalNominalValue = 6000,
                    DifferenceValueDelivered = 5000,
                    DifferenceValueDeliveredPercentage = 5
                },
                new ScenarioCampaignMetricItem
                {
                    CampaignExternalId = _campaigns[1].ExternalId,
                    TotalSpots = 1,
                    ZeroRatedSpots = 0,
                    NominalValue = 10000,
                    TotalNominalValue = 13000,
                    DifferenceValueDelivered = 11000,
                    DifferenceValueDeliveredPercentage = 5.5
                },
                new ScenarioCampaignMetricItem
                {
                    CampaignExternalId = _campaigns[2].ExternalId,
                    TotalSpots = 1,
                    ZeroRatedSpots = 0,
                    NominalValue = 20000,
                    TotalNominalValue = 25000,
                    DifferenceValueDelivered = 22000,
                    DifferenceValueDeliveredPercentage = 7.33
                }
            };

            //Act
            var result = GenerateCampaignKPIs();

            //Assert
            Assert.IsTrue(CheckIfEqualLists(expected, result));
        }

        private List<ScenarioCampaignMetricItem> GenerateCampaignKPIs()
        {
            var kpis = new List<ScenarioCampaignMetricItem>();
            var calculator = new ScenarioCampaignKPIsCalculator();

            foreach (var campaign in _campaigns)
            {
                var nominalPrice = _spots
                    .Where(s =>
                        !s.ClientPicked &&
                        !s.IsUnplaced &&
                        s.ExternalCampaignNumber == campaign.ExternalId)
                    .Sum(s => s.NominalPrice);

                var campaignRecommendations = _recommendations.Where(r =>
                    r.ExternalCampaignNumber == campaign.ExternalId);

                kpis.Add(calculator.CalculateCampaignKPIs(campaign, campaignRecommendations, nominalPrice));
            }

            return kpis;
        }

        private static bool CheckIfEqualLists(
            List<ScenarioCampaignMetricItem> expected,
            List<ScenarioCampaignMetricItem> result
            )
        {
            for (var i = 0; i < expected.Count; i++)
            {
                if (!CheckIfEqual(expected[i], result[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CheckIfEqual(ScenarioCampaignMetricItem expected, ScenarioCampaignMetricItem result)
        {
            return expected.CampaignExternalId == result.CampaignExternalId &&
                expected.TotalSpots == result.TotalSpots &&
                expected.ZeroRatedSpots == result.ZeroRatedSpots &&
                expected.NominalValue == result.NominalValue &&
                expected.TotalNominalValue == result.TotalNominalValue &&
                expected.DifferenceValueDelivered == result.DifferenceValueDelivered &&
                expected.DifferenceValueDeliveredPercentage == result.DifferenceValueDeliveredPercentage;
        }
    }
}

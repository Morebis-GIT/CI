using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using xggameplan.AutoGen.AgDataPopulation;
using xggameplan.Model.AutoGen;
using Xunit;

namespace ImagineCommunications.GamePlan.Core.Tests
{
    public class RatingPointsDataPopulationTests : DataPopulationTestBase
    {
        public RatingPointsDataPopulationTests()
        {
            _tenantSettings = _fixture.Build<TenantSettings>()
                .With(r => r.PeakStartTime, "120000")
                .With(r => r.PeakEndTime, "155959")
                .With(r => r.MidnightStartTime, "240000")
                .With(r => r.MidnightEndTime, "995959").Create();

            _salesAreas = _fixture.CreateMany<SalesArea>(10).ToList();
            _salesAreas.ForEach(s => s.Name = _salesAreaArr[_random.Next(0, _salesAreaArr.Length)]);
        }

        [Fact(DisplayName = "Perform Rating Points data population should return list of Rating Points")]
        public void PerformRatingPointsDataPopulation_ShouldReturnListOfRatingPoints()
        {
            //Arrange
            var testRatingPoint = new RatingPoint
            {
                SalesAreas = _salesAreas.Select(s => s.Name),
                OffPeakValue = 10,
                PeakValue = 50,
                MidnightToDawnValue = 5
            };

            var testPasses = new List<Pass>
            {
                _fixture.Create<Pass>()
            };

            testPasses[0].RatingPoints = new List<RatingPoint> { testRatingPoint };

            var expected = new List<AgRatingPoint>();

            foreach (var salesArea in _salesAreas)
            {
                expected.Add(
                    new AgRatingPoint
                    {
                        PassId = testPasses[0].Id,
                        SalesAreaNumber = salesArea.CustomId,
                        DaypartsCount = 4,
                        RatingPointsForDaypartsList = new AgRatingPointsForDaypartsList
                        {
                            new AgRatingPointForDaypart
                            {
                                StartTime = "0",
                                EndTime = "115959",
                                Value = 10
                            },
                            new AgRatingPointForDaypart
                            {
                                StartTime = "120000",
                                EndTime = "155959",
                                Value = 50
                            },
                            new AgRatingPointForDaypart
                            {
                                StartTime = "160000",
                                EndTime = "235959",
                                Value = 10
                            },
                            new AgRatingPointForDaypart
                            {
                                StartTime = "240000",
                                EndTime = "995959",
                                Value = 5
                            }
                        }
                    });
            }

            //Act
            var result = testPasses.ToAgRatingPoints(_salesAreas, _tenantSettings);

            //Assert
            _ = CheckIfEqual(expected, result?.AgRatingPoints).Should().BeTrue(null);
        }

        [Fact(DisplayName = "Perform Rating Points data population without peak daypart should return list of Rating Points")]
        public void PerformRatingPointsDataPopulationWithoutPeakDaypart_ShouldReturnListOfRatingPoints()
        {
            //Arrange
            var testRatingPoint = new RatingPoint
            {
                SalesAreas = _salesAreas.Select(s => s.Name),
                OffPeakValue = 10,
                MidnightToDawnValue = 5
            };

            var testPasses = new List<Pass>
            {
                _fixture.Create<Pass>()
            };

            testPasses[0].RatingPoints = new List<RatingPoint> { testRatingPoint };

            var expected = new List<AgRatingPoint>();

            foreach (var salesArea in _salesAreas)
            {
                expected.Add(
                    new AgRatingPoint
                    {
                        PassId = testPasses[0].Id,
                        SalesAreaNumber = salesArea.CustomId,
                        DaypartsCount = 3,
                        RatingPointsForDaypartsList = new AgRatingPointsForDaypartsList
                        {
                            new AgRatingPointForDaypart
                            {
                                StartTime = "0",
                                EndTime = "115959",
                                Value = 10
                            },
                            new AgRatingPointForDaypart
                            {
                                StartTime = "160000",
                                EndTime = "235959",
                                Value = 10
                            },
                            new AgRatingPointForDaypart
                            {
                                StartTime = "240000",
                                EndTime = "995959",
                                Value = 5
                            }
                        }
                    });
            }

            //Act
            var result = testPasses.ToAgRatingPoints(_salesAreas, _tenantSettings);

            //Assert
            _ = CheckIfEqual(expected, result?.AgRatingPoints).Should().BeTrue(null);
        }

        [Fact(DisplayName = "Perform Rating Points data population with multiple Rating Points for same sales area should return list of Rating Points")]
        public void PerformRatingPointsDataPopulationWithMultipleRatingPointsForSameSalesArea_ShouldReturnListOfRatingPoints()
        {
            //Arrange

            var testRatingPoint = new List<RatingPoint>
            {
                new RatingPoint
                {
                    SalesAreas = _salesAreas.Select(s => s.Name),
                    OffPeakValue = 10,
                },
                new RatingPoint
                {
                    SalesAreas = _salesAreas.Select(s => s.Name),
                    PeakValue = 50
                },
                new RatingPoint
                {
                    SalesAreas = _salesAreas.Select(s => s.Name),
                    MidnightToDawnValue = 5
                }
            };

            var testPasses = new List<Pass>
            {
                _fixture.Create<Pass>()
            };

            testPasses[0].RatingPoints = testRatingPoint;

            var expected = new List<AgRatingPoint>();

            foreach (var salesArea in _salesAreas)
            {
                expected.Add(
                    new AgRatingPoint
                    {
                        PassId = testPasses[0].Id,
                        SalesAreaNumber = salesArea.CustomId,
                        DaypartsCount = 4,
                        RatingPointsForDaypartsList = new AgRatingPointsForDaypartsList
                        {
                            new AgRatingPointForDaypart
                            {
                                StartTime = "0",
                                EndTime = "115959",
                                Value = 10
                            },
                            new AgRatingPointForDaypart
                            {
                                StartTime = "120000",
                                EndTime = "155959",
                                Value = 50
                            },
                            new AgRatingPointForDaypart
                            {
                                StartTime = "160000",
                                EndTime = "235959",
                                Value = 10
                            },
                            new AgRatingPointForDaypart
                            {
                                StartTime = "240000",
                                EndTime = "995959",
                                Value = 5
                            }
                        }
                    });
            }

            //Act
            var result = testPasses.ToAgRatingPoints(_salesAreas, _tenantSettings);

            //Assert
            _ = CheckIfEqual(expected, result?.AgRatingPoints).Should().BeTrue(null);
        }

        [Fact(DisplayName = "Perform Rating Points data population with Rating Point for all sales areas should return list of Rating Points")]
        public void PerformRatingPointsDataPopulationWithRatingPointForAllSalesAreas_ShouldReturnListOfRatingPoints()
        {
            //Arrange
            var testRatingPoints = new RatingPoint
            {
                SalesAreas = new List<string>(),
                OffPeakValue = 10,
                PeakValue = 50,
                MidnightToDawnValue = 5
            };

            var testPasses = new List<Pass>
            {
                _fixture.Create<Pass>()
            };

            testPasses[0].RatingPoints = new List<RatingPoint> { testRatingPoints };

            var expected = new List<AgRatingPoint>();

            foreach (var salesArea in _salesAreas)
            {
                expected.Add(
                    new AgRatingPoint
                    {
                        PassId = testPasses[0].Id,
                        SalesAreaNumber = salesArea.CustomId,
                        DaypartsCount = 4,
                        RatingPointsForDaypartsList = new AgRatingPointsForDaypartsList
                        {
                            new AgRatingPointForDaypart
                            {
                                StartTime = "0",
                                EndTime = "115959",
                                Value = 10
                            },
                            new AgRatingPointForDaypart
                            {
                                StartTime = "120000",
                                EndTime = "155959",
                                Value = 50
                            },
                            new AgRatingPointForDaypart
                            {
                                StartTime = "160000",
                                EndTime = "235959",
                                Value = 10
                            },
                            new AgRatingPointForDaypart
                            {
                                StartTime = "240000",
                                EndTime = "995959",
                                Value = 5
                            }
                        }
                    });
            }

            //Act
            var result = testPasses.ToAgRatingPoints(_salesAreas, _tenantSettings);

            //Assert
            _ = CheckIfEqual(expected, result?.AgRatingPoints).Should().BeTrue(null);
        }

        private static bool CheckIfEqual(List<AgRatingPoint> expected, List<AgRatingPoint> result)
        {
            foreach (var ratingPoint in expected)
            {
                if (result.Find(t => t.PassId == ratingPoint.PassId &&
                    t.SalesAreaNumber == ratingPoint.SalesAreaNumber &&
                    t.DaypartsCount == ratingPoint.DaypartsCount &&
                    CheckIfEqualRatingPointsForDayparts(ratingPoint.RatingPointsForDaypartsList, t.RatingPointsForDaypartsList)) is null)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CheckIfEqualRatingPointsForDayparts(
            AgRatingPointsForDaypartsList expected,
            AgRatingPointsForDaypartsList result)
        {
            foreach (var daypartRatingPoint in expected)
            {
                if (!result.Any(t =>
                    t.StartTime == daypartRatingPoint.StartTime &&
                    t.EndTime == daypartRatingPoint.EndTime &&
                    t.Value == daypartRatingPoint.Value))
                {
                    return false;
                }
            }

            return true;
        }
    }
}

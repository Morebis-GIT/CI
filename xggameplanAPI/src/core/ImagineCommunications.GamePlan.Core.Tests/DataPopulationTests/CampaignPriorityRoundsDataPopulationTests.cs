using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using xggameplan.AutoGen.AgDataPopulation;
using xggameplan.Model.AutoGen;
using Xunit;

namespace ImagineCommunications.GamePlan.Core.Tests
{
    public class CampaignPriorityRoundsDataPopulationTests : DataPopulationTestBase
    {
        [Fact(DisplayName = "Perform campaign priority rounds data population should return list of campaign priority rounds")]
        public void PerformCampaignPriorityRoundsDataPopulation_ShouldReturnListOfCampaignPriorityRounds()
        {
            //Arrange
            var testCampaignPriorityRounds = new CampaignPriorityRounds
            {
                ContainsInclusionRound = true,
                Rounds = new List<PriorityRound>
                {
                    new PriorityRound
                    {
                        Number = 1,
                        PriorityFrom = 1,
                        PriorityTo = 500
                    },
                    new PriorityRound
                    {
                        Number = 2,
                        PriorityFrom = 501,
                        PriorityTo = 999
                    },
                    new PriorityRound
                    {
                        Number = 3,
                        PriorityFrom = 1000,
                        PriorityTo = 1000
                    }
                }
            };

            var expected = new List<AgCampaignPriorityRound>
            {
                new AgCampaignPriorityRound
                {
                    Number = 1,
                    IsProgrammeInclusionsRound = 1,
                    PriorityFrom = 1,
                    PriorityTo = 500,
                    Description = "Round 1"
                },
                new AgCampaignPriorityRound
                {
                    Number = 2,
                    IsProgrammeInclusionsRound = 0,
                    PriorityFrom = 501,
                    PriorityTo = 999,
                    Description = "Round 2"
                },
                new AgCampaignPriorityRound
                {
                    Number = 3,
                    IsProgrammeInclusionsRound = 0,
                    PriorityFrom = 1000,
                    PriorityTo = 1000,
                    Description = "Round 3"
                }
            };

            _scenario = _fixture.Build<Scenario>()
                    .With(r => r.CampaignPriorityRounds, testCampaignPriorityRounds).Create();

            //Act
            var result = _scenario.ToAgCampaignPriorityRound();

            //Assert
            _ = CheckIfEqualLists(expected, result?.AgCampaignPriorityRounds).Should().BeTrue(null);
        }

        [Fact(DisplayName = "Perform campaign priority rounds data population and contains inclusion round is true should return list of campaign priority rounds and first round should be programme inclusions round")]
        public void PerformCampaignPriorityRoundsDataPopulation_AndContainsInclusionRoundIsTrue_ShouldReturnListOfCampaignPriorityRounds_AndFirstRoundShouldBeProgrammeInclusionsRound()
        {
            //Arrange
            var testCampaignPriorityRounds = new CampaignPriorityRounds
            {
                ContainsInclusionRound = true,
                Rounds = new List<PriorityRound>
                {
                    new PriorityRound
                    {
                        Number = 1,
                        PriorityFrom = 1,
                        PriorityTo = 500
                    },
                    new PriorityRound
                    {
                        Number = 2,
                        PriorityFrom = 501,
                        PriorityTo = 999
                    }
                }
            };

            var expected = new List<AgCampaignPriorityRound>
            {
                new AgCampaignPriorityRound
                {
                    Number = 1,
                    IsProgrammeInclusionsRound = 1,
                    PriorityFrom = 1,
                    PriorityTo = 500,
                    Description = "Round 1"
                },
                new AgCampaignPriorityRound
                {
                    Number = 2,
                    IsProgrammeInclusionsRound = 0,
                    PriorityFrom = 501,
                    PriorityTo = 999,
                    Description = "Round 2"
                }
            };

            _scenario = _fixture.Build<Scenario>()
                    .With(r => r.CampaignPriorityRounds, testCampaignPriorityRounds).Create();

            //Act
            var result = _scenario.ToAgCampaignPriorityRound();

            //Assert
            _ = result?.AgCampaignPriorityRounds[0].IsProgrammeInclusionsRound.Should().Be(1, null);
            _ = result?.AgCampaignPriorityRounds[1].IsProgrammeInclusionsRound.Should().Be(0, null);
        }

        [Fact(DisplayName = "Perform campaign priority rounds data population and there are no campaign priority rounds should return empty list of campaign priority rounds")]
        public void PerformCampaignPriorityRoundsDataPopulation_AndThereAreNoCampaignPriorityRounds_ShouldReturnEmptyListOfCampaignPriorityRounds()
        {
            // Arrange
            var expected = new List<AgCampaignPriorityRound>();

            _scenario = _fixture.Build<Scenario>()
                .Without(r => r.CampaignPriorityRounds).Create();

            // Act
            var result = _scenario.ToAgCampaignPriorityRound();

            // Assert
            _ = result?.AgCampaignPriorityRounds.Should().HaveCount(0, null);
        }

        private static bool CheckIfEqualLists(List<AgCampaignPriorityRound> expected, List<AgCampaignPriorityRound> result)
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

        private static bool CheckIfEqual(AgCampaignPriorityRound expected, AgCampaignPriorityRound result)
        {
            return expected.Number == result.Number &&
                expected.IsProgrammeInclusionsRound == result.IsProgrammeInclusionsRound &&
                expected.PriorityFrom == result.PriorityFrom &&
                expected.PriorityTo == result.PriorityTo &&
                expected.Description == result.Description;
        }
    }
}

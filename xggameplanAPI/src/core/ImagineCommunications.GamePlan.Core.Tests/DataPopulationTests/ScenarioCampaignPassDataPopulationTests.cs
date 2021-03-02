using System.Linq;
using AutoFixture;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using xggameplan.AutoGen.AgDataPopulation;
using Xunit;

namespace ImagineCommunications.GamePlan.Core.Tests
{
    public class ScenarioCampaignPassDataPopulationTests : DataPopulationTestBase
    {
        public ScenarioCampaignPassDataPopulationTests()
        {
            _scenario = _fixture.Build<Scenario>().Create();
        }

        [Fact(DisplayName = "Perform scenario campaign pass data population, should return full serializable object when scenario campaign passes are not null")]
        public void PerformScenarioCampaignPassDataPopulation_ShouldReturnFullSerializableObject_WhenScenarioCampaignPassesAreNotNull()
        {
            //Arrange
            var expected = _scenario.CampaignPassPriorities
                .SelectMany(cpp => cpp.PassPriorities)
                .Count(pp => pp.Priority > 0);

            //Act
            var result = _scenario.ToAgScenarioCampaignPass();

            //Assert
            _ = result.Size.Should().Be(expected, null);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using Moq;
using xggameplan.core.Helpers;
using Xunit;

namespace ImagineCommunications.GamePlan.Core.Tests.HelpersTests
{
    [Trait("Core", "Optimiser Input File Helper")]
    public class OptimiserInputFilesHelperTests : IDisposable
    {
        private static readonly Fixture _fixture = new SafeFixture();

        private List<Campaign> _dummyCampaigns { get; }
        private Dictionary<Guid, Scenario> _dummyScenarios { get; } = new Dictionary<Guid, Scenario>();
        private List<CampaignPassPriority> _dummyCampaignPassPriorities { get; } = new List<CampaignPassPriority>();
        private Run _dummyRun { get; }
        private Guid[] _dummyScenarioIds { get; }

        private Mock<IScenarioRepository> _mockScenarioRepository { get; set; }

        public OptimiserInputFilesHelperTests()
        {
            _mockScenarioRepository = new Mock<IScenarioRepository>();

            _dummyScenarioIds = _fixture.CreateMany<Guid>(3).ToArray();
            _dummyCampaigns = _fixture.Build<Campaign>().CreateMany(3).ToList();

            foreach (var campaign in _dummyCampaigns)
            {
                _dummyCampaignPassPriorities.Add(BuildCampaignPassPriority(999, campaign.CustomId));
            }

            _dummyRun = _fixture
                .Build<Run>()
                .With(r => r.Scenarios, new List<RunScenario>())
                .With(r => r.Optimisation, true)
                .With(r => r.RightSizer, true)
                .With(r => r.ISR, true)
                .Create();

            foreach (var scenarioId in _dummyScenarioIds)
            {
                _dummyScenarios.Add(
                    scenarioId,
                    _fixture
                        .Build<Scenario>()
                        .With(s => s.Id, scenarioId)
                        .With(s => s.CampaignPassPriorities, _dummyCampaignPassPriorities)
                        .Create()
                        );

                _dummyRun.Scenarios.Add(
                    _fixture
                        .Build<RunScenario>()
                        .With(rs => rs.Id, scenarioId)
                        .Create()
                   );

                _ = _mockScenarioRepository.Setup(o => o.Get(scenarioId)).Returns(_dummyScenarios[scenarioId]);
            }
        }

        [Fact(DisplayName =
            "Given a valid Run with multiple scenarios " +
            "When each scenario has the same campaign excluded " +
            "Then campaign list should have 1 less campaign")]
        public void MultipleScenariosWithOverlappingExcludedCampaignsSuccess()
        {
            // Arrange
            _dummyCampaignPassPriorities.First().PassPriorities.ForEach(o => o.Priority = 0);
            foreach (var key in _dummyScenarios.Keys)
            {
                _dummyScenarios[key].CampaignPassPriorities = _dummyCampaignPassPriorities;
                _ = _mockScenarioRepository.Setup(o => o.Get(key)).Returns(_dummyScenarios[key]);
            }
            var expectedCampaigns = _dummyCampaigns
                .Where(x => x.CustomId != _dummyCampaignPassPriorities.First().Campaign.CustomId)
                .ToList();

            // Act
            var resultCampaigns = OptimiserInputFilesHelper.FilterOutExcludedCampaigns(_mockScenarioRepository.Object, _dummyRun, _dummyCampaigns);

            // Assert
            _ = resultCampaigns.Should()
                .NotBeNullOrEmpty(becauseArgs: null)
                .And.HaveCount(_dummyCampaigns.Count - 1, becauseArgs: null)
                .And.Equal(expectedCampaigns, becauseArgs: null);
        }

        [Fact(DisplayName =
            "Given a valid Run with multiple scenarios " +
            "When each scenario has the same campaign excluded, but with ISR enabled " +
            "Then campaign list should not be filtered")]
        public void MultipleScenariosWithOverlappingExcludedCampaignsAndISRSuccess()
        {
            // Arrange
            _dummyCampaignPassPriorities.First().PassPriorities.ForEach(o => o.Priority = 0);
            foreach (var key in _dummyScenarios.Keys)
            {
                _dummyScenarios[key].CampaignPassPriorities = _dummyCampaignPassPriorities;

                _dummyScenarios[key].CampaignPassPriorities.ForEach(cpp => cpp.Campaign.InefficientSpotRemoval = true);

                _ = _mockScenarioRepository.Setup(o => o.Get(key)).Returns(_dummyScenarios[key]);
            }

            // Act
            var resultCampaigns = OptimiserInputFilesHelper.FilterOutExcludedCampaigns(_mockScenarioRepository.Object, _dummyRun, _dummyCampaigns);

            // Assert
            _ = resultCampaigns.Should()
                .NotBeNullOrEmpty(becauseArgs: null)
                .And.HaveCount(_dummyCampaigns.Count, becauseArgs: null)
                .And.Equal(_dummyCampaigns, becauseArgs: null);
        }

        [Fact(DisplayName =
            "Given a valid Run with multiple scenarios " +
            "When each scenario has the same campaign excluded, but with IncludeRightSizer enabled " +
            "Then campaign list should not be filtered")]
        public void MultipleScenariosWithOverlappingExcludedCampaignsAndIncludeRightsizerSuccess()
        {
            // Arrange
            _dummyCampaignPassPriorities.First().PassPriorities.ForEach(o => o.Priority = 0);
            foreach (var key in _dummyScenarios.Keys)
            {
                _dummyScenarios[key].CampaignPassPriorities = _dummyCampaignPassPriorities;

                _dummyScenarios[key].CampaignPassPriorities.ForEach(cpp => cpp.Campaign.IncludeRightSizer = IncludeRightSizer.CampaignLevel);

                _ = _mockScenarioRepository.Setup(o => o.Get(key)).Returns(_dummyScenarios[key]);
            }

            // Act
            var resultCampaigns = OptimiserInputFilesHelper.FilterOutExcludedCampaigns(_mockScenarioRepository.Object, _dummyRun, _dummyCampaigns);

            // Assert
            _ = resultCampaigns.Should()
                .NotBeNullOrEmpty(becauseArgs: null)
                .And.HaveCount(_dummyCampaigns.Count, becauseArgs: null)
                .And.Equal(_dummyCampaigns, becauseArgs: null);
        }

        [Fact(DisplayName =
            "Given a valid Run with multiple scenarios " +
            "When each scenario has the same campaign excluded, but with Optimisation enabled " +
            "Then campaign list should have 1 less campaign")]
        public void MultipleScenariosWithOverlappingExcludedCampaignsAndOptimisationSuccess()
        {
            // Arrange
            _dummyCampaignPassPriorities.First().PassPriorities.ForEach(o => o.Priority = 0);
            foreach (var key in _dummyScenarios.Keys)
            {
                _dummyScenarios[key].CampaignPassPriorities = _dummyCampaignPassPriorities;

                _dummyScenarios[key].CampaignPassPriorities.ForEach(cpp => cpp.Campaign.IncludeOptimisation = true);

                _ = _mockScenarioRepository.Setup(o => o.Get(key)).Returns(_dummyScenarios[key]);
            }

            var expectedCampaigns = _dummyCampaigns
                .Where(x => x.CustomId != _dummyCampaignPassPriorities.First().Campaign.CustomId)
                .ToList();

            // Act
            var resultCampaigns = OptimiserInputFilesHelper.FilterOutExcludedCampaigns(_mockScenarioRepository.Object, _dummyRun, _dummyCampaigns);

            // Assert
            _ = resultCampaigns.Should()
                .NotBeNullOrEmpty(becauseArgs: null)
                .And.HaveCount(_dummyCampaigns.Count - 1, becauseArgs: null)
                .And.Equal(expectedCampaigns, becauseArgs: null);
        }

        [Fact(DisplayName =
            "Given a valid Run with multiple scenarios " +
            "When each scenario has different campaign excluded " +
            "Then campaign list should not be filtered")]
        public void MultipleScenariosWithNoOverlappingExcludedCampaignsSuccess()
        {
            // Arrange
            int i = 0;
            foreach (var key in _dummyScenarios.Keys)
            {
                var tempListCampaignPassPriority = new List<CampaignPassPriority>();

                for (var x = 0; x < _dummyCampaigns.Count; x++)
                {
                    var priority = x == i ? 0 : 999;
                    tempListCampaignPassPriority.Add(BuildCampaignPassPriority(priority, _dummyCampaigns[x].CustomId));
                }

                i++;

                _dummyScenarios[key].CampaignPassPriorities = tempListCampaignPassPriority;
                _ = _mockScenarioRepository.Setup(o => o.Get(key)).Returns(_dummyScenarios[key]);
            }

            // Act
            var resultCampaigns = OptimiserInputFilesHelper.FilterOutExcludedCampaigns(_mockScenarioRepository.Object, _dummyRun, _dummyCampaigns);

            // Assert
            _ = resultCampaigns.Should()
                .NotBeNullOrEmpty(becauseArgs: null)
                .And.HaveCount(_dummyCampaigns.Count, becauseArgs: null)
                .And.Equal(_dummyCampaigns, becauseArgs: null);
        }

        [Fact(DisplayName =
            "Given a valid Run with multiple scenarios " +
            "When each scenario has no excluded campaigns " +
            "Then campaign list should not be filtered")]
        public void MultipleScenariosWithNoExcludedCampaignsSuccess()
        {
            // Act
            var resultCampaigns = OptimiserInputFilesHelper.FilterOutExcludedCampaigns(_mockScenarioRepository.Object, _dummyRun, _dummyCampaigns);

            // Assert
            _ = resultCampaigns.Should()
                .NotBeNullOrEmpty(becauseArgs: null)
                .And.HaveCount(_dummyCampaigns.Count, becauseArgs: null)
                .And.Equal(_dummyCampaigns, becauseArgs: null);
        }

        [Fact(DisplayName =
            "Given a valid Run with single scenario " +
            "When the scenario has one excluded campaign " +
            "Then campaign list should have 1 less campaign")]
        public void SingleScenarioWithExcludedCampaignsSuccess()
        {
            // Arrange
            var firstScenario = _dummyScenarios.First();
            _dummyCampaignPassPriorities.First().PassPriorities.ForEach(o => o.Priority = 0);

            firstScenario.Value.CampaignPassPriorities = _dummyCampaignPassPriorities;
            _ = _mockScenarioRepository.Setup(o => o.Get(firstScenario.Key)).Returns(firstScenario.Value);

            _dummyRun.Scenarios = _dummyRun.Scenarios.Where(o => o.Id == firstScenario.Key).ToList();
            var expectedCampaigns = _dummyCampaigns
                .Where(x => x.CustomId != _dummyCampaignPassPriorities.First().Campaign.CustomId)
                .ToList();

            // Act
            var resultCampaigns = OptimiserInputFilesHelper.FilterOutExcludedCampaigns(_mockScenarioRepository.Object, _dummyRun, _dummyCampaigns);

            // Assert
            _ = resultCampaigns.Should()
                .NotBeNullOrEmpty(becauseArgs: null)
                .And.HaveCount(_dummyCampaigns.Count - 1, becauseArgs: null)
                .And.Equal(expectedCampaigns, becauseArgs: null);
        }

        [Fact(DisplayName =
            "Given a valid Run with single scenario " +
            "When the scenario has one excluded campaign, but with ISR enabled " +
            "Then campaign list should not be filtered")]
        public void SingleScenarioWithExcludedCampaignsAndISRSuccess()
        {
            // Arrange
            var firstScenario = _dummyScenarios.First();
            _dummyCampaignPassPriorities.ForEach(cpp => cpp.Campaign.InefficientSpotRemoval = true);
            _dummyCampaignPassPriorities.First().PassPriorities.ForEach(o => o.Priority = 0);

            firstScenario.Value.CampaignPassPriorities = _dummyCampaignPassPriorities;
            _ = _mockScenarioRepository.Setup(o => o.Get(firstScenario.Key)).Returns(firstScenario.Value);

            _dummyRun.Scenarios = _dummyRun.Scenarios.Where(o => o.Id == firstScenario.Key).ToList();

            // Act
            var resultCampaigns = OptimiserInputFilesHelper.FilterOutExcludedCampaigns(_mockScenarioRepository.Object, _dummyRun, _dummyCampaigns);

            // Assert
            _ = resultCampaigns.Should()
                .NotBeNullOrEmpty(becauseArgs: null)
                .And.HaveCount(_dummyCampaigns.Count, becauseArgs: null)
                .And.Equal(_dummyCampaigns, becauseArgs: null);
        }

        [Fact(DisplayName =
            "Given a valid Run with single scenario " +
            "When the scenario has one excluded campaign, but with IncludeRightSizer enabled " +
            "Then campaign list should not be filtered")]
        public void SingleScenarioWithExcludedCampaignsAndIncludeRightsizerSuccess()
        {
            // Arrange
            var firstScenario = _dummyScenarios.First();
            _dummyCampaignPassPriorities.ForEach(cpp => cpp.Campaign.IncludeRightSizer = IncludeRightSizer.CampaignLevel);
            _dummyCampaignPassPriorities.First().PassPriorities.ForEach(o => o.Priority = 0);

            firstScenario.Value.CampaignPassPriorities = _dummyCampaignPassPriorities;
            _ = _mockScenarioRepository.Setup(o => o.Get(firstScenario.Key)).Returns(firstScenario.Value);

            _dummyRun.Scenarios = _dummyRun.Scenarios.Where(o => o.Id == firstScenario.Key).ToList();

            // Act
            var resultCampaigns = OptimiserInputFilesHelper.FilterOutExcludedCampaigns(_mockScenarioRepository.Object, _dummyRun, _dummyCampaigns);

            // Assert
            _ = resultCampaigns.Should()
                .NotBeNullOrEmpty(becauseArgs: null)
                .And.HaveCount(_dummyCampaigns.Count, becauseArgs: null)
                .And.Equal(_dummyCampaigns, becauseArgs: null);
        }

        [Fact(DisplayName =
            "Given a valid Run with single scenario " +
            "When the scenario has one excluded campaign, but with Optimisation enabled " +
            "Then campaign list should have 1 less campaign")]
        public void SingleScenarioWithExcludedCampaignsAndOptimisationSuccess()
        {
            // Arrange
            var firstScenario = _dummyScenarios.First();
            _dummyCampaignPassPriorities.ForEach(cpp => cpp.Campaign.IncludeOptimisation = true);
            _dummyCampaignPassPriorities.First().PassPriorities.ForEach(o => o.Priority = 0);

            firstScenario.Value.CampaignPassPriorities = _dummyCampaignPassPriorities;
            _ = _mockScenarioRepository.Setup(o => o.Get(firstScenario.Key)).Returns(firstScenario.Value);

            _dummyRun.Scenarios = _dummyRun.Scenarios.Where(o => o.Id == firstScenario.Key).ToList();

            var expectedCampaigns = _dummyCampaigns
                .Where(x => x.CustomId != _dummyCampaignPassPriorities.First().Campaign.CustomId)
                .ToList();

            // Act
            var resultCampaigns = OptimiserInputFilesHelper.FilterOutExcludedCampaigns(_mockScenarioRepository.Object, _dummyRun, _dummyCampaigns);

            // Assert
            _ = resultCampaigns.Should()
                .NotBeNullOrEmpty(becauseArgs: null)
                .And.HaveCount(_dummyCampaigns.Count - 1, becauseArgs: null)
                .And.Equal(expectedCampaigns, becauseArgs: null);
        }

        [Fact(DisplayName =
            "Given a valid Run with single scenario " +
            "When the scenario has no excluded campaigns " +
            "Then campaign list should not be filtered")]
        public void SingleScenarioWithNoExcludedCampaignsSuccess()
        {
            // Arrange
            _dummyRun.Scenarios = new List<RunScenario>() { _dummyRun.Scenarios.First() };

            // Act
            var resultCampaigns = OptimiserInputFilesHelper.FilterOutExcludedCampaigns(_mockScenarioRepository.Object, _dummyRun, _dummyCampaigns);

            // Assert
            _ = resultCampaigns.Should()
                .NotBeNullOrEmpty(becauseArgs: null)
                .And.HaveCount(_dummyCampaigns.Count, becauseArgs: null)
                .And.Equal(_dummyCampaigns, becauseArgs: null);
        }

        public void Dispose()
        {
            _mockScenarioRepository = null;
        }

        private CampaignPassPriority BuildCampaignPassPriority(int priority, int campaignId)
        {
            return _fixture
                        .Build<CampaignPassPriority>()
                        .With(cpp => cpp.PassPriorities,
                            _fixture
                            .Build<PassPriority>()
                            .With(pp => pp.Priority, priority)
                            .CreateMany(2)
                            .ToList()
                        )
                        .With(cpp => cpp.Campaign,
                            _fixture
                            .Build<CompactCampaign>()
                            .With(cc => cc.CustomId, campaignId)
                            .With(cc => cc.IncludeRightSizer, IncludeRightSizer.No)
                            .With(cc => cc.InefficientSpotRemoval, false)
                            .With(cc => cc.IncludeOptimisation, true)
                            .Create())
                        .Create();
        }
    }
}

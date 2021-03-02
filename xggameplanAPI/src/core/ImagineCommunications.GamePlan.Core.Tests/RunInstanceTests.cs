using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using ImagineCommunications.Gameplan.Synchronization.Interfaces;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using Moq;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks.Abstractions;
using xggameplan.core.BRS;
using xggameplan.core.Landmark.Abstractions;
using xggameplan.model.Internal.Landmark;
using xggameplan.Model;
using xggameplan.RunManagement;
using Xunit;

namespace ImagineCommunications.GamePlan.Core.Tests
{
    public class RunInstanceTests
    {
        private static readonly Fixture _fixture = new SafeFixture();

        private readonly Mock<IRepositoryFactory> _repositoryFactoryMock = new Mock<IRepositoryFactory>();
        private readonly Mock<IAuditEventRepository> _auditEventRepositoryMock = new Mock<IAuditEventRepository>();
        private readonly Mock<IAutoBookInputHandler> _inputHandlerMock = new Mock<IAutoBookInputHandler>();
        private readonly Mock<IAutoBookOutputHandler> _outputHandlerMock = new Mock<IAutoBookOutputHandler>();
        private readonly Mock<ISynchronizationService> _synchronizationServiceMock = new Mock<ISynchronizationService>();
        private readonly Mock<IPipelineAuditEventRepository> _pipelineAuditEventRepositoryMock = new Mock<IPipelineAuditEventRepository>();
        private readonly Mock<IBRSIndicatorManager> _brsIndicatorManagerMock = new Mock<IBRSIndicatorManager>();
        private readonly Mock<ILandmarkRunService> _landmarkRunServiceMock = new Mock<ILandmarkRunService>();

        private readonly RunInstance _runInstance;

        public RunInstanceTests()
        {
            _runInstance = new RunInstance(
                Guid.NewGuid(),
                Guid.NewGuid(),
                _repositoryFactoryMock.Object,
                _auditEventRepositoryMock.Object,
                _inputHandlerMock.Object,
                _outputHandlerMock.Object,
                null,
                null,
                _synchronizationServiceMock.Object,
                _pipelineAuditEventRepositoryMock.Object,
                _brsIndicatorManagerMock.Object,
                _landmarkRunServiceMock.Object,
                null,
                null
            );
        }

        [Fact]
        public void NotSendToLandmark_ScheduleSettingsIsNull()
        {
            var triggered = false;

            _ = _landmarkRunServiceMock.Setup(x => x.TriggerRunAsync(It.IsAny<LandmarkRunTriggerModel>(), It.IsAny<ScheduledRunSettingsModel>()))
                .Callback<LandmarkRunTriggerModel, ScheduledRunSettingsModel>((x, _) => triggered = true);

            var (run, _, scenarioResults) = GenerateData(3);
            run.ScheduleSettings = null;

            _runInstance.SendToLandmark(run, scenarioResults);

            Assert.False(triggered);
        }

        [Fact]
        public void NotSendToLandmark_CalculatedScenarioIsNull()
        {
            var triggered = false;

            _ = _landmarkRunServiceMock.Setup(x => x.TriggerRunAsync(It.IsAny<LandmarkRunTriggerModel>(), It.IsAny<ScheduledRunSettingsModel>()))
                .Callback<LandmarkRunTriggerModel, ScheduledRunSettingsModel>((x, _) => triggered = true);

            var (run, _, _) = GenerateData(3);
            run.ScheduleSettings = new RunScheduleSettings();

            _runInstance.SendToLandmark(run, null);

            Assert.False(triggered);
        }

        [Fact]
        public void SendToLandmark_SingleScenario()
        {
            var actualBestScenarioId = Guid.Empty;

            _ = _landmarkRunServiceMock.Setup(x => x.TriggerRunAsync(It.IsAny<LandmarkRunTriggerModel>(), It.IsAny<ScheduledRunSettingsModel>()))
                .Callback<LandmarkRunTriggerModel, ScheduledRunSettingsModel>((x, _) => actualBestScenarioId = x.ScenarioId);

            var (run, scenarios, scenarioResults) = GenerateData(1);
            run.ScheduleSettings = new RunScheduleSettings();

            _runInstance.SendToLandmark(run, scenarioResults);

            Assert.Equal(scenarios.First().Id, actualBestScenarioId);
        }

        [Fact]
        public void SendToLandmark_MultipleScenario_BestByBRS()
        {
            var actualBestScenarioId = Guid.Empty;

            _ = _landmarkRunServiceMock.Setup(x => x.TriggerRunAsync(It.IsAny<LandmarkRunTriggerModel>(), It.IsAny<ScheduledRunSettingsModel>()))
                .Callback<LandmarkRunTriggerModel, ScheduledRunSettingsModel>((x, _) => actualBestScenarioId = x.ScenarioId);

            var (run, scenarios, scenarioResults) = GenerateData(3);
            run.ScheduleSettings = new RunScheduleSettings();

            for (var i = 0; i < scenarioResults.Count; i++)
            {
                scenarioResults[i].BRSIndicator = i;
            }

            _runInstance.SendToLandmark(run, scenarioResults);

            Assert.Equal(scenarios.Last().Id, actualBestScenarioId);
        }

        [Fact]
        public void SendToLandmark_MultipleScenario_BestByWeightedAverageCompletion()
        {
            var actualBestScenarioId = Guid.Empty;

            _ = _landmarkRunServiceMock.Setup(x => x.TriggerRunAsync(It.IsAny<LandmarkRunTriggerModel>(), It.IsAny<ScheduledRunSettingsModel>()))
                .Callback<LandmarkRunTriggerModel, ScheduledRunSettingsModel>((x, _) => actualBestScenarioId = x.ScenarioId);

            var (run, scenarios, scenarioResults) = GenerateData(3);
            run.ScheduleSettings = new RunScheduleSettings();

            for (var i = 0; i < scenarioResults.Count; i++)
            {
                scenarioResults[i].BRSIndicator = 123;
                scenarioResults[i].Metrics = new List<KPI>(1)
                {
                    new KPI
                    {
                        Name = ScenarioKPINames.WeightedAverageCompletion,
                        Value = i
                    }
                };
            }

            _runInstance.SendToLandmark(run, scenarioResults);

            Assert.Equal(scenarios.Last().Id, actualBestScenarioId);
        }

        [Fact]
        public void SendToLandmark_MultipleScenario_FirstBecauseSameWeightedAverageCompletion()
        {
            var actualBestScenarioId = Guid.Empty;

            _ = _landmarkRunServiceMock.Setup(x => x.TriggerRunAsync(It.IsAny<LandmarkRunTriggerModel>(), It.IsAny<ScheduledRunSettingsModel>()))
                .Callback<LandmarkRunTriggerModel, ScheduledRunSettingsModel>((x, _) => actualBestScenarioId = x.ScenarioId);

            var (run, scenarios, scenarioResults) = GenerateData(3);
            run.ScheduleSettings = new RunScheduleSettings();

            for (var i = 0; i < scenarioResults.Count; i++)
            {
                scenarioResults[i].BRSIndicator = 123;
                scenarioResults[i].Metrics = new List<KPI>(1)
                {
                    new KPI
                    {
                        Name = ScenarioKPINames.WeightedAverageCompletion,
                        Value = 123
                    }
                };
                run.Scenarios.First(x => x.Id == scenarioResults[i].Id).Order = i;
            }

            _runInstance.SendToLandmark(run, scenarioResults);

            Assert.Equal(scenarios.First().Id, actualBestScenarioId);
        }

        private (Run, List<Scenario>, List<ScenarioResult>) GenerateData(int scenariosCount)
        {
            var scenarioIds = _fixture.CreateMany<Guid>(scenariosCount).ToArray();

            var scenarios = new List<Scenario>(scenariosCount);
            var scenarioResults = new List<ScenarioResult>(scenariosCount);

            var run = _fixture
                .Build<Run>()
                .With(r => r.Scenarios, new List<RunScenario>())
                .Create();

            foreach (var scenarioId in scenarioIds)
            {
                scenarios.Add(
                    _fixture
                        .Build<Scenario>()
                        .With(s => s.Id, scenarioId)
                        .Create()
                );

                run.Scenarios.Add(
                    _fixture
                        .Build<RunScenario>()
                        .With(rs => rs.Id, scenarioId)
                        .Create()
                );

                scenarioResults.Add(
                    _fixture
                        .Build<ScenarioResult>()
                        .With(rs => rs.Id, scenarioId)
                        .Create()
                );
            }

            return (run, scenarios, scenarioResults);
        }
    }
}

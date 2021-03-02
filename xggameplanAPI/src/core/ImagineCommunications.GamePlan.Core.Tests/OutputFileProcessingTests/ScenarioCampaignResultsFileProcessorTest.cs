using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using Moq;
using xggameplan.AuditEvents;
using xggameplan.core.OutputProcessors.Processors;
using xggameplan.CSVImporter;
using xggameplan.Model;
using xggameplan.OutputProcessors.Abstractions;
using xggameplan.Repository.CSV;
using Xunit;
using static ImagineCommunications.GamePlan.Core.Tests.FolderHelpers;

namespace ImagineCommunications.GamePlan.Core.Tests
{
    [Collection("Output file processing")]
    [Trait("Core", "Output file processing")]
    public class ScenarioCampaignResultsFileProcessorTest : IDisposable
    {
        private static readonly Fixture _fixture = new SafeFixture();

        private readonly RunWithScenarioReference _context;
        private IEnumerable<ScenarioCampaignResultImport> _expectedItems { get; }
        private IEnumerable<SalesArea> _dummySalesArea { get; }
        private Mock<ISalesAreaRepository> _mockSalesAreaRepository { get; set; }
        private Mock<IAuditEventRepository> _mockAuditEventRepository { get; set; }

        private ScenarioCampaignResultsFileProcessor _processor { get; }

        public ScenarioCampaignResultsFileProcessorTest()
        {
            _mockAuditEventRepository = new Mock<IAuditEventRepository>();
            _mockSalesAreaRepository = new Mock<ISalesAreaRepository>();
            _context = new RunWithScenarioReference(Guid.NewGuid(), Guid.NewGuid());
            var dataSnapshot = new Mock<IOutputDataSnapshot>();

            int id = 1;

            _dummySalesArea = _fixture
                .Build<SalesArea>()
                .Do(s => s.CustomId = id++)
                .CreateMany(5);

            _ = _mockSalesAreaRepository.Setup(m => m.GetAll()).Returns(_dummySalesArea);
            _ = dataSnapshot.Setup(x => x.AllSalesAreas)
                .Returns(new Lazy<IEnumerable<SalesArea>>(_mockSalesAreaRepository.Object.GetAll));

            var importSettings = CSVImportSettings.GetImportSettings(
                    Path.Combine(TestRootDataFolder, "lmkii_scen_camp_reqm_summ.csv"),
                    typeof(ScenarioCampaignResultHeaderMap),
                    typeof(ScenarioCampaignResultIndexMap)
                );

            var scenarioCampaignResultsImportRepository = new CSVScenarioCampaignResultsImportRepository(importSettings);

            _expectedItems = scenarioCampaignResultsImportRepository.GetAll();

            _processor = new ScenarioCampaignResultsFileProcessor(dataSnapshot.Object, _mockAuditEventRepository.Object);
        }

        [Fact(DisplayName = "When ScenarioCampaignResultFileProcessor run with stored file then it should return expected number data")]
        public void
        GivenScenarioCampaignResultFileProcessor_WhenProcessed_ThenShouldReturnExpectedNumberData()
        {
            var scenarioCampaignResults = _processor.ProcessFile(_context.ScenarioId, TestRootDataFolder);

            _ = _expectedItems.Should().HaveSameCount(scenarioCampaignResults.Items);
        }

        [Fact(DisplayName = "When ScenarioCampaignResultFileProcessor is run with stored file then it should return same optimiser booked spots value")]
        public void
        GivenScenarioCampaignResultFileProcessor_WhenProcessed_ThenShouldReturnSameOptimiserBookedSpotsValue()
        {
            // Arrange
            var scenarioCampaignResults = _processor.ProcessFile(_context.ScenarioId, TestRootDataFolder);

            // Act
            var result = scenarioCampaignResults.Items[0].OptimiserBookedSpots;

            // Assert
            _ = _expectedItems.First().OptimiserBookedSpots.Should().Be(result);
        }

        public void Dispose()
        {
            _mockSalesAreaRepository = null;
            _mockAuditEventRepository = null;
        }
    }
}

using System;
using System.Collections.Generic;
using AutoFixture;
using AutoMapper;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Domain.Spots;
using Moq;
using NUnit.Framework;
using xggameplan.AuditEvents;
using xggameplan.Configuration;
using xggameplan.Controllers;
using xggameplan.core.Configuration;
using xggameplan.Services;

namespace xggameplan.tests.ControllerTests
{
    [TestFixture(Category = "Controllers :: Breaks")]
    public partial class BreaksControllerTests : IDisposable
    {
        private Mock<IBreakRepository> _fakeBreakRepository;
        private Mock<IScheduleRepository> _fakeScheduleRepository;
        private Mock<ISpotRepository> _fakeSpotRepository;
        private Mock<IAuditEventRepository> _fakeAuditEventRepository;
        private Mock<IDataChangeValidator> _fakeDataChangeValidator;
        private Mock<IMetadataRepository> _fakeMetadataRepository;
        private Mock<ISalesAreaRepository> _fakeSalesAreaRepository;
        private IMapper _mapper;

        private readonly Fixture _fixture = new SafeFixture();

        private BreaksController _controller;

        [SetUp]
        public void Init()
        {
            _fakeBreakRepository = new Mock<IBreakRepository>();
            _fakeScheduleRepository = new Mock<IScheduleRepository>();
            _fakeSpotRepository = new Mock<ISpotRepository>();
            _fakeAuditEventRepository = new Mock<IAuditEventRepository>();
            _fakeDataChangeValidator = new Mock<IDataChangeValidator>();
            _fakeMetadataRepository = new Mock<IMetadataRepository>();
            _fakeSalesAreaRepository = new Mock<ISalesAreaRepository>();

            _ = _fakeMetadataRepository
                    .Setup(m => m.GetByKey(It.IsAny<MetaDataKeys>()))
                    .Returns(new List<Data> {
                        new Data {
                            Id = 1,
                            Value = ValidBreakType
                        }
                    });

            _mapper = AutoMapperInitializer.Initialize(AutoMapperConfig.SetupAutoMapper);

            _controller = new BreaksController(
                _fakeBreakRepository.Object,
                _mapper,
                _fakeScheduleRepository.Object,
                _fakeSpotRepository.Object,
                _fakeDataChangeValidator.Object,
                _fakeAuditEventRepository.Object,
                _fakeSalesAreaRepository.Object,
                null
            );
        }

        [TearDown]
        public void Cleanup()
        {
            Dispose();
        }

        public void Dispose()
        {
            _controller.Dispose();

            _mapper = null;
        }
    }
}

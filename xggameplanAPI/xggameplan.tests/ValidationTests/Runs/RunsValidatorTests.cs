using System;
using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups;
using ImagineCommunications.GamePlan.Domain.BRS;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.DeliveryCappingGroup;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.RunTypes;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using Moq;
using NUnit.Framework;
using xggameplan.Configuration;
using xggameplan.core.Configuration;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.tests.ValidationTests.Runs.Data;
using xggameplan.Validations.Runs;

namespace xggameplan.tests.ValidationTests.Runs
{
    [TestFixture(Category = "Validations :: RunsValidator")]
    public class RunsValidatorTests
    {
        private Mock<IBRSConfigurationTemplateRepository> _fakeBrsConfigurationTemplateRepository;
        private Mock<ITenantSettingsRepository> _fakeTenantSettingsRepository;
        private Mock<IRunRepository> _fakeRunRepository;
        private Mock<IScenarioRepository> _fakeScenarioRepository;
        private Mock<ISalesAreaRepository> _fakeSalesAreaRepository;
        private Mock<ICampaignRepository> _fakeCampaignRepository;
        private Mock<IDemographicRepository> _fakeDemographicRepository;
        private Mock<IDeliveryCappingGroupRepository> _fakeDeliveryCappingGroupRepository;
        private Mock<IFeatureManager> _fakeFeatureManager;
        private Mock<IRunTypeRepository> _fakeRunTypeRepository;
        private Mock<IAnalysisGroupRepository> _fakeAnalysisGroupRepository;
        private IMapper _mapper;

        private RunsValidator _runsValidator;

        [SetUp]
        public void Init()
        {
            _fakeBrsConfigurationTemplateRepository = new Mock<IBRSConfigurationTemplateRepository>();
            _fakeTenantSettingsRepository = new Mock<ITenantSettingsRepository>();
            _fakeRunRepository = new Mock<IRunRepository>();
            _fakeScenarioRepository = new Mock<IScenarioRepository>();
            _fakeSalesAreaRepository = new Mock<ISalesAreaRepository>();
            _fakeCampaignRepository = new Mock<ICampaignRepository>();
            _fakeDemographicRepository = new Mock<IDemographicRepository>();
            _fakeDeliveryCappingGroupRepository = new Mock<IDeliveryCappingGroupRepository>();
            _fakeFeatureManager = new Mock<IFeatureManager>();
            _fakeRunTypeRepository = new Mock<IRunTypeRepository>();
            _fakeAnalysisGroupRepository = new Mock<IAnalysisGroupRepository>();
            _mapper = AutoMapperInitializer.Initialize(AutoMapperConfig.SetupAutoMapper);

            _runsValidator = new RunsValidator(
                _fakeBrsConfigurationTemplateRepository.Object,
                _fakeTenantSettingsRepository.Object,
                _fakeRunRepository.Object,
                _fakeScenarioRepository.Object,
                _fakeSalesAreaRepository.Object,
                _fakeCampaignRepository.Object,
                _fakeDemographicRepository.Object,
                _fakeDeliveryCappingGroupRepository.Object,
                _fakeFeatureManager.Object,
                _fakeRunTypeRepository.Object,
                _fakeAnalysisGroupRepository.Object,
                _mapper);
        }

        #region ValidateForSave Tests

        [Test(Description = "Given all valid inputs then no exception should be thrown")]
        public void GivenAllValidInputsThenNoExceptionThown()
        {
            // Arrange
            var allPassesByScenario = new List<List<Pass>> { new List<Pass> { RunsValidationsDummyData.GetPass() } };
            _ = _fakeBrsConfigurationTemplateRepository.Setup(o => o.Exists(It.IsAny<int>())).Returns(true);
            _ = _fakeTenantSettingsRepository.Setup(o => o.Get()).Returns(RunsValidationsDummyData.GetTenantSettings());
            _ = _fakeSalesAreaRepository.Setup(o => o.FindByNames(It.IsAny<List<string>>())).Returns(RunsValidationsDummyData.GetSalesAreas());
            _ = _fakeAnalysisGroupRepository.Setup(x => x.GetByIds(It.IsAny<IEnumerable<int>>(), It.IsAny<bool>()))
                .Returns(RunsValidationsDummyData.GetAnalysisGroups());

            // Act
            Action action = () => _runsValidator.ValidateForSave(RunsValidationsDummyData.GetRun(), RunsValidationsDummyData.GetScenarios(), allPassesByScenario, RunsValidationsDummyData.GetSalesAreas());

            // Assert
            action.Should().NotThrow();
        }

        #endregion ValidateForSave Tests

        [TearDown]
        public void Cleanup()
        {
            _runsValidator = null;
            _mapper = null;
        }
    }
}

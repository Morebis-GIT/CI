using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Results;
using AutoFixture;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Autopilot.FlexibilityLevels;
using ImagineCommunications.GamePlan.Domain.Autopilot.Rules;
using ImagineCommunications.GamePlan.Domain.Autopilot.Settings;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Rules;
using ImagineCommunications.GamePlan.Domain.BusinessRules.RuleTypes;
using Moq;
using NUnit.Framework;
using xggameplan.Configuration;
using xggameplan.Controllers;
using xggameplan.core.Configuration;
using xggameplan.Model;
using xggameplan.model.External;
using xggameplan.Validations;
using xggameplan.Validations.AutopilotSettings;

namespace xggameplan.tests.ControllerTests
{
    [TestFixture(Category = "Controllers :: AutopilotSettings")]
    public class AutopilotSettingsControllerTests : IDisposable
    {
        private Mock<IAutopilotSettingsRepository> _fakeAutopilotSettingsRepository;
        private Mock<IAutopilotRuleRepository> _fakeAutopilotRuleRepository;
        private Mock<IRuleRepository> _fakeRuleRepository;
        private Mock<IRuleTypeRepository> _fakeRuleTypeRepository;
        private Mock<IFlexibilityLevelRepository> _fakeFlexibilityLevelRepository;

        private AutopilotSettingsController _controller;
        private IModelDataValidator<UpdateAutopilotSettingsModel> _modelDataValidator;
        private IMapper _mapper;
        private Fixture _fixture;

        [SetUp]
        public void Init()
        {
            _fakeAutopilotSettingsRepository = new Mock<IAutopilotSettingsRepository>();
            _fakeAutopilotRuleRepository = new Mock<IAutopilotRuleRepository>();
            _fakeRuleRepository = new Mock<IRuleRepository>();
            _fakeRuleTypeRepository = new Mock<IRuleTypeRepository>();
            _fakeFlexibilityLevelRepository = new Mock<IFlexibilityLevelRepository>();

            _fixture = new Fixture();

            _mapper = AutoMapperInitializer.Initialize(AutoMapperConfig.SetupAutoMapper);

            _modelDataValidator = new AutopilotSettingsModelValidator(new AutopilotSettingsModelValidation(_fakeFlexibilityLevelRepository.Object));

            _controller = new AutopilotSettingsController(
                _fakeAutopilotSettingsRepository.Object,
                _fakeAutopilotRuleRepository.Object,
                _fakeRuleRepository.Object,
                _fakeRuleTypeRepository.Object,
                _modelDataValidator,
                _mapper
            );
        }

        [Test]
        [Description("When getting autopilot settings then result must be successful")]
        public void GetDefaultWhenCalledThenShouldReturnOk()
        {
            _ = _fakeAutopilotSettingsRepository.Setup(r => r.GetDefault())
                .Returns(CreateAutopilotSettings());

            Assert.IsInstanceOf<OkNegotiatedContentResult<AutopilotSettingsModel>>(_controller.GetDefault());
        }

        [Test]
        [Description("Given valid model when Updating AutopilotSettings then model property must be updated")]
        public void PutDefaultWhenCalledWithValidModelThenShouldUpdateProperty()
        {
            var updatedAutopilotSettings = CreateValidModel();
            updatedAutopilotSettings.DefaultFlexibilityLevelId = 2;

            _ = _fakeFlexibilityLevelRepository.Setup(r => r.Get(updatedAutopilotSettings.DefaultFlexibilityLevelId)).Returns(new FlexibilityLevel());
            _ = _fakeAutopilotSettingsRepository.Setup(r => r.Get(CreateAutopilotSettings().Id)).Returns(CreateAutopilotSettings());

            var result = _controller.PutDefault(updatedAutopilotSettings.Id, updatedAutopilotSettings) as OkNegotiatedContentResult<AutopilotSettingsModel>;

            Assert.AreEqual(updatedAutopilotSettings.DefaultFlexibilityLevelId, result.Content.DefaultFlexibilityLevelId);
        }

        [Test]
        [Description("Given not valid AutopilotSettings when Updating then correct validation message must be returned")]
        public void PutDefaultWhenCalledWithNotValidFlexibilityLevelThenShouldReturnCorrectValidationMessage()
        {
            var autopilotSettings = CreateAutopilotSettings();
            var updatedAutopilotSettings = CreateValidModel();
            updatedAutopilotSettings.DefaultFlexibilityLevelId = 5;

            _ = _fakeAutopilotSettingsRepository.Setup(r => r.Get(autopilotSettings.Id)).Returns(CreateAutopilotSettings());

            var result = _controller.PutDefault(updatedAutopilotSettings.Id, updatedAutopilotSettings) as ResponseMessageResult;

            Assert.IsTrue(result.Response.StatusCode == HttpStatusCode.BadRequest);

            var errors = (result.Response.Content as ObjectContent<IEnumerable<ErrorModel>>)?.Value as IEnumerable<ErrorModel>;

            Assert.IsTrue(errors.Any(err => err.ErrorField == nameof(updatedAutopilotSettings.DefaultFlexibilityLevelId)));
        }

        private AutopilotSettings CreateAutopilotSettings()
        {
            return _fixture.Build<AutopilotSettings>()
                .With(r => r.Id, 1)
                .With(a => a.DefaultFlexibilityLevelId, 1)
                .With(a => a.ScenariosToGenerate, 8)
                .Create();
        }

        private UpdateAutopilotSettingsModel CreateValidModel()
        {
            var autopilotRules = _fixture.Build<UpdateAutopilotRuleModel>()
                .With(r => r.UniqueRuleKey, "1_1")
                .With(r => r.Enabled, true)
                .CreateMany(1);

            return _fixture.Build<UpdateAutopilotSettingsModel>()
                .With(r => r.Id, 1)
                .With(a => a.DefaultFlexibilityLevelId, 1)
                .With(a => a.ScenariosToGenerate, 8)
                .With(a => a.AutopilotRules, autopilotRules.ToList())
                .Create();
        }

        [TearDown]
        public void Cleanup()
        {
            _controller = null;
            _mapper = null;
        }

        public void Dispose()
        {
            _controller = null;
            _mapper = null;
        }
    }
}

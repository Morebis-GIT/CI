using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using ImagineCommunications.GamePlan.Domain.Autopilot.FlexibilityLevels;
using Moq;
using NUnit.Framework;
using xggameplan.Model;
using xggameplan.Validations;

namespace xggameplan.tests.ValidationTests
{
    [TestFixture(Category = "Validations :: AutopilotSettingsModel")]
    public class AutopilotSettingsModelValidationShould
    {
        private Mock<IFlexibilityLevelRepository> _flexibilityLevelRepositoryMock;
        private AutopilotSettingsModelValidation _target;
        private Fixture _fixture;
        private UpdateAutopilotSettingsModel _model;

        [OneTimeSetUp]
        public async Task OnInit()
        {
            _fixture = new Fixture();
            AssumeDependenciesAreSupplied();
            AssumeTargetIsInitialized();
        }

        [OneTimeTearDown]
        public async Task OnDestroy() => CleanUp();

        [SetUp]
        public async Task BeforeEach() => AssumeDefaultsAreSetup();

        [Test]
        public async Task PassWhenProvidedValidModel()
        {
            var validationResult = _target.Validate(_model);

            _ = validationResult.IsValid.Should().BeTrue();
            _ = validationResult.Errors.Count.Should().Be(0);
        }

        [Test]
        public async Task FailWhenFlexibilityLevelDoesNotExist()
        {
            _model.DefaultFlexibilityLevelId = -1;
            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
                _ = validationResult.Errors.Should().Contain(er =>
                      er.ErrorMessage == "FlexibilityLevel with identifier -1 does not exist");
            }
        }

        [Test]
        public async Task FailWhenNotValidScenarioTypesCount()
        {
            _model = BuildModel(0);
            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
                _ = validationResult.Errors.Should().Contain(er => er.ErrorMessage == "Please specify valid scenario types count to be generated");
            }
        }

        private void AssumeTargetIsInitialized()
        {
            _target = (AutopilotSettingsModelValidation)Activator.CreateInstance(typeof(AutopilotSettingsModelValidation), _flexibilityLevelRepositoryMock.Object);
        }

        private void AssumeDependenciesAreSupplied()
        {
            _flexibilityLevelRepositoryMock = new Mock<IFlexibilityLevelRepository>();
            _ = _flexibilityLevelRepositoryMock.Setup(r => r.Get(1)).Returns(new FlexibilityLevel { Id = 1, Name = "test" });
        }

        private void AssumeDefaultsAreSetup()
        {
            AssumeValidModelIsSupplied();
        }

        private void AssumeValidModelIsSupplied()
        {
            _model = BuildModel();
        }

        private UpdateAutopilotSettingsModel BuildModel(int numberOfScenarios = 8)
        {
            var autopilotRules = _fixture.Build<UpdateAutopilotRuleModel>()
                .With(r => r.Enabled, true)
                .With(r => r.UniqueRuleKey, "1_1")
                .CreateMany(1)
                .ToList();

            return _fixture.Build<UpdateAutopilotSettingsModel>()
                .With(s => s.Id, 0)
                .With(m => m.DefaultFlexibilityLevelId, 1)
                .With(m => m.ScenariosToGenerate, numberOfScenarios)
                .With(m => m.AutopilotRules, autopilotRules)
                .Create();
        }

        private void CleanUp()
        {
            _fixture = null;
            _target = null;
        }
    }
}

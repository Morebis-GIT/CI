using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using ImagineCommunications.GamePlan.Domain.Autopilot.FlexibilityLevels;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using Moq;
using NUnit.Framework;
using xggameplan.Model;
using xggameplan.Validations;

namespace xggameplan.tests.ValidationTests
{
    [TestFixture(Category = "Validations :: AutopilotEngageModel")]
    public class AutopilotEngageModelValidationShould
    {
        private Mock<IFlexibilityLevelRepository> _flexibilityLevelRepositoryMock;

        private AutopilotEngageModelValidation _target;
        private Fixture _fixture;
        private AutopilotEngageModel _model;

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
            _model.FlexibilityLevelId = -1;
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
        public async Task FailWhenMissingScenariosData()
        {
            _model.Scenarios = null;
            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
                _ = validationResult.Errors.Should().Contain(er =>
                      er.ErrorMessage == "Please provide at least one scenario");
            }
        }

        [Test]
        [TestCase("Scenario has no passes")]
        [TestCase("Please specify tighten pass index")]
        [TestCase("Please specify loosen pass index")]
        public async Task FailWhenMissingPassesData(string errorMessage)
        {
            _model = BuildModel(passesNumber: 0, tightenPassIndex: null, loosenPassIndex: null);
            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
                _ = validationResult.Errors.Should().Contain(er => er.ErrorMessage == errorMessage);
            }
        }

        [Test]
        [TestCase("Specified loosen pass index is not present in the passes collection")]
        [TestCase("Specified tighten pass index is not present in the passes collection")]
        [TestCase("Tighten pass must go first")]
        public async Task FailWhenNotValidPassIndexes(string errorMessage)
        {
            _model = BuildModel(tightenPassIndex: 12, loosenPassIndex: 10);
            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
                _ = validationResult.Errors.Should().Contain(er => er.ErrorMessage == errorMessage);
            }
        }

        private void AssumeTargetIsInitialized()
        {
            _target = (AutopilotEngageModelValidation)Activator.CreateInstance(typeof(AutopilotEngageModelValidation), _flexibilityLevelRepositoryMock.Object);
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

        private AutopilotEngageModel BuildModel(int scenariosNumber = 1, int passesNumber = 1, int? tightenPassIndex = 0,
            int? loosenPassIndex = 0)
        {
            var passes = _fixture.Build<AutopilotPassModel>()
                .With(p => p.Id, 0)
                .With(p => p.Name, "Test pass")
                .With(p => p.General,
                    new List<GeneralModel> { new GeneralModel { RuleId = (int)RuleID.MinimumEfficiency, Value = "1" } })
                .With(p => p.Weightings, new List<WeightingModel> { new WeightingModel() })
                .With(p => p.Tolerances, new List<ToleranceModel> { new ToleranceModel() })
                .With(p => p.Rules, new List<PassRuleModel> { new PassRuleModel() })
                .CreateMany(passesNumber)
                .ToList();

            var scenarios = _fixture.Build<AutopilotScenarioEngageModel>()
                .With(s => s.Name, "Test scenario")
                .With(s => s.TightenPassIndex, tightenPassIndex)
                .With(s => s.LoosenPassIndex, loosenPassIndex)
                .With(s => s.Passes, passes)
                .With(s => s.Status, ScenarioStatuses.Pending)
                .CreateMany(scenariosNumber)
                .ToList();

            return _fixture.Build<AutopilotEngageModel>()
                .With(m => m.FlexibilityLevelId, 1)
                .With(m => m.Scenarios, scenarios)
                .Create();
        }

        private void CleanUp()
        {
            _fixture = null;
            _target = null;
        }
    }
}

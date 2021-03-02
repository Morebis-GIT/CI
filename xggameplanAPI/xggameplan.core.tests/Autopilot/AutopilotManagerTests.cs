using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoFixture;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Autopilot.FlexibilityLevels;
using ImagineCommunications.GamePlan.Domain.Autopilot.Rules;
using ImagineCommunications.GamePlan.Domain.Autopilot.Settings;
using ImagineCommunications.GamePlan.Domain.BusinessRules.RuleTypes;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using Moq;
using NUnit.Framework;
using xggameplan.AuditEvents;
using xggameplan.Autopilot;
using xggameplan.Common;
using xggameplan.Configuration;
using xggameplan.core.Configuration;
using xggameplan.Model;

namespace xggameplan.tests.Autopilot
{
    [TestFixture]
    public class AutopilotManagerTests : IDisposable
    {
        private const int TightenALotPercentage = 10;
        private const int TightenABitPercentage = 5;
        private const int LoosenABitPercentage = -5;
        private const int LoosenALotPercentage = -10;
        private const int ToleranceLoosenABitPercentage = 5;
        private const int ToleranceLoosenALotPercentage = 10;
        private const int LastScenarioIndex = AutopilotManager.MaxAutopilotScenarios - 1;

        private Mock<IAutopilotRuleRepository> _mockAutopilotRuleRepository;
        private Mock<IAutopilotSettingsRepository> _mockAutopilotSettingsRepository;
        private Mock<IAuditEventRepository> _mockAuditEventRepository;
        private Fixture _fixture;
        private IMapper _mapper;

        private IAutopilotManager _autopilotManager;

        [SetUp]
        public void Init()
        {
            _fixture = new Fixture();

            _mapper = AutoMapperInitializer.Initialize(AutoMapperConfig.SetupAutoMapper);

            _mockAuditEventRepository = new Mock<IAuditEventRepository>();
            _mockAutopilotRuleRepository = new Mock<IAutopilotRuleRepository>();
            _mockAutopilotSettingsRepository = new Mock<IAutopilotSettingsRepository>();

            _ = _mockAutopilotRuleRepository.Setup(r => r.GetByFlexibilityLevelId(1)).Returns(CreateValidAutopilotRules());
            _ = _mockAutopilotSettingsRepository.Setup(r => r.GetDefault()).Returns(BuildAutopilotSettings());

            _autopilotManager =
                new AutopilotManager(_mockAuditEventRepository.Object, _mockAutopilotSettingsRepository.Object, _mockAutopilotRuleRepository.Object, _mapper);
        }

        [Test]
        [Description("Given valid model when generating autopilot scenarios then correct number of scenarios must be generated")]
        public void EngageWhenCalledWithValidModelThenShouldReturnCorrectNumberOfScenarios([Range(1, 8)] int numberOfScenarios)
        {
            var scenarios = _autopilotManager.Engage(BuildModel(numberOfScenarios));
            Assert.AreEqual(numberOfScenarios * 8, scenarios.Count);
        }

        [Test]
        [Description("Given valid model when generating autopilot scenarios then correct number of passes must be generated")]
        [TestCase(AutopilotScenarioType.First, 2)]
        [TestCase(AutopilotScenarioType.Second, 2)]
        [TestCase(AutopilotScenarioType.Third, 3)]
        [TestCase(AutopilotScenarioType.Fourth, 3)]
        [TestCase(AutopilotScenarioType.Fifth, 3)]
        [TestCase(AutopilotScenarioType.Sixth, 4)]
        [TestCase(AutopilotScenarioType.Seventh, 4)]
        [TestCase(AutopilotScenarioType.Eighth, 5)]
        public void EngageWhenCalledWithValidModelThenShouldReturnScenariosWithNumberOfPasses(int scenarioTypeNumber, int numberOfPasses)
        {
            var scenario = _autopilotManager.Engage(BuildModel())[scenarioTypeNumber];
            Assert.AreEqual(scenario.Passes.Count, numberOfPasses);
        }

        [Test]
        [Description("Given valid model when generating autopilot scenarios then passes on the right positions must be returned")]
        [TestCase(AutopilotScenarioType.First, -1, 0, -1, -1)]
        [TestCase(AutopilotScenarioType.Second, -1, -1, 1, -1)]
        [TestCase(AutopilotScenarioType.Third, -1, 0, 2, -1)]
        [TestCase(AutopilotScenarioType.Fourth, 0, 1, -1, -1)]
        [TestCase(AutopilotScenarioType.Fifth, -1, -1, 1, 2)]
        [TestCase(AutopilotScenarioType.Sixth, 0, 1, 3, -1)]
        [TestCase(AutopilotScenarioType.Seventh, -1, 0, 2, 3)]
        [TestCase(AutopilotScenarioType.Eighth, 0, 1, 3, 4)]
        public void EngageWhenCalledWithValidModelThenShouldReturnPassesOnTheRightPositions(int scenarioTypeNumber,
            int tightenALotIndex, int tightenABitIndex, int loosenABitIndex, int loosenALotIndex)
        {
            var scenario = _autopilotManager.Engage(BuildModel())[scenarioTypeNumber];

            var tightenALotPass = scenario.Passes.SingleOrDefault(p => p.AutopilotType == AutopilotPassType.TightenALot);
            var tightenABitPass = scenario.Passes.SingleOrDefault(p => p.AutopilotType == AutopilotPassType.TightenABit);
            var loosenABitPass = scenario.Passes.SingleOrDefault(p => p.AutopilotType == AutopilotPassType.LoosenABit);
            var loosenALotPass = scenario.Passes.SingleOrDefault(p => p.AutopilotType == AutopilotPassType.LoosenALot);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(tightenALotIndex, scenario.Passes.IndexOf(tightenALotPass));
                Assert.AreEqual(tightenABitIndex, scenario.Passes.IndexOf(tightenABitPass));
                Assert.AreEqual(loosenABitIndex, scenario.Passes.IndexOf(loosenABitPass));
                Assert.AreEqual(loosenALotIndex, scenario.Passes.IndexOf(loosenALotPass));
            });
        }

        [Test]
        [Description("Given valid model when generating autopilot scenarios then passes with tightened general rule values must be returned")]
        [TestCase(TightenALotPercentage, 0)]
        [TestCase(TightenABitPercentage, 1)]
        public void EngageWhenCalledWithValidModelThenShouldReturnGeneralRulesWithTightenedValues(int percentage, int passIndex)
        {
            var model = BuildModel();
            var scenario = _autopilotManager.Engage(model)[LastScenarioIndex];
            var basePass = model.Scenarios.First().Passes[0];
            var baseGeneralValue = int.Parse(basePass.General[0].Value, CultureInfo.InvariantCulture);

            var adjustedPass = scenario.Passes[passIndex];
            var adjustedGeneralValue = int.Parse(adjustedPass.General[0].Value, CultureInfo.InvariantCulture);
            var adjustedPercentage = CalculatePercentageDiff(baseGeneralValue, adjustedGeneralValue);

            Assert.GreaterOrEqual(adjustedPercentage, percentage);
        }

        [Test]
        [Description("Given valid model when generating autopilot scenarios then passes with loosen general rule values must be returned")]
        [TestCase(LoosenABitPercentage, 3)]
        [TestCase(LoosenALotPercentage, 4)]
        public void EngageWhenCalledWithValidModelThenShouldReturnGeneralRulesWithLoosenValues(int percentage, int passIndex)
        {
            var model = BuildModel();
            var scenario = _autopilotManager.Engage(model)[LastScenarioIndex];
            var basePass = model.Scenarios.First().Passes[0];
            var baseGeneralValue = int.Parse(basePass.General[0].Value, CultureInfo.InvariantCulture);

            var adjustedPass = scenario.Passes[passIndex];
            var adjustedGeneralValue = int.Parse(adjustedPass.General[0].Value, CultureInfo.InvariantCulture);
            var adjustedPercentage = CalculatePercentageDiff(baseGeneralValue, adjustedGeneralValue);

            Assert.LessOrEqual(adjustedPercentage, percentage);
        }

        [Test]
        [Description("Given valid model when generating autopilot scenarios then passes with tightened tolerance rule values must be returned")]
        [TestCase(TightenALotPercentage, -TightenALotPercentage, 0)]
        [TestCase(TightenABitPercentage, -TightenABitPercentage, 1)]
        public void EngageWhenCalledWithValidModelThenShouldReturnToleranceRulesWithTightenedValues(int percentageUnder,
            int percentageOver, int passIndex)
        {
            var model = BuildModel();
            var scenario = _autopilotManager.Engage(model)[LastScenarioIndex];
            var basePass = model.Scenarios.First().Passes[0];
            var baseUnderValue = basePass.Tolerances[0].Under;
            var baseOverValue = basePass.Tolerances[0].Over;

            var adjustedPass = scenario.Passes[passIndex];
            var adjustedUnderValue = adjustedPass.Tolerances[0].Under;
            var adjustedOverValue = adjustedPass.Tolerances[0].Over;

            Assert.Multiple(() =>
            {
                Assert.GreaterOrEqual(CalculatePercentageDiff(baseUnderValue, adjustedUnderValue), percentageUnder);
                Assert.LessOrEqual(CalculatePercentageDiff(baseOverValue, adjustedOverValue), percentageOver);
            });
        }

        [Test]
        [Description("Given valid model when generating autopilot scenarios then passes with loosen tolerance rule values must be returned")]
        [TestCase(-ToleranceLoosenABitPercentage, ToleranceLoosenABitPercentage, 3)]
        [TestCase(-ToleranceLoosenALotPercentage, ToleranceLoosenALotPercentage, 4)]
        public void EngageWhenCalledWithValidModelThenShouldReturnToleranceRulesWithLoosenValues(int percentageUnder,
            int percentageOver, int passIndex)
        {
            var model = BuildModel();
            var scenario = _autopilotManager.Engage(model)[LastScenarioIndex];
            var basePass = model.Scenarios.First().Passes[0];
            var baseUnderValue = basePass.Tolerances[0].Under;
            var baseOverValue = basePass.Tolerances[0].Over;

            var adjustedPass = scenario.Passes[passIndex];
            var adjustedUnderValue = adjustedPass.Tolerances[0].Under;
            var adjustedOverValue = adjustedPass.Tolerances[0].Over;

            Assert.Multiple(() =>
            {
                Assert.LessOrEqual(CalculatePercentageDiff(baseUnderValue, adjustedUnderValue), percentageUnder);
                Assert.GreaterOrEqual(CalculatePercentageDiff(baseOverValue, adjustedOverValue), percentageOver);
            });
        }

        [Test]
        [Description("Given valid model when generating autopilot scenarios then passes with tightened rule values must be returned")]
        [TestCase(TightenALotPercentage, 0)]
        [TestCase(TightenABitPercentage, 1)]
        public void EngageWhenCalledWithValidModelThenShouldReturnRulesWithTightenedValues(int percentage, int passIndex)
        {
            var model = BuildModel();
            var scenario = _autopilotManager.Engage(model)[LastScenarioIndex];
            var basePass = model.Scenarios.First().Passes[0];
            var baseRuleValue = int.Parse(basePass.Rules[0].Value, CultureInfo.InvariantCulture);

            var adjustedPass = scenario.Passes[passIndex];
            var adjustedRuleValue = int.Parse(adjustedPass.Rules[0].Value, CultureInfo.InvariantCulture);
            var adjustedPercentage = CalculatePercentageDiff(baseRuleValue, adjustedRuleValue);

            Assert.GreaterOrEqual(adjustedPercentage, percentage);
        }

        [Test]
        [Description("Given valid model when generating autopilot scenarios then passes with loosen rule values must be returned")]
        [TestCase(LoosenABitPercentage, 3)]
        [TestCase(LoosenALotPercentage, 4)]
        public void EngageWhenCalledWithValidModelThenShouldReturnRulesWithLoosenValues(int percentage, int passIndex)
        {
            var model = BuildModel();
            var scenario = _autopilotManager.Engage(model)[LastScenarioIndex];
            var basePass = model.Scenarios.First().Passes[0];
            var baseRuleValue = int.Parse(basePass.Rules[0].Value, CultureInfo.InvariantCulture);

            var adjustedPass = scenario.Passes[passIndex];
            var adjustedRuleValue = int.Parse(adjustedPass.Rules[0].Value, CultureInfo.InvariantCulture);
            var adjustedPercentage = CalculatePercentageDiff(baseRuleValue, adjustedRuleValue);

            Assert.LessOrEqual(adjustedPercentage, percentage);
        }

        [Test]
        [Description("Given valid model when generating autopilot scenarios then passes with tightened slotting limits values must be returned")]
        [TestCase(TightenALotPercentage, 0)]
        [TestCase(TightenABitPercentage, 1)]
        public void EngageWhenCalledWithValidModelThenShouldReturnSlottingLimitsWithTightenedValues(int percentage, int passIndex)
        {
            var model = BuildModel();
            var scenario = _autopilotManager.Engage(model)[LastScenarioIndex];
            var basePass = model.Scenarios.First().Passes[0];

            var adjustedPass = scenario.Passes[passIndex];

            var baseMinimumEfficiency = int.Parse(basePass.General[0].Value, CultureInfo.InvariantCulture);
            var baseMaximumEfficiency = int.Parse(basePass.General[1].Value, CultureInfo.InvariantCulture);
            var baseBandingTolerance = int.Parse(basePass.General[2].Value, CultureInfo.InvariantCulture);

            var adjustedMinimumEfficiency = int.Parse(adjustedPass.General[0].Value, CultureInfo.InvariantCulture);
            var adjustedMaximumEfficiency = int.Parse(adjustedPass.General[1].Value, CultureInfo.InvariantCulture);
            var adjustedBandingTolerance = int.Parse(adjustedPass.General[2].Value, CultureInfo.InvariantCulture);

            Assert.Multiple(() =>
            {
                Assert.GreaterOrEqual(CalculatePercentageDiff(baseMinimumEfficiency, adjustedMinimumEfficiency), percentage);
                Assert.GreaterOrEqual(CalculatePercentageDiff(baseMaximumEfficiency, adjustedMaximumEfficiency), percentage);
                Assert.GreaterOrEqual(CalculatePercentageDiff(baseBandingTolerance, adjustedBandingTolerance), percentage);
            });
        }

        [Test]
        [Description("Given valid model when generating autopilot scenarios then passes with loosen slotting limits values must be returned")]
        [TestCase(LoosenABitPercentage, 3)]
        [TestCase(LoosenALotPercentage, 4)]
        public void EngageWhenCalledWithValidModelThenShouldReturnSlottingLimitsWithLoosenValues(int percentage, int passIndex)
        {
            var model = BuildModel();
            var scenario = _autopilotManager.Engage(model)[LastScenarioIndex];
            var basePass = model.Scenarios.First().Passes[0];

            var adjustedPass = scenario.Passes[passIndex];

            var baseMinimumEfficiency = int.Parse(basePass.General[0].Value, CultureInfo.InvariantCulture);
            var baseMaximumEfficiency = int.Parse(basePass.General[1].Value, CultureInfo.InvariantCulture);
            var baseBandingTolerance = int.Parse(basePass.General[2].Value, CultureInfo.InvariantCulture);

            var adjustedMinimumEfficiency = int.Parse(adjustedPass.General[0].Value, CultureInfo.InvariantCulture);
            var adjustedMaximumEfficiency = int.Parse(adjustedPass.General[1].Value, CultureInfo.InvariantCulture);
            var adjustedBandingTolerance = int.Parse(adjustedPass.General[2].Value, CultureInfo.InvariantCulture);

            Assert.Multiple(() =>
            {
                Assert.LessOrEqual(CalculatePercentageDiff(baseMinimumEfficiency, adjustedMinimumEfficiency), percentage);
                Assert.LessOrEqual(CalculatePercentageDiff(baseMaximumEfficiency, adjustedMaximumEfficiency), percentage);
                Assert.LessOrEqual(CalculatePercentageDiff(baseBandingTolerance, adjustedBandingTolerance), percentage);
            });
        }

        [Test]
        [Description("Given excluded rule when generating autopilot scenarios then rule value must not be changed")]
        public void EngageWhenCalledWithExcludedRuleThenShouldReturnTheSameRuleValue()
        {
            var autopilotRules = CreateValidAutopilotRules().ToList();

            foreach (var autopilotRule in autopilotRules.Where(r => r.RuleTypeId == (int)RuleCategory.General))
            {
                autopilotRule.Enabled = false;
            }

            _ = _mockAutopilotRuleRepository.Setup(r => r.GetByFlexibilityLevelId(1)).Returns(autopilotRules);

            var model = BuildModel();
            var basePass = model.Scenarios.First().Passes[0];
            var adjustedPass = _autopilotManager.Engage(model)[0].Passes[0];

            Assert.AreEqual(basePass.General[0].Value, adjustedPass.General[0].Value);
        }

        private static double CalculatePercentageDiff(int baseValue, int adjustedValue) => (adjustedValue - baseValue) / (double)baseValue * 100;

        private AutopilotSettings BuildAutopilotSettings(int numberOfScenarioTypes = 8)
        {
            return _fixture.Build<AutopilotSettings>()
                .With(r => r.Id, 1)
                .With(a => a.DefaultFlexibilityLevelId, 1)
                .With(a => a.ScenariosToGenerate, numberOfScenarioTypes)
                .Create();
        }

        private AutopilotEngageModel BuildModel(int numberOfScenarios = 1)
        {
            var passes = _fixture.Build<AutopilotPassModel>()
                .With(p => p.Id, 0)
                .With(p => p.Name, "Test pass")
                .With(p => p.General,
                    new List<GeneralModel>
                    {
                        new GeneralModel {RuleId = 1, Value = "11"},
                        new GeneralModel {RuleId = 2, Value = "12"},
                        new GeneralModel {RuleId = 3, Value = "14"}
                    })
                .With(p => p.Weightings, new List<WeightingModel> { new WeightingModel() })
                .With(p => p.Tolerances,
                    new List<ToleranceModel> { new ToleranceModel { RuleId = 1, Under = 10, Over = 15 } })
                .With(p => p.Rules,
                    new List<PassRuleModel> { new PassRuleModel { RuleId = 1, Value = "11" } })
                .CreateMany(1)
                .ToList();

            var scenarios = _fixture.Build<AutopilotScenarioEngageModel>()
                .With(s => s.Name, "Test scenario")
                .With(s => s.TightenPassIndex, 0)
                .With(s => s.LoosenPassIndex, 0)
                .With(s => s.Passes, passes)
                .With(s => s.Status, ScenarioStatuses.Pending)
                .CreateMany(numberOfScenarios)
                .ToList();

            return _fixture.Build<AutopilotEngageModel>()
                .With(m => m.FlexibilityLevelId, 1)
                .With(m => m.Scenarios, scenarios)
                .Create();
        }

        public static IEnumerable<AutopilotRule> CreateValidAutopilotRules(AutopilotFlexibilityLevel flexibilityLevel = AutopilotFlexibilityLevel.Low)
        {
            return new List<AutopilotRule>
            {
                AutopilotRule.Create((int) flexibilityLevel, 1, (int) RuleCategory.General,
                    TightenABitPercentage, LoosenABitPercentage, TightenALotPercentage, LoosenALotPercentage),
                AutopilotRule.Create((int) flexibilityLevel, 2, (int) RuleCategory.General,
                    TightenABitPercentage, LoosenABitPercentage, TightenALotPercentage, LoosenALotPercentage),
                AutopilotRule.Create((int) flexibilityLevel, 3, (int) RuleCategory.General,
                    TightenABitPercentage, LoosenABitPercentage, TightenALotPercentage, LoosenALotPercentage),
                AutopilotRule.Create((int) flexibilityLevel, 1, (int) RuleCategory.Tolerances,
                    TightenABitPercentage, ToleranceLoosenABitPercentage, TightenALotPercentage, ToleranceLoosenALotPercentage),
                AutopilotRule.Create((int) flexibilityLevel, 1, (int) RuleCategory.Rules,
                    TightenABitPercentage, LoosenABitPercentage, TightenALotPercentage, LoosenALotPercentage),
                AutopilotRule.Create((int) flexibilityLevel, 1, (int) RuleCategory.SlottingLimits,
                    TightenABitPercentage, LoosenABitPercentage, TightenALotPercentage, LoosenALotPercentage)
            };
        }

        [TearDown]
        public void Cleanup()
        {
            _mapper = null;
        }

        public void Dispose()
        {
            _mapper = null;
        }
    }
}

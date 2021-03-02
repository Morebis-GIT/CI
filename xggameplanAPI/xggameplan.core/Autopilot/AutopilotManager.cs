using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Autopilot.Rules;
using ImagineCommunications.GamePlan.Domain.Autopilot.Settings;
using ImagineCommunications.GamePlan.Domain.BusinessRules.RuleTypes;
using xggameplan.AuditEvents;
using xggameplan.Common;
using xggameplan.Model;

namespace xggameplan.Autopilot
{
    public class AutopilotManager : IAutopilotManager
    {
        public const int MaxAutopilotScenarios = 8;

        private readonly IAuditEventRepository _auditEventRepository;
        private readonly IAutopilotSettingsRepository _autopilotSettingsRepository;
        private readonly IAutopilotRuleRepository _autopilotRuleRepository;
        private readonly IMapper _mapper;

        public AutopilotManager(IAuditEventRepository auditEventRepository,
            IAutopilotSettingsRepository autopilotSettingsRepository, IAutopilotRuleRepository autopilotRuleRepository,
            IMapper mapper)
        {
            _auditEventRepository = auditEventRepository;
            _autopilotSettingsRepository = autopilotSettingsRepository;
            _autopilotRuleRepository = autopilotRuleRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Generates list of scenarios for the specified scenario types
        /// </summary>
        /// <param name="command">Model with base scenarios and settings</param>
        /// <returns>List of generated scenarios</returns>
        public IList<AutopilotScenarioModel> Engage(AutopilotEngageModel command)
        {
            var autopilotSettings = _autopilotSettingsRepository.GetDefault();
            var flexibilityLevelId = command.FlexibilityLevelId != 0
                ? command.FlexibilityLevelId
                : autopilotSettings.DefaultFlexibilityLevelId;
            var autopilotRules = GetAutopilotRules(flexibilityLevelId);
            var generatedScenarios = new List<AutopilotScenarioModel>();
            var now = DateTime.UtcNow;
            var scenarioTypesToGenerate = Enum.GetValues(typeof(AutopilotScenarioType))
                .Cast<AutopilotScenarioType>()
                .Skip(MaxAutopilotScenarios - autopilotSettings.ScenariosToGenerate)
                .ToList();

            foreach (var scenarioModel in command.Scenarios)
            {
                var (tightenPassIndex, loosenPassIndex) = scenarioModel.GetAutopilotPassIndexes();
                var tightenPass = scenarioModel.Passes[tightenPassIndex];
                var loosenPass = scenarioModel.Passes[loosenPassIndex];

                if (tightenPassIndex == loosenPassIndex)
                {
                    tightenPass.AutopilotType = AutopilotPassType.TightenAndLoosenFrom;
                }
                else
                {
                    tightenPass.AutopilotType = AutopilotPassType.TightenFrom;
                    loosenPass.AutopilotType = AutopilotPassType.LoosenFrom;
                }

                var autopilotPasses = new Dictionary<AutopilotPassType, AutopilotPassModel>
                {
                    {
                        AutopilotPassType.TightenALot,
                        BuildAutopilotPass(tightenPass, autopilotRules, AutopilotPassType.TightenALot, now, "AP Tighten A Lot Pass")
                    },
                    {
                        AutopilotPassType.TightenABit,
                        BuildAutopilotPass(tightenPass, autopilotRules, AutopilotPassType.TightenABit, now, "AP Tighten A Bit Pass")
                    },
                    {
                        AutopilotPassType.LoosenABit,
                        BuildAutopilotPass(loosenPass, autopilotRules, AutopilotPassType.LoosenABit, now, "AP Loosen A Bit Pass")
                    },
                    {
                        AutopilotPassType.LoosenALot,
                        BuildAutopilotPass(loosenPass, autopilotRules, AutopilotPassType.LoosenALot, now, "AP Loosen A Lot Pass")
                    }
                };

                foreach (var scenarioType in scenarioTypesToGenerate)
                {
                    generatedScenarios.Add(GenerateAutopilotScenarioByType(scenarioType, scenarioModel, autopilotPasses));
                }
            }

            return generatedScenarios;
        }

        /// <summary>
        /// Gets collection of valid autopilot rules for all needed rule categories
        /// </summary>
        /// <param name="flexibilityLevelId"></param>
        /// <returns>ReadOnly collection of autopilot rules</returns>
        private IReadOnlyDictionary<int, List<AutopilotRule>> GetAutopilotRules(int flexibilityLevelId)
        {
            var autopilotRules = _autopilotRuleRepository.GetByFlexibilityLevelId(flexibilityLevelId).ToList();

            string exceptionMessage = null;

            if (!autopilotRules.Any())
            {
                exceptionMessage = $"Autopilot rules missing for '{flexibilityLevelId}' flexibility level";
            }

            var rulesByType = autopilotRules
                .Where(ar => ar.Enabled || ar.RuleTypeId == (int)RuleCategory.SlottingLimits)
                .GroupBy(ar => ar.RuleTypeId)
                .ToDictionary(g => g.Key, g => g.ToList());

            if (!rulesByType.ContainsKey((int)RuleCategory.SlottingLimits) ||
                rulesByType[(int)RuleCategory.SlottingLimits].Count != 1)
            {
                exceptionMessage = "Missing default SlottingLimits Autopilot rule";
            }

            if (exceptionMessage != null)
            {
                var exception = new Exception("Error while generating autopilot scenarios");
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, exceptionMessage, exception));
                throw exception;
            }

            // default values for all rule categories
            foreach (var ruleCategory in Enum.GetValues(typeof(RuleCategory)).Cast<RuleCategory>())
            {
                if (!rulesByType.ContainsKey((int)ruleCategory))
                {
                    rulesByType[(int)ruleCategory] = new List<AutopilotRule>();
                }
            }

            return rulesByType;
        }

        /// <summary>
        /// Builds autopilot scenario of the specified type by putting newly generated passes into the correct positions
        /// </summary>
        /// <param name="scenarioType"></param>
        /// <param name="baseScenario"></param>
        /// <param name="autopilotPasses">Autopilot passes, grouped by pass type</param>
        /// <returns>Generated scenario with the expanded list of passes</returns>
        private AutopilotScenarioModel GenerateAutopilotScenarioByType(AutopilotScenarioType scenarioType,
            AutopilotScenarioEngageModel baseScenario, IReadOnlyDictionary<AutopilotPassType, AutopilotPassModel> autopilotPasses)
        {
            var newScenarioModel = (AutopilotScenarioEngageModel)baseScenario.Clone();
            newScenarioModel.Id = Guid.Empty;
            newScenarioModel.Name += $", #{(int)scenarioType + 1} AP type";
            newScenarioModel.IsAutopilot = true;

            var (tightenPassIndex, loosenPassIndex) = baseScenario.GetAutopilotPassIndexes();
            loosenPassIndex++;

            var tightenALotPass = (AutopilotPassModel)autopilotPasses[AutopilotPassType.TightenALot].Clone();
            var tightenABitPass = (AutopilotPassModel)autopilotPasses[AutopilotPassType.TightenABit].Clone();
            var loosenABitPass = (AutopilotPassModel)autopilotPasses[AutopilotPassType.LoosenABit].Clone();
            var loosenALotPass = (AutopilotPassModel)autopilotPasses[AutopilotPassType.LoosenALot].Clone();

            switch (scenarioType)
            {
                case AutopilotScenarioType.First:
                    newScenarioModel.Passes.Insert(tightenPassIndex, tightenABitPass);
                    break;
                case AutopilotScenarioType.Second:
                    newScenarioModel.Passes.Insert(loosenPassIndex, loosenABitPass);
                    break;
                case AutopilotScenarioType.Third:
                    newScenarioModel.Passes.Insert(loosenPassIndex, loosenABitPass);
                    newScenarioModel.Passes.Insert(tightenPassIndex, tightenABitPass);
                    break;
                case AutopilotScenarioType.Fourth:
                    newScenarioModel.Passes.Insert(tightenPassIndex, tightenABitPass);
                    newScenarioModel.Passes.Insert(tightenPassIndex, tightenALotPass);
                    break;
                case AutopilotScenarioType.Fifth:
                    newScenarioModel.Passes.Insert(loosenPassIndex, loosenALotPass);
                    newScenarioModel.Passes.Insert(loosenPassIndex, loosenABitPass);
                    break;
                case AutopilotScenarioType.Sixth:
                    newScenarioModel.Passes.Insert(loosenPassIndex, loosenABitPass);
                    newScenarioModel.Passes.Insert(tightenPassIndex, tightenABitPass);
                    newScenarioModel.Passes.Insert(tightenPassIndex, tightenALotPass);
                    break;
                case AutopilotScenarioType.Seventh:
                    newScenarioModel.Passes.Insert(loosenPassIndex, loosenALotPass);
                    newScenarioModel.Passes.Insert(loosenPassIndex, loosenABitPass);
                    newScenarioModel.Passes.Insert(tightenPassIndex, tightenABitPass);
                    break;
                case AutopilotScenarioType.Eighth:
                    newScenarioModel.Passes.Insert(loosenPassIndex, loosenALotPass);
                    newScenarioModel.Passes.Insert(loosenPassIndex, loosenABitPass);
                    newScenarioModel.Passes.Insert(tightenPassIndex, tightenABitPass);
                    newScenarioModel.Passes.Insert(tightenPassIndex, tightenALotPass);
                    break;
                default:
                    var exception = new Exception("Error while generating autopilot scenario");
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0,
                        "Attempt to process unknown autopilot scenario type", exception));
                    throw exception;
            }

            var originalPassNames = newScenarioModel.AddUniquePassNames();
            newScenarioModel.UpdateCampaignPassPriorities(originalPassNames);

            return _mapper.Map<AutopilotScenarioModel>(newScenarioModel);
        }

        /// <summary>
        /// Builds autopilot pass using rules and adjustment values
        /// </summary>
        /// <param name="basePass"></param>
        /// <param name="autopilotRules"></param>
        /// <param name="passType"></param>
        /// <param name="createDateTime"></param>
        /// <param name="passName"></param>
        /// <returns>Pass with adjusted rule values</returns>
        private static AutopilotPassModel BuildAutopilotPass(AutopilotPassModel basePass,
            IReadOnlyDictionary<int, List<AutopilotRule>> autopilotRules, AutopilotPassType passType,
            DateTime createDateTime, string passName)
        {
            var newPass = (AutopilotPassModel)basePass.Clone();
            newPass.Id = 0;
            newPass.Name = passName;
            newPass.DateCreated = createDateTime;
            newPass.AutopilotType = passType;

            var generalRules = autopilotRules[(int)RuleCategory.General];
            var slottingLimitsRuleEnabled = autopilotRules[(int)RuleCategory.SlottingLimits].First().Enabled;

            foreach (var generalRule in generalRules)
            {
                var rule = newPass.General.FirstOrDefault(tpg => tpg.RuleId == generalRule.RuleId);
                if (rule is null)
                {
                    continue;
                }

                rule.Value = UpdateRuleValue(rule.Value, generalRule.GetAdjustmentValue(passType));
            }

            foreach (var toleranceRule in autopilotRules[(int)RuleCategory.Tolerances])
            {
                var rule = newPass.Tolerances.FirstOrDefault(tpg => tpg.RuleId == toleranceRule.RuleId);
                if (rule is null)
                {
                    continue;
                }

                var adjustmentValue = toleranceRule.GetAdjustmentValue(passType);

                switch (passType)
                {
                    case AutopilotPassType.TightenALot:
                    case AutopilotPassType.TightenABit:
                        rule.Under = UpdateRuleValue(rule.Under, adjustmentValue);
                        rule.Over = UpdateRuleValue(rule.Over, -adjustmentValue);
                        break;
                    case AutopilotPassType.LoosenABit:
                    case AutopilotPassType.LoosenALot:
                        rule.Under = UpdateRuleValue(rule.Under, -adjustmentValue);
                        rule.Over = UpdateRuleValue(rule.Over, adjustmentValue);
                        break;
                }
            }

            foreach (var rulesRule in autopilotRules[(int)RuleCategory.Rules])
            {
                var rule = newPass.Rules.FirstOrDefault(tpg => tpg.RuleId == rulesRule.RuleId);
                if (rule is null)
                {
                    continue;
                }

                var adjustmentValue = rulesRule.GetAdjustmentValue(passType);

                rule.Value = UpdateRuleValue(rule.Value, adjustmentValue);
                rule.PeakValue = UpdateRuleValue(rule.PeakValue, adjustmentValue);
            }

            if (slottingLimitsRuleEnabled)
            {
                var generalMinimumEfficiency = generalRules.FirstOrDefault(gr => gr.RuleId == 1)?.GetAdjustmentValue(passType) ?? 0;
                var generalMaximumRank = generalRules.FirstOrDefault(gr => gr.RuleId == 2)?.GetAdjustmentValue(passType) ?? 0;
                var generalDemographBandingTolerance = generalRules.FirstOrDefault(gr => gr.RuleId == 3)?.GetAdjustmentValue(passType) ?? 0;

                foreach (var slRule in newPass.SlottingLimits)
                {
                    slRule.MinimumEfficiency = UpdateRuleValue(slRule.MinimumEfficiency, generalMinimumEfficiency);
                    slRule.MaximumEfficiency = UpdateRuleValue(slRule.MaximumEfficiency, generalMaximumRank);
                    slRule.BandingTolerance = UpdateRuleValue(slRule.BandingTolerance, generalDemographBandingTolerance);
                }
            }

            return newPass;
        }

        /// <summary>
        /// Adjusts rule value to a certain percentage
        /// </summary>
        /// <param name="ruleValue">Original value</param>
        /// <param name="percentageChange">Adjustment value</param>
        /// <returns>Adjusted rule value</returns>
        private static int UpdateRuleValue(int ruleValue, int percentageChange)
        {
            if (percentageChange == 0)
            {
                return ruleValue;
            }

            var newRuleValue = ruleValue + ruleValue * (percentageChange / 100d);

            return newRuleValue >= ruleValue
                ? (int)Math.Ceiling(newRuleValue)
                : (int)Math.Floor(newRuleValue);
        }

        private static string UpdateRuleValue(string stringValue, int percentageChange)
        {
            return int.TryParse(stringValue, out var intValue)
                ? UpdateRuleValue(intValue, percentageChange).ToString(CultureInfo.InvariantCulture)
                : stringValue;
        }
    }
}

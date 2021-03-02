using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups.Objects;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects;
using ImagineCommunications.GamePlan.Domain.Autopilot.Rules;
using ImagineCommunications.GamePlan.Domain.Autopilot.Settings;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Rules;
using ImagineCommunications.GamePlan.Domain.BusinessRules.RuleTypes;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Settings;
using ImagineCommunications.GamePlan.Domain.RunTypes.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignMetrics;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignMetrics.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Domain.Shared.System.Users;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using Raven.Abstractions.Extensions;
using Raven.Client.Linq;
using xggameplan.AuditEvents;
using xggameplan.core.Services;
using xggameplan.model.External;
using xggameplan.Model;

namespace xggameplan.Services
{
    /// <summary>
    /// Mappings from external model to internal model.
    ///
    /// NOTE: For xGS then we use IMapper to map from internal to external but we don't use it for the reverse. It may be because the
    /// external model may be a subset of properties from the internal model and so we're only transferring some properties from the
    /// external model to an existing (rather than new instance of) internal model.
    /// </summary>
    internal class Mappings
    {
        public static void ApplyToScenario(Scenario scenario, ScenarioModel command, IMapper mapper)
        {
            scenario.Id = (command.Id != Guid.Empty ? command.Id : scenario.Id);
            scenario.Name = command.Name;

            scenario.CampaignPriorityRounds = mapper.Map<CampaignPriorityRounds>(command.CampaignPriorityRounds);

            var oldPasses = new List<PassReference>();

            scenario.Passes.ForEach(item => oldPasses.Add(item));
            scenario.Passes.Clear();

            foreach (var passModel in command.Passes)
            {
                var oldPass = oldPasses.Find(item => item.Id == passModel.Id);

                if (oldPass == null)    // New pass
                {
                    var pass = MapToPassReference(passModel);
                    scenario.Passes.Add(pass);
                }
                else
                {
                    ApplyToPassReference(oldPass, passModel);
                    scenario.Passes.Add(oldPass);
                }
            }
        }

        public static void ApplyChangesToScenarioAndSetIds(Scenario scenario, CreateRunScenarioModel createRunScenarioModel,
                                                           List<CampaignWithProductFlatModel> campaigns, Guid runScenarioId,
                                                           IIdentityGeneratorResolver identityGeneratorResolver,
                                                           IMapper mapper)
        {
            if (scenario != null && createRunScenarioModel != null)
            {
                scenario.Id = createRunScenarioModel.Id != Guid.Empty ? createRunScenarioModel.Id : (scenario.Id != Guid.Empty ? scenario.Id : runScenarioId);
                scenario.Name = createRunScenarioModel.Name;

                var oldPasses = new List<PassReference>();
                oldPasses.AddRange(scenario.Passes);
                scenario.Passes.Clear();

                foreach (PassModel passModel in createRunScenarioModel.Passes)
                {
                    PassReference oldPass = oldPasses.FirstOrDefault(item => item.Id == passModel.Id);
                    if (oldPass == null)    // New pass
                    {
                        oldPass = MapToPassReference(passModel);
                    }
                    else
                    {
                        ApplyToPassReference(oldPass, passModel);
                    }

                    IdUpdater.SetIdForPasssReference(oldPass, identityGeneratorResolver);
                    //reset the pass id in CampaignPassPriorities for those passes matching the pass name
                    createRunScenarioModel.CampaignPassPriorities.SelectMany(c => c.PassPriorities)
                                                  .Where(a => a.PassName.Trim().ToUpperInvariant() == passModel.Name.Trim().ToUpperInvariant())
                                                  .ForEach((p) =>
                                                  {
                                                      p.PassId = oldPass.Id;
                                                      p.PassName = p.PassName.Trim();
                                                  });
                    scenario.Passes.Add(oldPass);
                }

                var campaignPassPriorities = mapper.Map<List<CampaignPassPriority>>(Tuple.Create(createRunScenarioModel.CampaignPassPriorities, campaigns));

                var campaignPriorityRounds = mapper.Map<CampaignPriorityRounds>(createRunScenarioModel.CampaignPriorityRounds);

                scenario.CampaignPassPriorities = campaignPassPriorities;

                scenario.CampaignPriorityRounds = campaignPriorityRounds;

                IdUpdater.SetIdForScenario(scenario, identityGeneratorResolver);
            }
        }

        public static void ApplyToScenario(Scenario scenario, CreateScenarioModel command)
        {
            scenario.Id = (command.Id != Guid.Empty ? command.Id : scenario.Id);

            scenario.Name = command.Name;

            scenario.Passes.Clear();

            command.Passes.ForEach(passModel
                => scenario.Passes.Add(MapToPassReference(passModel)));
        }

        public static void ApplyToPassReference(PassReference passReference, PassModel command)
        {
            passReference.Id = (command.Id > 0 ? command.Id : passReference.Id);
        }

        public static void ApplyToPass(Pass pass, PassModel command, IMapper mapper)
        {
            if (pass != null && command != null)
            {
                pass.Id = command.Id > 0 ? command.Id : pass.Id;
                pass.Name = command.Name;

                List<General> oldGenerals = new List<General>();
                pass.General.ForEach(item => oldGenerals.Add(item));
                pass.General.Clear();
                foreach (GeneralModel generalModel in command.General)
                {
                    General oldGeneral = oldGenerals.Find(item => item.RuleId == generalModel.RuleId);
                    if (oldGeneral == null)
                    {
                        General general = MapToGeneral(generalModel);
                        pass.General.Add(general);
                    }
                    else
                    {
                        ApplyToGeneral(oldGeneral, generalModel);
                        pass.General.Add(oldGeneral);
                    }
                }

                List<PassRule> oldRules = new List<PassRule>();
                pass.Rules.ForEach(item => oldRules.Add(item));
                pass.Rules.Clear();
                foreach (PassRuleModel ruleModel in command.Rules)
                {
                    PassRule oldRule = oldRules.Find(item => item.RuleId == ruleModel.RuleId);
                    if (oldRule == null)        // New rule
                    {
                        PassRule rule = MapToRule(ruleModel);
                        pass.Rules.Add(rule);
                    }
                    else
                    {
                        ApplyToRule(oldRule, ruleModel);
                        pass.Rules.Add(oldRule);
                    }
                }

                #region pass.Tolerances

                List<Tolerance> oldTolerances = new List<Tolerance>();
                pass.Tolerances.ForEach(item => oldTolerances.Add(item));
                pass.Tolerances.Clear();
                foreach (ToleranceModel toleranceModel in command.Tolerances)
                {
                    Tolerance oldTolerance = oldTolerances.Find(item => item.RuleId == toleranceModel.RuleId);
                    if (oldTolerance == null)        // New rule
                    {
                        Tolerance tolerance = MapToTolerance(toleranceModel);
                        pass.Tolerances.Add(tolerance);
                    }
                    else
                    {
                        ApplyToTolerance(oldTolerance, toleranceModel);
                        pass.Tolerances.Add(oldTolerance);
                    }
                }

                #endregion pass.Tolerances

                #region pass.RatingPoints

                pass.RatingPoints = command.RatingPoints.Select(x => mapper.Map<RatingPoint>(x)).ToList();

                #endregion pass.RatingPoints

                #region pass.Weightings

                List<Weighting> oldWeightings = new List<Weighting>();
                pass.Weightings.ForEach(item => oldWeightings.Add(item));
                pass.Weightings.Clear();
                foreach (WeightingModel weightingMode in command.Weightings)
                {
                    Weighting oldWeighting = oldWeightings.Find(item => item.RuleId == weightingMode.RuleId);
                    if (oldWeighting == null)        // New rule
                    {
                        Weighting weighting = MapToWeighting(weightingMode);
                        pass.Weightings.Add(weighting);
                    }
                    else
                    {
                        ApplyToWeighting(oldWeighting, weightingMode);
                        pass.Weightings.Add(oldWeighting);
                    }
                }

                #endregion pass.Weightings

                #region pass.ProgrammeRepetitions

                pass.ProgrammeRepetitions.Clear();
                foreach (ProgrammeRepetitionModel programmeRepetitionMode in command.ProgrammeRepetitions)
                {
                    ProgrammeRepetition programmeRepetition = MapToProgrammeRepetition(programmeRepetitionMode);
                    pass.ProgrammeRepetitions.Add(programmeRepetition);
                }

                #endregion pass.ProgrammeRepetitions

                #region pass.BreakExclusions

                pass.BreakExclusions.Clear();
                foreach (BreakExclusionModel breakExclusionMode in command.BreakExclusions)
                {
                    BreakExclusion breakExclusion = MapToBreakExclusion(breakExclusionMode);
                    pass.BreakExclusions.Add(breakExclusion);
                }

                #endregion pass.BreakExclusions

                #region pass.SlottingLimits

                pass.SlottingLimits.Clear();
                foreach (SlottingLimitModel slottingLimitMode in command.SlottingLimits)
                {
                    SlottingLimit slottingLimit = MapToSlottingLimit(slottingLimitMode);
                    pass.SlottingLimits.Add(slottingLimit);
                }

                #endregion pass.SlottingLimits

                #region pass.PassSalesAreaPriorities

                pass.PassSalesAreaPriorities = MapToPassSalesAreaPriorities(command.PassSalesAreaPriorities);

                #endregion pass.PassSalesAreaPriorities
            }
        }

        public static void ApplyToGeneral(General rule, GeneralModel command)
        {
            rule.RuleId = command.RuleId > 0 ? command.RuleId : rule.RuleId;
            rule.Description = command.Description;
            rule.InternalType = command.InternalType;
            rule.Type = command.Type;
            rule.Value = command.Value;
        }

        public static void ApplyToRule(PassRule rule, PassRuleModel command)
        {
            rule.RuleId = command.RuleId > 0 ? command.RuleId : rule.RuleId;
            rule.Description = command.Description;
            rule.InternalType = command.InternalType;
            rule.Ignore = command.Ignore;
            rule.Type = command.Type;
            rule.Value = command.Value;
            rule.PeakValue = command.PeakValue;
            rule.CampaignType = command.CampaignType;
        }

        public static void ApplyToWeighting(Weighting rule, WeightingModel command)
        {
            rule.RuleId = command.RuleId > 0 ? command.RuleId : rule.RuleId;
            rule.Description = command.Description;
            rule.InternalType = command.InternalType;
            //rule.ForceUnderOver = command.ForceUnderOver;
            //rule.Ignore = command.Ignore;
            rule.InternalType = command.InternalType;
            //rule.Over = command.Over;
            rule.Type = command.Type;
            //rule.Under = command.Under;
            rule.Value = command.Value;
        }

        public static void ApplyToTolerance(Tolerance rule, ToleranceModel command)
        {
            rule.RuleId = command.RuleId > 0 ? command.RuleId : rule.RuleId;
            rule.Description = command.Description;
            rule.InternalType = command.InternalType;
            rule.ForceOverUnder = command.ForceUnderOver;
            rule.Ignore = command.Ignore;
            rule.BookTargetArea = command.BookTargetArea;
            rule.InternalType = command.InternalType;
            rule.Over = command.Over;
            rule.Type = command.Type;
            rule.Under = command.Under;
            rule.Value = command.Value;
            rule.CampaignType = command.CampaignType;
        }

        private static PassReference MapToPassReference(PassModel command)
        {
            return new PassReference()
            {
                Id = command.Id
            };
        }

        private static General MapToGeneral(GeneralModel command)
        {
            return new General()
            {
                Description = command.Description,
                InternalType = command.InternalType,
                RuleId = command.RuleId,
                Type = command.Type,
                Value = command.Value
            };
        }

        private static PassRule MapToRule(PassRuleModel command)
        {
            return new PassRule()
            {
                Description = command.Description,
                InternalType = command.InternalType,
                RuleId = command.RuleId,
                Type = command.Type,
                Ignore = command.Ignore,
                Value = command.Value,
                PeakValue = command.PeakValue,
                CampaignType = command.CampaignType
            };
        }

        private static Tolerance MapToTolerance(ToleranceModel command)
        {
            var tolerance = new Tolerance()
            {
                Description = command.Description,
                InternalType = command.InternalType,
                RuleId = command.RuleId,
                ForceOverUnder = command.ForceUnderOver,
                Ignore = command.Ignore,
                BookTargetArea = command.BookTargetArea,
                Over = command.Over,
                Type = command.Type,
                Under = command.Under,
                Value = command.Value,
                CampaignType = command.CampaignType
            };
            return tolerance;
        }

        private static Weighting MapToWeighting(WeightingModel command)
        {
            var weighting = new Weighting()
            {
                Description = command.Description,
                InternalType = command.InternalType,
                RuleId = command.RuleId,
                //ForceUnderOver = command.ForceUnderOver,
                //Ignore = command.Ignore,
                //Over = command.Over,
                Type = command.Type,
                //Under = command.Under,
                Value = command.Value
            };
            return weighting;
        }

        private static ProgrammeRepetition MapToProgrammeRepetition(ProgrammeRepetitionModel command)
        {
            var programmeRepetition = new ProgrammeRepetition()
            {
                Factor = command.Factor,
                PeakFactor = command.PeakFactor,
                Minutes = command.Minutes
            };
            return programmeRepetition;
        }

        private static SlottingLimit MapToSlottingLimit(SlottingLimitModel command)
        {
            var slottingLimit = new SlottingLimit()
            {
                MinimumEfficiency = command.MinimumEfficiency,
                BandingTolerance = command.BandingTolerance,
                Demographs = command.Demographs,
                MaximumEfficiency = command.MaximumEfficiency
            };
            return slottingLimit;
        }

        private static BreakExclusion MapToBreakExclusion(BreakExclusionModel command)
        {
            var breakExclusion = new BreakExclusion()
            {
                SalesArea = command.SalesArea,
                EndDate = command.EndDate,
                EndTime = command.EndTime,
                SelectableDays = command.SelectableDays,
                StartDate = command.StartDate,
                StartTime = command.StartTime
            };
            return breakExclusion;
        }

        private static PassSalesAreaPriority MapToPassSalesAreaPriorities(PassSalesAreaPriorityModel command)
        {
            if (command == null)
            {
                return null;
            }

            return new PassSalesAreaPriority
            {
                DaysOfWeek = command.DaysOfWeek,
                AreDatesRetained = command.AreDatesRetained,
                StartDate = command.StartDate?.Date,
                EndDate = command.EndDate?.Date,
                AreTimesRetained = command.AreTimesRetained,
                StartTime = command.StartTime,
                EndTime = command.EndTime,
                SalesAreaPriorities = command.SalesAreaPriorities.Select(x => new SalesAreaPriority
                {
                    SalesArea = x.SalesArea,
                    Priority = x.Priority
                }).ToList(),
                IsOffPeakTime = command.IsOffPeakTime,
                IsPeakTime = command.IsPeakTime,
                IsMidnightTime = command.IsMidnightTime
            };
        }

        /// <summary>
        /// Returns RunModel for run
        /// </summary>
        /// <param name="run"></param>
        /// <param name="scenarioRepository"></param>
        /// <param name="passRepository"></param>
        /// <param name="tenantSettingsRepository"></param>
        /// <param name="allCampaigns"></param>
        /// <param name="functionalAreaRepository"></param>
        /// <returns></returns>
        public static RunModel MapToRunModel(Run run, IScenarioRepository scenarioRepository, IPassRepository passRepository,
                                             ITenantSettingsRepository tenantSettingsRepository, IMapper mapper,
                                             IAnalysisGroupRepository analysisGroupRepository,
                                             IFunctionalAreaRepository functionalAreaRepository = null,
                                             IScenarioCampaignMetricRepository scenarioCampaignMetricRepository = null,
                                             List<CampaignWithProductFlatModel> allCampaigns = null,
                                             List<Pass> passesFromDb = null, List<Scenario> scenariosFromDb = null)
        {
            // Get scenarios for run
            var scenarios = scenariosFromDb ?? scenarioRepository.FindByIds(run.Scenarios.Select(s => s.Id)).ToList();

            // Get passes for run
            var passes = passesFromDb ?? passRepository.FindByIds(scenarios.SelectMany(s => s.Passes).Select(p => p.Id).Distinct()).ToList();

            var analysisGroups = run.AnalysisGroupTargets.Any()
                ? analysisGroupRepository.GetByIds(run.AnalysisGroupTargets.Select(x => x.AnalysisGroupId).ToArray()).ToDictionary(x => x.Id)
                : new Dictionary<int, AnalysisGroupNameModel>();

            // Get default ScenarioId
            var defaultScenarioId = tenantSettingsRepository.GetDefaultScenarioId();

            if (run.RunStatus == RunStatus.NotStarted && allCampaigns != null && allCampaigns.Any())
            {
                // Since the run has not already started amend Run Scenarios
                // with CampaignPassPriorities for new Campaigns and remove
                // CampaignPassPriorities of deleted Campaigns
                CampaignPassPrioritiesServiceMapper.AmendCampaignPassPriorities(
                    scenarios,
                    passes,
                    allCampaigns,
                    passRepository,
                    mapper);
            }

            var runModel = mapper.Map<RunModel>(Tuple.Create(run, scenarios, passes, analysisGroups, defaultScenarioId));

            if (run.RunStatus == RunStatus.Complete)
            {
                ApplyFunctionalAreaFaultTypesToRunModel(runModel, run, functionalAreaRepository);
            }

            if (scenarioCampaignMetricRepository != null)
            {
                runModel.Scenarios.ForEach(s => ApplyKPIsToScenarioCampaigns(s, scenarioCampaignMetricRepository));
            }

            return runModel;
        }

        /// <summary>
        /// Returns RunModel with BasicData for run
        /// </summary>
        /// <param name="run"></param>
        /// <param name="scenarioRepository"></param>
        /// <returns></returns>
        public static RunModel MapToRunModel(Run run, IScenarioRepository scenarioRepository, IMapper mapper)
        {
            // Get scenarios for run
            var scenarios = scenarioRepository.FindByIds(run.Scenarios.Select(s => s.Id)).ToList();

            var runModel = mapper.Map<RunModel>(Tuple.Create(run, scenarios));

            return runModel;
        }

        private static void ApplyKPIsToScenarioCampaigns(ScenarioModel scenario, IScenarioCampaignMetricRepository scenarioCampaignMetricRepository)
        {
            var scenarioCampaignMetrics = scenarioCampaignMetricRepository.Get(scenario.Id);

            if (scenarioCampaignMetrics == null)
            {
                return;
            }

            scenario.CampaignPassPriorities.ForEach(c =>
            {
                var campaignMetrics = scenarioCampaignMetrics.Metrics.FirstOrDefault(m => m.CampaignExternalId == c.CampaignExternalId);

                if (campaignMetrics != null)
                {
                    ApplyKPIsToScenarioCampaign(c.Campaign, campaignMetrics);
                }
            });
        }

        private static void ApplyKPIsToScenarioCampaign(CampaignWithProductFlatModel campaign, ScenarioCampaignMetricItem scenarioCampaignMetrics)
        {
            campaign.TotalSpots = scenarioCampaignMetrics.TotalSpots;
            campaign.ZeroRatedSpots = scenarioCampaignMetrics.ZeroRatedSpots;
            campaign.NominalValue = scenarioCampaignMetrics.NominalValue;
            campaign.TotalNominalValue = scenarioCampaignMetrics.TotalNominalValue;
            campaign.DifferenceValueDelivered = scenarioCampaignMetrics.DifferenceValueDelivered;
            campaign.DifferenceValueDeliveredPercentage = scenarioCampaignMetrics.DifferenceValueDeliveredPercentage;
        }

        private static void ApplyFunctionalAreaFaultTypesToRunModel(RunModel runModel, Run run, IFunctionalAreaRepository functionalAreaRepository)
        {
            if (run.FailureTypes == null || !run.FailureTypes.Any())
            {
                return;
            }

            var selectedFaultTypes = functionalAreaRepository.FindFaultTypes(run.FailureTypes);
            if (selectedFaultTypes != null && selectedFaultTypes.Any())
            {
                runModel.FaultTypes = selectedFaultTypes.Select(sft => new RunFaultTypeModel
                {
                    Id = sft.Id,
                    Description = sft.Description?.FirstOrDefault(e => e.Key == "ENG").Value
                });
            }
        }

        /// <summary>
        /// Returns RunModels for runs
        /// </summary>
        /// <param name="runs"></param>
        /// <param name="scenarioRepository"></param>
        /// <param name="passRepository"></param>
        /// <param name="analysisGroupRepository"></param>
        /// <param name="tenantSettingsRepository"></param>
        /// <returns></returns>
        public static List<RunModel> MapToRunModels(IEnumerable<Run> runs, IScenarioRepository scenarioRepository,
            IPassRepository passRepository, IAnalysisGroupRepository analysisGroupRepository,
            ITenantSettingsRepository tenantSettingsRepository, IMapper mapper)
        {
            // Get scenarios for all runs
            var scenarios = scenarioRepository.FindByIds(runs.SelectMany(r => r.Scenarios).Select(s => s.Id).Distinct()).ToList();

            // Get passes for all runs
            var passes = passRepository.FindByIds(scenarios.SelectMany(s => s.Passes).Select(p => p.Id).Distinct()).ToList();

            var analysisGroupIds = runs.SelectMany(x => x.AnalysisGroupTargets.Select(t => t.AnalysisGroupId)).ToArray();
            var analysisGroups = analysisGroupIds.Any()
                ? analysisGroupRepository.GetByIds(analysisGroupIds).ToDictionary(x => x.Id)
                : new Dictionary<int, AnalysisGroupNameModel>();

            // Get default ScenarioId
            var defaultScenarioId = tenantSettingsRepository.Get().DefaultScenarioId;

            List<RunModel> runModels = new List<RunModel>();
            foreach (var run in runs)
            {
                var runModel = mapper.Map<RunModel>(Tuple.Create(run, scenarios, passes, analysisGroups, defaultScenarioId));
                runModels.Add(runModel);
            }
            return runModels;
        }

        /// <summary>
        /// Returns ScenarioModel from Scenario
        /// </summary>
        /// <param name="scenario"></param>
        /// <param name="passes"></param>
        /// <param name="libraryScenarioIds"></param>
        /// <param name="defaultScenarioId"></param>
        /// <returns></returns>
        public static ScenarioModel MapToScenarioModel(Scenario scenario, List<Pass> passes,
            Guid defaultScenarioId, IMapper mapper)
        {
            var scenarioModel = mapper.Map<ScenarioModel>(Tuple.Create(scenario, passes, defaultScenarioId));
            return scenarioModel;
        }

        /// <summary>
        /// Returns ScenarioModel from Scenario
        /// </summary>
        /// <param name="scenario"></param>
        /// <param name="scenarioRepository"></param>
        /// <param name="passRepository"></param>
        /// <param name="tenantSettingsRepository"></param>
        /// <returns></returns>
        public static ScenarioModel MapToScenarioModel(Scenario scenario, IScenarioRepository scenarioRepository, IPassRepository passRepository,
            ITenantSettingsRepository tenantSettingsRepository, IMapper mapper)
        {
            // Get passes
            List<Pass> passes = passRepository.FindByIds(scenario.Passes.Select(p => p.Id)).ToList();

            // Get default ScenarioId
            var defaultScenarioId = tenantSettingsRepository.Get().DefaultScenarioId;

            var scenarioModel = mapper.Map<ScenarioModel>(Tuple.Create(scenario, passes, defaultScenarioId));
            return scenarioModel;
        }

        /// <summary>
        /// Returns ScenarioModel list from Scenario list
        /// </summary>
        /// <param name="scenarios"></param>
        /// <param name="scenarioRepository"></param>
        /// <param name="passRepository"></param>
        /// <param name="tenantSettingsRepository"></param>
        /// <returns></returns>
        public static List<ScenarioModel> MapToScenarioModels(
            List<Scenario> scenarios,
            IScenarioRepository scenarioRepository,
            IPassRepository passRepository,
            ITenantSettingsRepository tenantSettingsRepository,
            IMapper mapper)
        {
            var scenarioModels = new List<ScenarioModel>();
            var passIdsInScenarios = scenarios
                        .SelectMany(s => s.Passes)
                        .Select(p => p.Id);

            var passes = passRepository.GetAll().Where(p => passIdsInScenarios.Contains(p.Id)).ToList();

            var defaultScenarioId = tenantSettingsRepository.Get().DefaultScenarioId;

            scenarios.ForEach(scenario => scenarioModels.Add(mapper.Map<ScenarioModel>(
                    Tuple.Create(scenario, passes, defaultScenarioId))));

            return scenarioModels;
        }

        /// <summary>
        /// Returns PassModel from Pass
        /// </summary>
        /// <param name="pass"></param>
        /// <param name="passRepository"></param>
        /// <returns></returns>
        public static PassModel MapToPassModel(Pass pass, IPassRepository passRepository, IMapper mapper)
        {
            var passModel = mapper.Map<PassModel>(pass);

            return passModel;
        }

        /// <summary>
        /// Returns PassModel list from Pass list
        /// </summary>
        /// <param name="passes"></param>
        /// <param name="passRepository"></param>
        /// <returns></returns>
        public static List<PassModel> MapToPassModels(
            IEnumerable<Pass> passes,
            IPassRepository passRepository,
            IMapper mapper)
        {
            var passModels = new List<PassModel>();

            foreach (var pass in passes)
            {
                passModels.Add(mapper.Map<PassModel>(pass));
            }

            return passModels;
        }

        public static IncludeRightSizer MapToIncludeRightSizer(CampaignRunProcessesSettings usingSetting)
        {
            var defaultResult = IncludeRightSizer.No;
            if (!usingSetting.IncludeRightSizer.HasValue || !usingSetting.IncludeRightSizer.Value || !usingSetting.RightSizerLevel.HasValue)
            {
                return defaultResult;
            }

            switch (usingSetting.RightSizerLevel.Value)
            {
                case RightSizerLevel.CampaignLevel:
                    return IncludeRightSizer.CampaignLevel;

                case RightSizerLevel.DetailLevel:
                    return IncludeRightSizer.DetailLevel;

                default:
                    return defaultResult;
            }
        }

        internal static List<RunSearchResultModel> MapToRunSearchResultModel(List<Run> runs, IMapper mapper)
        {
            List<RunSearchResultModel> result = new List<RunSearchResultModel>();
            foreach (var item in runs)
            {
                result.Add(new RunSearchResultModel
                {
                    Id = item.Id,
                    Description = item.Description,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    ExecuteStartedDateTime = item.ExecuteStartedDateTime,
                    Author = item.Author,
                    Status = item.RunStatus,
                    FirstScenarioStartedDateTime = item.FirstScenarioStartedDateTime,
                    LastScenarioCompletedDateTime = item.LastScenarioCompletedDateTime,
                    LastModifiedDateTime = item.LastModifiedDateTime,
                    InventoryLock = mapper.Map<InventoryLockModel>(item.InventoryLock),
                    HasNonPendingScenario = item.HasNonPendingScenario,
                    HasAllScenarioCompletedSuccessfully = item.HasAllScenarioCompletedSuccessfully,
                    ExternalStatus = item.ExternalStatus
                });
            }
            return result;
        }

        public static SponsorshipModel MapToSponsorshipModel(Sponsorship sponsorship, IMapper mapper) => mapper.Map<SponsorshipModel>(sponsorship);

        public static List<SponsorshipModel> MapToSponsorshipModels(IEnumerable<Sponsorship> sponsorships, IMapper mapper)
        {
            var result = new List<SponsorshipModel>();
            sponsorships.ForEach(item => result.Add(MapToSponsorshipModel(item, mapper)));
            return result;
        }

        /// <summary>
        /// Builds instance of <see cref="RuleTypeModel"/> model using <see cref="RuleType"/>
        /// </summary>
        /// <param name="ruleType"></param>
        /// <param name="ruleRepository"></param>
        /// <returns></returns>
        public static RuleTypeModel MapToRuleTypeModel(RuleType ruleType, IRuleRepository ruleRepository, IMapper mapper)
        {
            var rules = ruleRepository.FindByRuleTypeId(ruleType.Id).ToList();
            return mapper.Map<RuleTypeModel>(Tuple.Create(ruleType, rules));
        }

        /// <summary>
        /// Builds list of <see cref="RuleTypeModel"/> models using base <see cref="RuleType"/> instances
        /// </summary>
        /// <param name="ruleTypes"></param>
        /// <param name="ruleRepository"></param>
        /// <returns></returns>
        public static List<RuleTypeModel> MapToRuleTypeModel(IReadOnlyCollection<RuleType> ruleTypes, IRuleRepository ruleRepository, IMapper mapper)
        {
            var ruleTypeModels = new List<RuleTypeModel>();

            var groupedRules = ruleRepository
                .FindByRuleTypeIds(ruleTypes.Select(rt => rt.Id))
                .GroupBy(r => r.RuleTypeId)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var ruleType in ruleTypes)
            {
                var rules = groupedRules.ContainsKey(ruleType.Id) ? groupedRules[ruleType.Id] : new List<Rule>();
                ruleTypeModels.Add(mapper.Map<RuleTypeModel>(Tuple.Create(ruleType, rules)));
            }

            return ruleTypeModels;
        }

        /// <summary>
        /// Builds instance of <see cref="AutopilotSettingsModel"/> model using <see cref="AutopilotSettings"/>
        /// </summary>
        /// <param name="autopilotSettings"></param>
        /// <param name="autopilotRuleRepository"></param>
        /// <param name="ruleRepository"></param>
        /// <param name="ruleTypeRepository"></param>
        /// <returns></returns>
        public static AutopilotSettingsModel MapToAutopilotSettingsModel(AutopilotSettings autopilotSettings,
            IAutopilotRuleRepository autopilotRuleRepository, IRuleRepository ruleRepository,
            IRuleTypeRepository ruleTypeRepository,
            IMapper mapper)
        {
            // get only autopilot enabled rule types with rules
            var ruleTypeModels = MapToRuleTypeModel(ruleTypeRepository.GetAll(true).ToList(), ruleRepository, mapper);
            var rules = ruleTypeModels.SelectMany(r => r.Rules).ToList();

            // one autopilot rule per internal rule
            var autopilotRulesGrouped = autopilotRuleRepository.GetAll()
                .GroupBy(r => r.UniqueRuleKey)
                .Select(r => r.First())
                .ToList();

            return mapper.Map<AutopilotSettingsModel>(Tuple.Create(autopilotSettings, autopilotRulesGrouped, rules, ruleTypeModels));
        }

        public static List<RunNotification> MapToRunNotification(IEnumerable<Run> runList)
        {
            var runNotificationList = new List<RunNotification>();
            runList.ForEach(run => runNotificationList.Add(MapToRunNotification(run)));
            return runNotificationList;
        }

        public static RunNotification MapToRunNotification(Run run)
        {
            var runNotification = new RunNotification()
            {
                id = run.Id,
                description = run.Description,
                endDate = (DateTime)run.LastScenarioCompletedDateTime,
                endTime = ((DateTime)run.LastScenarioCompletedDateTime).TimeOfDay,
                status = run.RunStatus
            };
            return runNotification;
        }

        public static List<RunTypeModel> MapToRunTypes(IEnumerable<RunType> runTypes, IMapper mapper)
        {
            var runTypeList = new List<RunTypeModel>();
            runTypes.ForEach(runType => runTypeList.Add(mapper.Map<RunTypeModel>(runType)));
            return runTypeList;
        }

        public static PipelineAuditEventModel MapToPipelineAuditEventModel(PipelineAuditEvent pipelineAuditEvent, IMapper mapper)
        {
            return mapper.Map<PipelineAuditEventModel>(pipelineAuditEvent);
        }

        public static PipelineAuditEvent MapToPipelineAuditEvent(PipelineAuditEventModel pipelineAuditEventModel, IMapper mapper)
        {
            return mapper.Map<PipelineAuditEvent>(pipelineAuditEventModel);
        }

        public static AutoBookTaskReportModel MapToAutoBookTaskReportModel(AutoBookTaskReport autoBookTaskReport, IMapper mapper)
        {
            return mapper.Map<AutoBookTaskReportModel>(autoBookTaskReport);
        }

        public static AutoBookTaskReport MapToAutoBookTaskReport(AutoBookTaskReportModel autoBookTaskReportModel, IMapper mapper)
        {
            return mapper.Map<AutoBookTaskReport>(autoBookTaskReportModel);
        }

        public static AnalysisGroupModel MapToAnalysisGroupModel(AnalysisGroup analysisGroup,
            ICampaignRepository campaignRepository,
            IClashRepository clashRepository,
            IProductRepository productRepository,
            IUsersRepository usersRepository,
            IMapper mapper)
        {
            var filterWithLabels = MapToAnalysisGroupFilterSearchModel(analysisGroup.Filter, campaignRepository, clashRepository, productRepository);
            var users = mapper.Map<List<UserReducedModel>>(usersRepository.GetByIds(new List<int> { analysisGroup.CreatedBy, analysisGroup.ModifiedBy }));

            return mapper.Map<AnalysisGroupModel>((analysisGroup, filterWithLabels, users.ToDictionary(x => x.Id)));
        }

        public static List<AnalysisGroupModel> MapToAnalysisGroupModels(List<AnalysisGroup> analysisGroups,
            ICampaignRepository campaignRepository,
            IClashRepository clashRepository,
            IProductRepository productRepository,
            IUsersRepository usersRepository,
            IMapper mapper)
        {
            var filterWithLabels = MapToAnalysisGroupFilterSearchModel(AnalysisGroupFilter.BuildFrom(analysisGroups), campaignRepository, clashRepository, productRepository);
            var userIds = new HashSet<int>();

            foreach (var analysisGroup in analysisGroups)
            {
                userIds.Add(analysisGroup.CreatedBy);
                userIds.Add(analysisGroup.ModifiedBy);
            }

            var users = mapper.Map<List<UserReducedModel>>(usersRepository.GetByIds(userIds.ToList()));
            return mapper.Map<List<AnalysisGroupModel>>((analysisGroups, filterWithLabels, users.ToDictionary(x => x.Id)));
        }

        public static AnalysisGroupFilterSearchModel MapToAnalysisGroupFilterSearchModel(AnalysisGroupFilter filter,
            ICampaignRepository campaignRepository,
            IClashRepository clashRepository,
            IProductRepository productRepository)
        {
            if (filter is null)
            {
                throw new InvalidOperationException("AnalysisGroup filter can not be null");
            }

            return new AnalysisGroupFilterSearchModel
            {
                Advertisers = productRepository.GetAdvertisers(filter.AdvertiserExternalIds)
                    .ToDictionary(x => x.AdvertiserIdentifier, x => x.AdvertiserName),
                Agencies = productRepository.GetAgencies(filter.AgencyExternalIds)
                    .ToDictionary(x => x.AgencyIdentifier, x => x.AgencyName),
                AgencyGroups = productRepository.GetAgencyGroups(filter.AgencyGroupCodes)
                    .ToDictionary(x => x.Code, x => x.ShortName),
                BusinessTypes = filter.BusinessTypes.Any()
                    ? new HashSet<string>(campaignRepository.GetBusinessTypes())
                    : filter.BusinessTypes,
                Campaigns = campaignRepository.FindNameByRefs(filter.CampaignExternalIds)
                    .ToDictionary(x => x.ExternalId, x => x.Name),
                ClashCodes = clashRepository.GetDescriptionByExternalRefs(filter.ClashExternalRefs)
                    .ToDictionary(x => x.Externalref, x => x.Description),
                Products = productRepository.GetByExternalIds(filter.ProductExternalIds)
                    .ToDictionary(x => x.Externalidentifier, x => x.Name),
                ReportingCategories = filter.ReportingCategories.Any()
                    ? new HashSet<string>(productRepository.GetReportingCategories())
                    : filter.ReportingCategories,
                SalesExecs = productRepository.GetSalesExecutives(filter.SalesExecExternalIds)
                    .ToDictionary(x => x.Identifier, x => x.Name)
            };
        }
    }
}

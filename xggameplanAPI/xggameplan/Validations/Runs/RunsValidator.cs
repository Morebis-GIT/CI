using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups;
using ImagineCommunications.GamePlan.Domain.BRS;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.DeliveryCappingGroup;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.RunTypes;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.Extensions;
using xggameplan.Model;
using xggameplan.Validations.Runs.Interfaces;

namespace xggameplan.Validations.Runs
{
    public class RunsValidator : IRunsValidator
    {
        private readonly IBRSConfigurationTemplateRepository _brsConfigurationTemplateRepository;
        private readonly ITenantSettingsRepository _tenantSettingsRepository;
        private readonly IRunRepository _runRepository;
        private readonly IScenarioRepository _scenarioRepository;
        private readonly ISalesAreaRepository _salesAreaRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IDemographicRepository _demographicRepository;
        private readonly IDeliveryCappingGroupRepository _deliveryCappingGroupRepository;
        private readonly IFeatureManager _featureManager;
        private readonly IRunTypeRepository _runTypeRepository;
        private readonly IAnalysisGroupRepository _analysisGroupRepository;
        private readonly IMapper _mapper;

        public RunsValidator(
            IBRSConfigurationTemplateRepository brsConfigurationTemplateRepository,
            ITenantSettingsRepository tenantSettingsRepository,
            IRunRepository runRepository,
            IScenarioRepository scenarioRepository,
            ISalesAreaRepository salesAreaRepository,
            ICampaignRepository campaignRepository,
            IDemographicRepository demographicRepository,
            IDeliveryCappingGroupRepository deliveryCappingGroupRepository,
            IFeatureManager featureManager,
            IRunTypeRepository runTypeRepository,
            IAnalysisGroupRepository analysisGroupRepository,
            IMapper mapper
            )
        {
            _brsConfigurationTemplateRepository = brsConfigurationTemplateRepository;
            _tenantSettingsRepository = tenantSettingsRepository;
            _runRepository = runRepository;
            _scenarioRepository = scenarioRepository;
            _salesAreaRepository = salesAreaRepository;
            _campaignRepository = campaignRepository;
            _demographicRepository = demographicRepository;
            _deliveryCappingGroupRepository = deliveryCappingGroupRepository;
            _featureManager = featureManager;
            _runTypeRepository = runTypeRepository;
            _analysisGroupRepository = analysisGroupRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Validates for saving run
        /// </summary>
        /// <param name="run"></param>
        /// <param name="allScenarios"></param>
        /// <param name="allPassesByScenario"></param>
        public void ValidateForSave(
            Run run,
            List<Scenario> allScenarios,
            List<List<Pass>> allPassesByScenario,
            List<SalesArea> allSalesAreas)
        {
            // Basic validation
            Run.ValidateForSave(run);

            ValidateAnalysisGroupTargets(run.AnalysisGroupTargets);

            if (run.RunTypeId != 0)
            {
                if (_featureManager.IsEnabled(nameof(ProductFeature.RunType)))
                {
                    var runType = _runTypeRepository.Get(run.RunTypeId);
                    if (runType is null || runType.Hidden)
                    {
                        throw new Exception("Run type not found");
                    }
                }
                else
                {
                    run.RunTypeId = 0;
                }
            }

            if (run.Scenarios.Count > 1 && !_brsConfigurationTemplateRepository.Exists(run.BRSConfigurationTemplateId))
            {
                throw new Exception("BRS template not found");
            }

            RunsValidations.ValidateScenarios(allScenarios);

            // validate all passes
            for (int index = 0; index < allPassesByScenario.Count; index++)
            {
                allPassesByScenario[index]?.ForEach(pass =>
                {
                    Pass.ValidateForSave(pass);
                    Pass.ValidateScenarioNamingUniqueness(pass, new List<Scenario>() { allScenarios[index] });
                });
            }

            // Check that run doesn't contain default scenario, can only contain
            // a copy
            TenantSettings tenantSettings = _tenantSettingsRepository.Get();
            if (tenantSettings.DefaultScenarioId != Guid.Empty && run.Scenarios.Where(s => s.Id == tenantSettings.DefaultScenarioId).Any())
            {
                throw new Exception("Run cannot contain the default scenario. It must contain a copy");
            }

            if (string.IsNullOrWhiteSpace(tenantSettings.PeakStartTime)
                || string.IsNullOrWhiteSpace(tenantSettings.PeakEndTime))
            {
                throw new ArgumentNullException(nameof(tenantSettings), "Peak daypart is not set. Please check the tenant settings.");
            }

            if (string.IsNullOrWhiteSpace(tenantSettings.MidnightStartTime)
                || string.IsNullOrWhiteSpace(tenantSettings.MidnightEndTime))
            {
                throw new ArgumentNullException(nameof(tenantSettings), "Midnight daypart is not set. Please check the tenant settings.");
            }

            // Check sales areas priorities
            if (run.SalesAreaPriorities != null && run.SalesAreaPriorities.Any())
            {
                var existingSalesAreaNames = new HashSet<string>(allSalesAreas.Select(s => s.Name));
                var runSalesAreaNames = new HashSet<string>();

                var unknownSalesAreas = new List<string>();
                var duplicatedSalesAreas = new List<string>();
                bool areAllExcluded = true;

                foreach (var priority in run.SalesAreaPriorities)
                {
                    if (!existingSalesAreaNames.Contains(priority.SalesArea))
                    {
                        unknownSalesAreas.Add(priority.SalesArea);
                    }

                    if (runSalesAreaNames.Contains(priority.SalesArea))
                    {
                        duplicatedSalesAreas.Add(priority.SalesArea);
                    }

                    if (priority.Priority != SalesAreaPriorityType.Exclude)
                    {
                        areAllExcluded = false;
                    }

                    runSalesAreaNames.Add(priority.SalesArea);
                }

                if (unknownSalesAreas.Any())
                {
                    throw new Exception(string.Format("Sales area {0} is not valid", unknownSalesAreas[0]));
                }

                if (areAllExcluded)
                {
                    throw new Exception("All Sales area priorities are set to Exclude");
                }

                if (duplicatedSalesAreas.Any())
                {
                    throw new Exception(string.Format("Run Sales area priorities contains duplicate {0}", duplicatedSalesAreas[0]));
                }
            }

            // Get runs & scenarios
            var runsWithScenarioId = _runRepository.GetRunsWithScenarioId();

            // Get scenarios & passes
            var scenariosWithPassId = _scenarioRepository.GetScenariosWithPassId();

            if (run.Scenarios != null && run.Scenarios.Any())
            {
                var runSalesAreas = run.SalesAreaPriorities?.Where(sa => sa.Priority != SalesAreaPriorityType.Exclude).Select(x => x.SalesArea).ToList();
                var salesAreasToValidate = new List<string>();

                for (int scenarioIndex = 0; scenarioIndex < run.Scenarios.Count; scenarioIndex++)
                {
                    var scenario = run.Scenarios[scenarioIndex];
                    var passes = allPassesByScenario[scenarioIndex];

                    // Check that scenario isn't linked to another run
                    var otherRunIdsForScenarioId = runsWithScenarioId.Where(rws => rws.ScenarioId == scenario.Id && rws.RunId != run.Id).Select(rws => rws.RunId).ToList();
                    if (otherRunIdsForScenarioId.Any())
                    {
                        var otherRun = _runRepository.Find(otherRunIdsForScenarioId.First());
                        throw new Exception(string.Format("Scenario is already linked to Run {0}", otherRun.Description));
                    }

                    if (passes != null && passes.Any())
                    {
                        foreach (var pass in passes)
                        {
                            // Check that passes aren't linked to other scenarios
                            var otherScenarioIdsForPassId = scenariosWithPassId.Where(swp => swp.PassId == pass.Id && swp.ScenarioId != scenario.Id).Select(swp => swp.ScenarioId).ToList();
                            if (otherScenarioIdsForPassId.Any())
                            {
                                var otherScenario = _scenarioRepository.Get(otherScenarioIdsForPassId.First());
                                throw new Exception(string.Format("Pass is already linked to Scenario {0}", otherScenario.Name));
                            }

                            // Check that passes aren't linked to other
                            // scenarios in this run
                            if (pass.Id > 0)
                            {
                                if (passes.Where(p => p.Id == pass.Id).ToList().Count > 1)
                                {
                                    throw new Exception("Scenario cannot contain multiple instances of the same PassID");
                                }

                                for (int scenarioIndex2 = 0; scenarioIndex2 < run.Scenarios.Count; scenarioIndex2++)
                                {
                                    if (scenarioIndex != scenarioIndex2)
                                    {
                                        if (allPassesByScenario[scenarioIndex2].Select(p => p.Id).Contains(pass.Id))
                                        {
                                            throw new Exception("Pass is already linked to another Scenario for this run");
                                        }
                                    }
                                }
                            }

                            var errorMsg = RunsValidations.ValidatePassSalesAreaPriorities(run, pass, tenantSettings, runSalesAreas);
                            if (!string.IsNullOrWhiteSpace(errorMsg))
                            {
                                throw new Exception(errorMsg);
                            }
                        }
                    }

                    // Check break exclusions
                    if (passes != null && passes.Any() && passes.Any(p => p.BreakExclusions != null && p.BreakExclusions.Any()))
                    {
                        var breakExclusions = passes
                            .Where(p => p?.BreakExclusions != null && p.BreakExclusions.Any())
                            .SelectMany(p => p.BreakExclusions)
                            .Select(b => b.SalesArea);

                        salesAreasToValidate.AddRange(breakExclusions);
                    }
                }

                if (salesAreasToValidate.Any())
                {
                    _salesAreaRepository.ValidateSaleArea(salesAreasToValidate);
                }
            }

            // Check campaigns
            if (run.Campaigns != null && run.Campaigns.Count > 0)
            {
                var existingCampaigns = new HashSet<string>(_campaignRepository.GetAllFlat().Select(x => x.ExternalId));
                var unknownCampaigns = run.Campaigns.Select(ca => ca.ExternalId)
                    .Where(externalId => !existingCampaigns.Contains(externalId)).ToList();
                if (unknownCampaigns.Any())
                {
                    throw new Exception(string.Format("Campaign {0} is not valid", unknownCampaigns[0]));
                }
            }

            // Validation for slotting control by Demograph
            if (run.Scenarios != null && run.Scenarios.Any())
            {
                var demographicsToValidate = new List<string>();

                for (int scenarioIndex = 0; scenarioIndex < run.Scenarios.Count; scenarioIndex++)
                {
                    var passes = allPassesByScenario[scenarioIndex];

                    if (passes != null && passes.Any() && passes.Any(p => p.SlottingLimits != null && p.SlottingLimits.Any()))
                    {
                        var slottingLimits = passes
                            .Where(p => p?.SlottingLimits != null && p.SlottingLimits.Any())
                            .SelectMany(p => p.SlottingLimits)
                            .Select(b => b.Demographs);

                        demographicsToValidate.AddRange(slottingLimits);
                    }
                }

                demographicsToValidate = demographicsToValidate.Where(x => x != null).Distinct().ToList();

                if (demographicsToValidate.Any() && !_demographicRepository.ValidateDemographics(demographicsToValidate,
                    out List<string> invalidDemographics))
                {
                    var msg = string.Concat(
                        "Invalid Demographic in slotting control by Demograph: ",
                        invalidDemographics != null
                            ? string.Join(",", invalidDemographics)
                            : string.Empty);
                    throw new InvalidDataException(msg);
                }
            }

            ValidateDeliveryCappingGroupIds(_mapper.Map<IEnumerable<CampaignRunProcessesSettingsModel>>(run.CampaignsProcessesSettings));
        }

        /// <summary>
        /// Validates <see cref="Run.AnalysisGroupTargets"/> before save
        /// </summary>
        /// <param name="targets"></param>
        /// <exception cref="System.IO.InvalidDataException">Thrown when invalid entities found</exception>
        private void ValidateAnalysisGroupTargets(IReadOnlyCollection<AnalysisGroupTarget> targets)
        {
            if (!targets.Any())
            {
                return;
            }

            var analysisGroupIds = new HashSet<int>();
            foreach (var target in targets)
            {
                if (string.IsNullOrWhiteSpace(target.KPI))
                {
                    throw new Exception("Analysis Group Target KPI should not be empty");
                }

                analysisGroupIds.Add(target.AnalysisGroupId);
            }

            if (!_analysisGroupRepository.IsValid(analysisGroupIds, out var invalidGroups))
            {
                var msg = string.Concat("Invalid Analysis Group ids: ", invalidGroups != null
                    ? string.Join(",", invalidGroups)
                    : string.Empty);

                throw new InvalidDataException(msg);
            }
        }

        /// <summary>
        /// Validate CampaignRunProcessesSettingsModel list
        /// DeliveryCappingGroupIds vs existing DeliveryCappingGroupIds
        /// </summary>
        /// <param name="campaignRunProcessesSettings"></param>
        public void ValidateDeliveryCappingGroupIds(IEnumerable<CampaignRunProcessesSettingsModel> campaignRunProcessesSettings)
        {
            var deliveryCappingGroupsIds = campaignRunProcessesSettings
                .Where(x => x.DeliveryCappingGroupId != 0)
                .Select(x => x.DeliveryCappingGroupId)
                .Distinct();

            if (!deliveryCappingGroupsIds.Any())
            {
                return;
            }

            var existingsDeliveryCappingGroupIds = _deliveryCappingGroupRepository.Get(deliveryCappingGroupsIds).Select(x => x.Id);

            var invalidDeliveryCappingGroupIds = deliveryCappingGroupsIds.Except(existingsDeliveryCappingGroupIds);
            if (invalidDeliveryCappingGroupIds.Any())
            {
                throw new ArgumentException($"Invalid delivery capping groups ids: {string.Join(", ", invalidDeliveryCappingGroupIds)}");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BRS;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using Newtonsoft.Json;
using xggameplan.AuditEvents;

namespace xggameplan.core.BRS
{
    public class BRSIndicatorManager : IBRSIndicatorManager
    {
        private readonly IBRSConfigurationTemplateRepository _brsConfigurationTemplateRepository;
        private readonly IKPIPriorityRepository _kpiPriorityRepository;
        private readonly IScenarioResultRepository _scenarioResultRepository;
        private readonly IBRSCalculator _brsCalculator;
        private readonly IAuditEventRepository _auditEventRepository;

        public BRSIndicatorManager(
            IBRSConfigurationTemplateRepository brsConfigurationTemplateRepository,
            IKPIPriorityRepository kpiPriorityRepository,
            IScenarioResultRepository scenarioResultRepository,
            IBRSCalculator brsCalculator,
            IAuditEventRepository auditEventRepository)
        {
            _brsConfigurationTemplateRepository = brsConfigurationTemplateRepository;
            _kpiPriorityRepository = kpiPriorityRepository;
            _scenarioResultRepository = scenarioResultRepository;
            _brsCalculator = brsCalculator;
            _auditEventRepository = auditEventRepository;
        }

        public IEnumerable<ScenarioResult> CalculateBRSIndicators(Run run, int? brsConfigurationTemplateId = null)
        {
            if (run.Scenarios.Count == 1)
            {
                throw new Exception("Impossible to calculate BRS Indicators for run with 1 scenario.");
            }

            if (!run.HasAllScenarioCompletedSuccessfully)
            {
                throw new Exception("Impossible to calculate BRS Indicators for not successfully completed run.");
            }

            var id = brsConfigurationTemplateId.HasValue
                ? brsConfigurationTemplateId.Value
                : run.BRSConfigurationTemplateId;

            var brsConfigurationTemplate = _brsConfigurationTemplateRepository.Get(id);
            if (brsConfigurationTemplate == null)
            {
                throw new Exception($"BRS configuration with id {id} not found");
            }

            var infoLabel = $"(RunID={run.Id}, BRSConfigurationTemplateId={run.BRSConfigurationTemplateId})";
            RaiseInfo($"BRS template {infoLabel}: {JsonConvert.SerializeObject(brsConfigurationTemplate)}");

            var scenarioResults = _scenarioResultRepository.Find(run.Scenarios.Select(x => x.Id).ToArray());

            RaiseInfo($"ScenarioResults before calculation {infoLabel}: {JsonConvert.SerializeObject(scenarioResults)}");

            var kpiPriorities = _kpiPriorityRepository.GetAll();

            return _brsCalculator.Calculate(brsConfigurationTemplate.KPIConfigurations, scenarioResults, kpiPriorities);
        }

        public IEnumerable<ScenarioResult> CalculateBRSIndicatorsAfterRunCompleted(Run run)
        {
            if (run.Scenarios.Count > 1 && run.HasAllScenarioCompletedSuccessfully)
            {
                var infoLabel = $"(RunID={run.Id}, BRSConfigurationTemplateId={run.BRSConfigurationTemplateId})";

                RaiseInfo($"Starting calculation of BRS Indicator {infoLabel}. Scenarios info: {run.Scenarios.Select(x => new { ScenarioId = x.Id, x.Status }) }");
                var scenarioResults = CalculateBRSIndicators(run);
                RaiseInfo($"Scenario results after calculation {infoLabel}: {JsonConvert.SerializeObject(scenarioResults)}");
                _scenarioResultRepository.UpdateRange(scenarioResults);
                _scenarioResultRepository.SaveChanges();
                RaiseInfo($"Finished calculation of BRS Indicator {infoLabel}");

                return scenarioResults;
            }

            return null;
        }

        private void RaiseInfo(string message) => 
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, message));
    }
}

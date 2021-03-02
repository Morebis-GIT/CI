using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Domain.Spots;
using xggameplan.Model;
using xggameplan.OutputProcessors.Abstractions;

namespace xggameplan.core.OutputProcessors
{
    public class OutputImmutableDataSnapshot : IOutputDataSnapshot
    {
        public OutputImmutableDataSnapshot(RunWithScenarioReference runWithScenarioRef, IRunRepository runRepository,
            IScenarioRepository scenarioRepository, IPassRepository passRepository,
            ICampaignRepository campaignRepository, IScheduleRepository scheduleRepository,
            ISalesAreaRepository salesAreaRepository, IDemographicRepository demographicRepository,
            IProgrammeDictionaryRepository programmeDictionaryRepository, ISpotRepository spotRepository,
            IMetadataRepository metadataRepository)
        {
            var runStartDate = new Lazy<DateTime>(() => Run.Value.StartDate.Add(Run.Value.StartTime));
            var runEndDate = new Lazy<DateTime>(() => DateHelper.ConvertBroadcastToStandard(Run.Value.EndDate, Run.Value.EndTime));

            Run = new Lazy<Run>(() => runRepository.Find(runWithScenarioRef.RunId));
            Scenario = new Lazy<Scenario>(() => scenarioRepository.Get(runWithScenarioRef.ScenarioId), true);
            ScenarioPasses = new Lazy<IEnumerable<Pass>>(() => passRepository.FindByScenarioId(runWithScenarioRef.ScenarioId), true);
            AllCampaigns = new Lazy<IEnumerable<Campaign>>(campaignRepository.GetAll, true);
            AllSalesAreas = new Lazy<IEnumerable<SalesArea>>(salesAreaRepository.GetAll, true);
            AllDemographics = new Lazy<IEnumerable<Demographic>>(demographicRepository.GetAll, true);
            AllProgrammeDictionaries = new Lazy<IEnumerable<ProgrammeDictionary>>(programmeDictionaryRepository.GetAll, true);
            BreakTypes = new Lazy<IEnumerable<string>>(() => metadataRepository.GetByKey(MetaDataKeys.BreakTypes).Select(e => (string)e.Value));

            SpotsForRun = new Lazy<IEnumerable<Spot>>(() =>
            {
                var salesAreaPriorities = Run.Value.SalesAreaPriorities.Count == 0
                    ? AllSalesAreas.Value
                    : AllSalesAreas.Value.Where(sa => Run.Value.SalesAreaPriorities.Find(rsa => rsa.SalesArea == sa.Name) != null);

                return spotRepository.Search(
                    runStartDate.Value,
                    runEndDate.Value,
                    salesAreaPriorities.Select(sa => sa.Name).ToList()
                );
            },
                true
            );

            BreaksForRun = new Lazy<IEnumerable<BreakSimple>>(() =>
                    scheduleRepository.GetScheduleSimpleBreaks(
                        AllSalesAreas.Value.Select(c => c.Name).ToList(),
                        runStartDate.Value,
                        runEndDate.Value
                    ),
                true
            );
        }

        public Lazy<Run> Run { get; }
        public Lazy<Scenario> Scenario { get; }
        public Lazy<IEnumerable<Pass>> ScenarioPasses { get; }
        public Lazy<IEnumerable<BreakSimple>> BreaksForRun { get; }
        public Lazy<IEnumerable<Spot>> SpotsForRun { get; }
        public Lazy<IEnumerable<Campaign>> AllCampaigns { get; }
        public Lazy<IEnumerable<SalesArea>> AllSalesAreas { get; }
        public Lazy<IEnumerable<Demographic>> AllDemographics { get; }
        public Lazy<IEnumerable<ProgrammeDictionary>> AllProgrammeDictionaries { get; }
        public Lazy<IEnumerable<string>> BreakTypes { get; }
    }
}

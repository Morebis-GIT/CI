using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT9571_UpdateMaxRatingsRulesValues : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGT9571_UpdateMaxRatingsRulesValues(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            ValidateParametersBeforeUse(tenantConnectionStrings, updatesFolder);

            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        private const int MaxRatingsForSpotCampaignsRuleId = 21;
        private const int MaxRatingsForRatingCampaignsRuleId = 27;

        public Guid Id => new Guid("6dec4039-0260-460d-af7e-a2ede7851a27");

        public void Apply()
        {
            foreach (string tenantConnectionString in _tenantConnectionStrings)
            {
                using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                using (IDocumentSession session = documentStore.OpenSession())
                {
                    var scenarios = session.GetAll<Scenario>();

                    var passes = session.GetAll<Pass>();

                    var notStartedRuns = session.GetAll<Run>()
                        .FindAll(r => r.RunStatus == RunStatus.NotStarted && r.Scenarios.Any());

                    var notStartedRunScenarios = new List<Scenario>();

                    foreach (var run in notStartedRuns)
                    {
                        var scenarioIds = run.Scenarios.Select(x => x.Id);

                        notStartedRunScenarios.AddRange(scenarios.FindAll(s => scenarioIds.Contains(s.Id)));
                    }

                    var scenarioPasses = GetScenariosPasses(notStartedRunScenarios, passes);

                    var libraryScenarios = scenarios.FindAll(s => s.IsLibraried == true);

                    var libraryScenariosPasses = GetScenariosPasses(libraryScenarios, passes);

                    var libraryPasses = passes.FindAll(p => p.IsLibraried == true);

                    scenarioPasses.ForEach(UpdatePassRules);

                    libraryScenariosPasses.ForEach(UpdatePassRules);

                    libraryPasses.ForEach(UpdatePassRules);

                    session.SaveChanges();                  
                }
            }
        }

        private static List<Pass> GetScenariosPasses(List<Scenario> scenarios, List<Pass> allPasses)
        {
            var scenarioPassIds = new List<int>();

            foreach (var scenario in scenarios)
            {
                scenarioPassIds.AddRange(scenario.Passes.Select(p => p.Id));
            }

            return allPasses.FindAll(p => scenarioPassIds.Contains(p.Id));
        }

        private static void UpdatePassRules(Pass pass)
        {
            var spotCampaignsRule = pass.Rules.FirstOrDefault(r => r.RuleId == MaxRatingsForSpotCampaignsRuleId);

            if (spotCampaignsRule != null)
            {
                spotCampaignsRule.Ignore = true;
            }

            var ratingCampaignsRule = pass.Rules.FirstOrDefault(r => r.RuleId == MaxRatingsForRatingCampaignsRuleId);

            if (ratingCampaignsRule != null)
            {
                ratingCampaignsRule.Ignore = true;
            }
        }

        public void RollBack()
        {
            throw new NotImplementedException();
        }

        public int Sequence => 1;

        public string Name => "XGGT-9571 : Update existing max ratings rules, setting Ignore to true.";

        public bool SupportsRollback => false;

        private static void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, true);
        }
    }
}

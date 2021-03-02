using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Rules;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT12646_RenameMaxRatingsRules : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGT12646_RenameMaxRatingsRules(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            ValidateParametersBeforeUse(tenantConnectionStrings, updatesFolder);

            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        private const int MaxRatingsForSpotCampaignsRuleId = 21;
        private const int MaxRatingsForRatingCampaignsRuleId = 27;
        private const string MaxRatingsForSpotCampaignsDescription = "Max Rating Points for Spot Campaigns";
        private const string MaxRatingsForRatingCampaignsDescription = "Max Rating Points for Rating Campaigns";

        public Guid Id => new Guid("36bdb809-edb3-44dc-892c-8ab0d91dbdb6");

        public void Apply()
        {
            foreach (string tenantConnectionString in _tenantConnectionStrings)
            {
                using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                using (IDocumentSession session = documentStore.OpenSession())
                {
                    var scenarios = session.GetAll<Scenario>();

                    var passes = session.GetAll<Pass>();

                    var rules = session.GetAll<Rule>();

                    UpdateRules(rules);

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
                spotCampaignsRule.Description = MaxRatingsForSpotCampaignsDescription;
            }

            var ratingCampaignsRule = pass.Rules.FirstOrDefault(r => r.RuleId == MaxRatingsForRatingCampaignsRuleId);

            if (ratingCampaignsRule != null)
            {
                ratingCampaignsRule.Description = MaxRatingsForRatingCampaignsDescription;
            }
        }

        private static void UpdateRules(List<Rule> rules)
        {
            var spotCampaignsRule = rules.FirstOrDefault(r => r.RuleId == MaxRatingsForSpotCampaignsRuleId);

            if (spotCampaignsRule != null)
            {
                spotCampaignsRule.Description = MaxRatingsForSpotCampaignsDescription;
            }

            var ratingCampaignsRule = rules.FirstOrDefault(r => r.RuleId == MaxRatingsForRatingCampaignsRuleId);

            if (ratingCampaignsRule != null)
            {
                ratingCampaignsRule.Description = MaxRatingsForRatingCampaignsDescription;
            }
        }

        public void RollBack()
        {
            throw new NotImplementedException();
        }

        public int Sequence => 1;

        public string Name => "XGGT-12646 : Rename existing max ratings rules, adding '%'.";

        public bool SupportsRollback => false;

        private static void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, true);
        }
    }
}

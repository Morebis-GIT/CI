using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Autopilot.Rules;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Rules;
using ImagineCommunications.GamePlan.Domain.BusinessRules.RuleTypes;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;

namespace xggameplan.Updates.UpdateXGGT16602
{
    internal class UpdateStepXGGT16602_AddSpotBasedAutopilotRules : CodeUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;

        public UpdateStepXGGT16602_AddSpotBasedAutopilotRules(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            var connectionStrings = tenantConnectionStrings.ToList();
            ValidateParametersBeforeUse(connectionStrings, updatesFolder);

            _tenantConnectionStrings = connectionStrings;
            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("1C37EF13-3E71-4414-A97E-C3A78FDB80B1");

        public string Name => "XGGT-16602";

        public int Sequence => 1;

        public bool SupportsRollback => false;

        private static readonly IReadOnlyDictionary<int, int> PassRuleIdsMapping = new Dictionary<int, int>
        {
            {1, 28},
            {2, 29},
            {3, 30},
            {4, 31},
            {5, 32},
            {6, 33},
            {7, 34},
            {17, 44},
            {20, 47},
            {23, 50},
            {24, 51},
            {25, 52},
            {26, 53}
        };

        private static readonly IReadOnlyDictionary<int, int> ToleranceIdsMapping = new Dictionary<int, int>
        {
            {1, 19},
            {2, 20},
            {3, 21},
            {4, 22},
            {5, 23},
            {6, 24},
            {7, 25},
            {8, 26},
            {9, 27},
            {10, 28},
            {11, 29},
            {12, 30},
            {13, 31},
            {14, 32},
            {37, 38}
        };

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                using (var session = documentStore.OpenSession())
                {
                    var existingRules = session.GetAll<Rule>();
                    var existingAutopilotRules = session.GetAll<AutopilotRule>();

                    var newRules = GenerateSpotBasedRules(existingRules);
                    var newAutopilotRules = GenerateSpotBasedAutopilotRules(existingAutopilotRules);

                    newRules.ForEach(r => session.Store(r));
                    newAutopilotRules.ForEach(ar => session.Store(ar));
                    session.SaveChanges();
                }
            }
        }

        private static List<Rule> GenerateSpotBasedRules(List<Rule> existingRules)
        {
            var newRules = new List<Rule>();

            foreach (var existingRule in existingRules)
            {
                switch (existingRule.RuleTypeId)
                {
                    case (int)RuleCategory.Tolerances:
                    {
                        existingRule.CampaignType = CampaignDeliveryType.Rating;

                        if (ToleranceIdsMapping.TryGetValue(existingRule.RuleId, out var ruleId) &&
                            !existingRules.Exists(x => x.RuleTypeId == (int)RuleCategory.Tolerances && x.RuleId == ruleId))
                        {
                            var newRule = (Rule)existingRule.Clone();
                            newRule.Id = 0;
                            newRule.CampaignType = CampaignDeliveryType.Spot;
                            newRule.RuleId = ruleId;

                            newRules.Add(newRule);
                        }

                        break;
                    }
                    case (int)RuleCategory.Rules:
                    {
                        existingRule.CampaignType = CampaignDeliveryType.Spot;

                        if (PassRuleIdsMapping.TryGetValue(existingRule.RuleId, out var ruleId) &&
                            !existingRules.Exists(x => x.RuleTypeId == (int)RuleCategory.Rules && x.RuleId == ruleId))
                        {
                            var newRule = (Rule)existingRule.Clone();
                            newRule.Id = 0;
                            newRule.CampaignType = CampaignDeliveryType.Rating;
                            newRule.RuleId = ruleId;

                            newRules.Add(newRule);
                        }

                        break;
                    }
                }
            }

            return newRules;
        }

        private static List<AutopilotRule> GenerateSpotBasedAutopilotRules(List<AutopilotRule> existingRules)
        {
            var newRules = new List<AutopilotRule>();

            foreach (var existingRule in existingRules)
            {
                switch (existingRule.RuleTypeId)
                {
                    case (int)RuleCategory.Tolerances:
                    {
                        if (ToleranceIdsMapping.TryGetValue(existingRule.RuleId, out var ruleId) &&
                            !existingRules.Exists(x => x.RuleTypeId == (int)RuleCategory.Tolerances && x.RuleId == ruleId))
                        {
                            var newRule = (AutopilotRule)existingRule.Clone();
                            newRule.Id = 0;
                            newRule.RuleId = ruleId;

                            newRules.Add(newRule);
                        }

                        break;
                    }
                    case (int)RuleCategory.Rules:
                    {
                        if (PassRuleIdsMapping.TryGetValue(existingRule.RuleId, out var ruleId) &&
                            !existingRules.Exists(x => x.RuleTypeId == (int)RuleCategory.Rules && x.RuleId == ruleId))
                        {
                            var newRule = (AutopilotRule)existingRule.Clone();
                            newRule.Id = 0;
                            newRule.RuleId = ruleId;

                            newRules.Add(newRule);
                        }

                        break;
                    }
                }
            }

            return newRules;
        }

        private static void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings,
            string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, true);
        }

        public void RollBack() => throw new NotImplementedException();
    }
}

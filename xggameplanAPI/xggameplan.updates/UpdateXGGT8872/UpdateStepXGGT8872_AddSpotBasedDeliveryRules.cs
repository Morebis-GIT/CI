using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;

namespace xggameplan.Updates.UpdateXGGT8872
{
    internal class UpdateStepXGGT8872_AddSpotBasedDeliveryRules : CodeUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;

        public UpdateStepXGGT8872_AddSpotBasedDeliveryRules(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            var connectionStrings = tenantConnectionStrings.ToList();
            ValidateParametersBeforeUse(connectionStrings, updatesFolder);

            _tenantConnectionStrings = connectionStrings;
            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("E6CAB928-A23C-4A9F-B3D1-C23955BDEBB9");

        public string Name => "XGGT-8872";

        public int Sequence => 1;

        public bool SupportsRollback => false;

        private const int SpotCampaignsRuleId = 21;
        private const int RatingCampaignsRuleId = 27;

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
            {18, 45},
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
            {14, 32}
        };

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                using (var session = documentStore.OpenSession())
                {
                    var passes = session.GetAll<Pass>();

                    // Add new Rules And Tolerances to the Pass for spot based campaign delivery type
                    foreach (var pass in passes)
                    {
                        // Set campaign delivery type for exiting rules
                        pass.Rules.ForEach(r => r.CampaignType = r.RuleId == RatingCampaignsRuleId ? CampaignDeliveryType.Rating : CampaignDeliveryType.Spot);
                        pass.Tolerances.ForEach(r => r.CampaignType = CampaignDeliveryType.Rating);

                        GenerateRatingBasedPassRules(pass);
                        GenerateSpotBasedTolerances(pass);
                    }

                    session.SaveChanges();
                }
            }
        }

        private static void GenerateRatingBasedPassRules(Pass pass)
        {
            var newRules = new List<PassRule>();
            var ratingCampaignsRule = pass.Rules.FirstOrDefault(r => r.RuleId == RatingCampaignsRuleId);

            if (ratingCampaignsRule != null)
            {
                _ = pass.Rules.Remove(ratingCampaignsRule);
            }

            foreach (var oldRule in pass.Rules)
            {
                // adjust position of existing Max Ratings for Rating Campaigns rule to match with spot-based version
                if (oldRule.RuleId == SpotCampaignsRuleId && ratingCampaignsRule != null)
                {
                    newRules.Add(ratingCampaignsRule);
                    continue;
                }

                if (!PassRuleIdsMapping.ContainsKey(oldRule.RuleId) ||
                    pass.Rules.Exists(r => r.RuleId == PassRuleIdsMapping[oldRule.RuleId]))
                {
                    continue;
                }

                var newRule = (PassRule)oldRule.Clone();
                newRule.RuleId = PassRuleIdsMapping[oldRule.RuleId];
                newRule.CampaignType = CampaignDeliveryType.Rating;

                newRules.Add(newRule);
            }

            pass.Rules.AddRange(newRules);
        }

        private static void GenerateSpotBasedTolerances(Pass pass)
        {
            var newTolerances = new List<Tolerance>();

            foreach (var oldTolerance in pass.Tolerances)
            {
                if (!ToleranceIdsMapping.ContainsKey(oldTolerance.RuleId) ||
                    pass.Tolerances.Exists(r => r.RuleId == ToleranceIdsMapping[oldTolerance.RuleId]))
                {
                    continue;
                }

                var newTolerance = (Tolerance)oldTolerance.Clone();
                newTolerance.RuleId = ToleranceIdsMapping[oldTolerance.RuleId];
                newTolerance.CampaignType = CampaignDeliveryType.Spot;

                newTolerances.Add(newTolerance);
            }

            pass.Tolerances.AddRange(newTolerances);
        }

        private static void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings,
            string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, true);
        }

        public void RollBack()
        {
            throw new NotImplementedException();
        }
    }
}

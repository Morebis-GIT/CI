using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT13656
{
    internal class UpdateStepXGGT13656_AddNewCampaignKPIColumns : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;

        public UpdateStepXGGT13656_AddNewCampaignKPIColumns(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            var connectionStrings = tenantConnectionStrings.ToList();
            ValidateParametersBeforeUse(connectionStrings, updatesFolder);

            _tenantConnectionStrings = connectionStrings;
            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("6d1490fd-f891-4cae-9c93-59f2c412fda6");

        public int Sequence => 1;

        public string Name => "XGGT-13656";

        public bool SupportsRollback => false;

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString, null))
                using (var session = documentStore.OpenSession())
                {
                    foreach (var campaign in session.GetAll<Campaign>())
                    {
                        campaign.ActiveLength = null;
                        campaign.RatingsDifferenceExcludingPayback = null;
                        campaign.ValueDifference = null;
                        campaign.ValueDifferenceExcludingPayback = null;
                        campaign.AchievedPercentageTargetRatings = null;
                        campaign.AchievedPercentageRevenueBudget = null;
                    }
                    session.SaveChanges();
                }
            }
        }

        private static void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, throwOnInvalid: true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, throwOnInvalid: true);
        }

        public void RollBack() => throw new NotImplementedException();
    }
}

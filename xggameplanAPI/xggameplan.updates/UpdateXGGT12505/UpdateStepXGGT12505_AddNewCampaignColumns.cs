using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;

namespace xggameplan.Updates.UpdateXGGT12505
{
    internal class UpdateStepXGGT12505_AddNewCampaignColumns : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;

        public UpdateStepXGGT12505_AddNewCampaignColumns(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            var connectionStrings = tenantConnectionStrings.ToList();
            ValidateParametersBeforeUse(connectionStrings, updatesFolder);

            _tenantConnectionStrings = connectionStrings;
            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("b5ac43db-7d28-40d7-bc44-8d441ddbca33");

        public int Sequence => 1;

        public string Name => "XGGT-12505";

        public bool SupportsRollback => false;

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString, null))
                using (var session = documentStore.OpenSession())
                {
                    foreach (var campaign in session.GetAll<Campaign>().Where(x => !x.TargetZeroRatedBreaks))
                    {
                        campaign.CampaignPaybacks = null;
                        campaign.TargetXP = null;
                        campaign.RevenueBooked = null;
                        campaign.CreationDate = null;
                        campaign.AutomatedBooked = null;
                        campaign.TopTail = null;
                        campaign.Spots = null;

                        foreach (var campaignTarget in campaign.SalesAreaCampaignTarget)
                        {
                            foreach (var target in campaignTarget.CampaignTargets)
                            {
                                foreach (var strikeWeight in target.StrikeWeights)
                                {
                                    strikeWeight.Payback = null;
                                }
                            }
                        }
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

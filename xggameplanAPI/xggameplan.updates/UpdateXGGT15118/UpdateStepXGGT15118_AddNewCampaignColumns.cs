using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;

namespace xggameplan.Updates.UpdateXGGT15118
{
    internal class UpdateStepXGGT15118_AddNewCampaignColumns : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;

        public UpdateStepXGGT15118_AddNewCampaignColumns(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            var connectionStrings = tenantConnectionStrings.ToList();
            ValidateParametersBeforeUse(connectionStrings, updatesFolder);

            _tenantConnectionStrings = connectionStrings;
            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("5B76ACB9-2898-41A6-85DF-E068A5F93939");

        public int Sequence => 1;

        public string Name => "XGGT-15118";

        public bool SupportsRollback => false;

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString, null))
                using (var session = documentStore.OpenSession())
                {
                    foreach (var campaign in session.GetAll<Campaign>(x => x.CampaignPaybacks == null))
                    {
                        campaign.CampaignPaybacks = new List<CampaignPayback>();
                    }

                    foreach (var scenario in session.GetAll<Scenario>(x => x.CampaignPassPriorities != null && x.CampaignPassPriorities.Any()))
                    {
                        foreach (var campaignPassPriorities in scenario.CampaignPassPriorities)
                        {
                            campaignPassPriorities.Campaign.AgencyGroup = null;
                            campaignPassPriorities.Campaign.ReportingCategory = null;
                            campaignPassPriorities.Campaign.SalesExecutiveName = null;
                            campaignPassPriorities.Campaign.StopBooking = false;
                            campaignPassPriorities.Campaign.TargetXP = null;
                            campaignPassPriorities.Campaign.RevenueBooked = null;
                            campaignPassPriorities.Campaign.CreationDate = null;
                            campaignPassPriorities.Campaign.AutomatedBooked = null;
                            campaignPassPriorities.Campaign.TopTail = null;
                            campaignPassPriorities.Campaign.Spots = null;
                            campaignPassPriorities.Campaign.ActiveLength = null;
                            campaignPassPriorities.Campaign.RatingsDifferenceExcludingPayback = null;
                            campaignPassPriorities.Campaign.ValueDifference = null;
                            campaignPassPriorities.Campaign.ValueDifferenceExcludingPayback = null;
                            campaignPassPriorities.Campaign.AchievedPercentageTargetRatings = null;
                            campaignPassPriorities.Campaign.AchievedPercentageRevenueBudget = null;
                            campaignPassPriorities.Campaign.CampaignPaybacks = new List<CampaignPayback>();
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

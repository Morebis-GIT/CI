using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;

namespace xggameplan.Updates.UpdateXGGT14199
{
    internal class UpdateStepXGGT14199_AddNewCampaignFields : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;

        public UpdateStepXGGT14199_AddNewCampaignFields(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            var connectionStrings = tenantConnectionStrings.ToList();
            ValidateParametersBeforeUse(connectionStrings, updatesFolder);

            _tenantConnectionStrings = connectionStrings;
            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("6bbd9e0c-25f0-4355-991a-956c2c5668f9");

        public int Sequence => 1;

        public string Name => "XGGT-14199";

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
                        foreach (var salesAreaCampaignTarget in campaign.SalesAreaCampaignTarget)
                        {
                            foreach (var campaignTarget in salesAreaCampaignTarget.CampaignTargets)
                            {
                                foreach (var strikeWeight in campaignTarget.StrikeWeights)
                                {
                                    foreach (var dayPart in strikeWeight.DayParts)
                                    {
                                        dayPart.TotalSpotCount = 0;
                                        dayPart.ZeroRatedSpotCount = 0;
                                        dayPart.Ratings = 0;
                                        dayPart.BaseDemographRatings = 0;
                                        dayPart.NominalValue = 0;
                                    }
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

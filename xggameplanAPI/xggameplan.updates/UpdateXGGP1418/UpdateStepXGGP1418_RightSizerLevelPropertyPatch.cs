using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGP1418_RightSizerLevelPropertyPatch : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGP1418_RightSizerLevelPropertyPatch(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            ValidateParametersBeforeUse(tenantConnectionStrings, updatesFolder);

            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("fb2725b8-e950-48b9-944c-e76e4adefe6b");

        public void Apply()
        {
            foreach (string tenantConnectionString in _tenantConnectionStrings)
            {
                using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                using (IDocumentSession session = documentStore.OpenSession())
                {
                    var campaigns = session.GetAll<Campaign>();
                    foreach (var campaign in campaigns)
                    {
                        if (!campaign.RightSizerLevel.HasValue && campaign.IncludeRightSizer)
                        {
                            campaign.RightSizerLevel = RightSizerLevel.CampaignLevel;
                        }
                    }

                    session.SaveChanges();
                }
            }
        }

        public void RollBack()
        {
            throw new NotImplementedException();
        }

        public int Sequence => 1;

        public string Name => "XGGP-1418 : Right Sizer Level Property Patch";

        public bool SupportsRollback => false;

        private void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, throwOnInvalid: true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, throwOnInvalid: true);
        }
    }
}

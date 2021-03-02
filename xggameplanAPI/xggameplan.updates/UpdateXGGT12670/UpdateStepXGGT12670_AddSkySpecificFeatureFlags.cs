using System;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Shared.System.Features;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT12670
{
    internal class UpdateStepXGGT12670_AddSkySpecificFeatureFlags : PatchUpdateStepBase, IUpdateStep
    {
        private readonly string _masterConnectionString;

        public UpdateStepXGGT12670_AddSkySpecificFeatureFlags(string masterConnectionString, string updatesFolder)
        {
            _masterConnectionString = masterConnectionString;

            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("02E04FCD-A45A-4A44-8FC7-17835B7172AE");

        public int Sequence => 1;

        public string Name => "XGGT-12670";

        public bool SupportsRollback => false;

        public void Apply()
        {
            using (var documentStore = DocumentStoreFactory.CreateStore(_masterConnectionString, null))
            using (var session = documentStore.OpenSession())
            {
                foreach (int tenantId in session.GetAll<Tenant>().Select(t => t.Id))
                {
                    session.Store(new TenantProductFeature
                    {
                        Name = nameof(ProductFeature.NineValidationMinSpot),
                        TenantId = tenantId,
                        IsShared = false,
                        Enabled = true
                    });
                    session.Store(new TenantProductFeature
                    {
                        Name = nameof(ProductFeature.NineValidationRatingPredictions),
                        TenantId = tenantId,
                        IsShared = false,
                        Enabled = true
                    });
                    session.Store(new TenantProductFeature
                    {
                        Name = nameof(ProductFeature.IncludeChannelGroupFileForOptimiser),
                        TenantId = tenantId,
                        IsShared = false,
                        Enabled = true
                    });
                }

                session.SaveChanges();
            }
        }

        public void RollBack() => throw new NotImplementedException();
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Shared.System.Features;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;

namespace xggameplan.Updates.UpdateXGGT9876
{
    internal class UpdateXGGT9876_AddDefaultFeatures : PatchUpdateStepBase, IUpdateStep
    {
        private readonly string _masterConnectionString;

        public UpdateXGGT9876_AddDefaultFeatures(string masterConnectionString, string updatesFolder)
        {
            _masterConnectionString = masterConnectionString;

            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("F57E93CB-8273-4188-B0D8-2811814B79B7");

        public int Sequence => 1;

        public string Name => "XGGT-9876";

        public bool SupportsRollback => false;

        public void Apply()
        {
            using (var documentStore = DocumentStoreFactory.CreateStore(_masterConnectionString, null))
            using (var session = documentStore.OpenSession())
            {
                var existingFeatures = session.GetAll<TenantProductFeature>().Select(f => f.Name).ToHashSet();
                var newFeatures = Enum.GetValues(typeof(ProductFeature))
                    .Cast<ProductFeature>()
                    .Except(new[] {default(ProductFeature)})
                    .Where(name => !existingFeatures.Contains(name.ToString()))
                    .ToList();
                var defaultFeatures = new List<TenantProductFeature>();

                // add all features with disabled state
                foreach (int tenantId in session.GetAll<Tenant>().Select(t => t.Id))
                {
                    defaultFeatures.AddRange(newFeatures
                        .Select(feature => new TenantProductFeature
                        {
                            Name = feature.ToString(),
                            TenantId = tenantId,
                            IsShared = IsShared(feature),
                            Enabled = false
                        }));
                }

                foreach (var feature in defaultFeatures)
                {
                    session.Store(feature);
                }

                session.SaveChanges();
            }
        }

        private static bool IsShared(ProductFeature feature) =>
            feature == ProductFeature.CampaignDeliveryType ||
            feature == ProductFeature.DeliveryCappingGroup ||
            feature == ProductFeature.LandmarkBooking ||
            feature == ProductFeature.SalesAreaZeroRevenueSplit ||
            feature == ProductFeature.TargetSalesArea ||
            feature == ProductFeature.ZeroRatedBreaksMapping ||
            feature == ProductFeature.InventoryStatus;

        public void RollBack() => throw new NotImplementedException();
    }
}

using System;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Shared.System.Features;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT14246_AddLandmarkBookingProductFeature : PatchUpdateStepBase, IUpdateStep
    {
        private readonly string _masterConnectionString;

        public UpdateStepXGGT14246_AddLandmarkBookingProductFeature(string masterConnectionString, string updatesFolder)
        {
            _masterConnectionString = masterConnectionString;

            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("be665af6-95d9-4f08-894c-5a4df61d00ed");

        public void Apply()
        {
            using (var documentStore = DocumentStoreFactory.CreateStore(_masterConnectionString, null))
            using (var session = documentStore.OpenSession())
            {
                var existingFlag = session
                    .GetAll<TenantProductFeature>()
                    .FirstOrDefault(x => x.Name == nameof(ProductFeature.LandmarkBooking));

                if (existingFlag != null)
                {
                    return;
                }

                foreach (int tenantId in session.GetAll<Tenant>().Select(t => t.Id))
                {
                    session.Store(new TenantProductFeature
                    {
                        Name = nameof(ProductFeature.LandmarkBooking),
                        TenantId = tenantId,
                        IsShared = true,
                        Enabled = false
                    });
                }

                session.SaveChanges();
            }
        }

        public void RollBack() => throw new NotImplementedException();

        public int Sequence => 1;

        public string Name => "XGGT-14246";

        public bool SupportsRollback => false;
    }
}

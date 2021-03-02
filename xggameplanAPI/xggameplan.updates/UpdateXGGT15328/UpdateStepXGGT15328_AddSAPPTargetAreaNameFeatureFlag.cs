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
    internal class UpdateStepXGGT15328_AddSAPPTargetAreaNameFeatureFlag : IUpdateStep
    {
        private readonly string _masterConnectionString;

        public UpdateStepXGGT15328_AddSAPPTargetAreaNameFeatureFlag(string masterConnectionString, string updatesFolder)
        {
            ValidateParametersBeforeUse(masterConnectionString, updatesFolder);
            _masterConnectionString = masterConnectionString;

            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("f23d918d-f980-4e20-bf2e-2399cbd59c4a");
        public string Name => "XGGT-15328: Add SAPPTargetAreaName FeatureFlag into the corresponding collection";
        public int Sequence => 1;

        public void Apply()
        {
            using (var documentStore = DocumentStoreFactory.CreateStore(_masterConnectionString, null))
            using (var session = documentStore.OpenSession())
            {
                var existingFlag = session
                    .GetAll<TenantProductFeature>()
                    .FirstOrDefault(x => x.Name == nameof(ProductFeature.SAPPTargetAreaName));

                if (existingFlag != null)
                {
                    return;
                }

                foreach (int tenantId in session.GetAll<Tenant>().Select(t => t.Id))
                {
                    session.Store(new TenantProductFeature
                    {
                        Name = nameof(ProductFeature.SAPPTargetAreaName),
                        TenantId = tenantId,
                        IsShared = true,
                        Enabled = false
                    });
                }

                session.SaveChanges();
            }
        }

        public bool SupportsRollback => false;

        public void RollBack() => throw new NotImplementedException();

        private static void ValidateParametersBeforeUse(string masterConnectionString, string updatesFolder)
        {
            _ = UpdateValidator.ValidateMasterConnectionString(masterConnectionString, throwOnInvalid: true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, throwOnInvalid: true);
        }
    }
}

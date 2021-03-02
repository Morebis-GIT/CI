using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT10244_AddMidnightStartAndEndTimeToTenantSettings : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGT10244_AddMidnightStartAndEndTimeToTenantSettings(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("bc39ec47-ce8c-4214-89e6-4595dc8e8bc7");

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString, null))
                using (var session = documentStore.OpenSession())
                {
                    var tenantSettings = session.GetAll<TenantSettings>()[0];

                    tenantSettings.MidnightStartTime = "240000";
                    tenantSettings.MidnightEndTime = "995959";

                    session.SaveChanges();
                }
            }
        }

        public void RollBack() => throw new NotImplementedException();

        public int Sequence => 1;

        public string Name => "XGGT-10244: Add Midnight start and end time to Tenant settings";

        public bool SupportsRollback => false;
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;

namespace xggameplan.Updates.UpdateXGGT4159
{
    internal class UpdateStepXGGT4159_DefaultTargetAreaName : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;

        public UpdateStepXGGT4159_DefaultTargetAreaName(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            var connectionStrings = tenantConnectionStrings.ToList();
            ValidateParametersBeforeUse(connectionStrings, updatesFolder);

            _tenantConnectionStrings = connectionStrings;
            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("E278DD57-ACF4-42A6-A746-D8190794E189");

        public int Sequence => 1;

        public string Name => "XGGT-4159";

        public bool SupportsRollback => false;

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString, null))
                using (var session = documentStore.OpenSession())
                {
                    var salesAreas = session.GetAll<SalesArea>();

                    // Default TargetAreaName
                    foreach (var salesArea in salesAreas)
                    {
                        salesArea.TargetAreaName = salesArea.Name;
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

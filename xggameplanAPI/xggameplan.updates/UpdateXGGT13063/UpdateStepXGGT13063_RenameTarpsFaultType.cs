using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT13063_RenameTarpsFaultType : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGT13063_RenameTarpsFaultType(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            ValidateParametersBeforeUse(tenantConnectionStrings, updatesFolder);

            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("1e733486-6001-4395-b0e9-082167801253");

        public string Name => "XGGT-13063 : TARPs Fault Type rename to Rating Points";

        public int Sequence => 1;

        private const int TarpsFaultTypeId = 82;
        private const string SlottingControlsFunctionalAreaDescription = "Slotting Controls";
        private const string TarpsFaultTypeDescriptionNew = "Min Rating Points not met";

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                using (var session = documentStore.OpenSession())
                {
                    var functionalAreas = session.GetAll<FunctionalArea>();

                    var slottingControlsFunctionalArea =
                        functionalAreas.FirstOrDefault(fa => fa.Description.ContainsValue(SlottingControlsFunctionalAreaDescription));

                    var tarpsFaultType = slottingControlsFunctionalArea?.FaultTypes.FirstOrDefault(ft => ft.Id == TarpsFaultTypeId);

                    if (tarpsFaultType is null)
                    {
                        return;
                    }

                    foreach (var language in tarpsFaultType.Description.Keys.ToArray())
                    {
                        tarpsFaultType.Description[language] = TarpsFaultTypeDescriptionNew;
                    }

                    session.SaveChanges();
                }
            }
        }

        public bool SupportsRollback => false;

        public void RollBack()
        {
            throw new NotImplementedException();
        }

        private static void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, true);
        }
    }
}

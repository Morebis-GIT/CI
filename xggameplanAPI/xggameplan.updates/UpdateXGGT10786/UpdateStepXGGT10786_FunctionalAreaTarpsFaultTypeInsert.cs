using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT10786_FunctionalAreaTarpsFaultTypeInsert : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGT10786_FunctionalAreaTarpsFaultTypeInsert(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            ValidateParametersBeforeUse(tenantConnectionStrings, updatesFolder);

            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("25e02b25-d818-4a45-b2c1-8d371cf36102");

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                using (var session = documentStore.OpenSession())
                {
                    const int tarpsFaultTypeId = 82;
                    const string slottingControlsFunctionalAreaDescription = "Slotting Controls";
                    const string tarpsFaultTypeDescription = "Min TARPs not met";

                    var functionalAreas = session.GetAll<FunctionalArea>();

                    var slottingControlsFunctionalArea =
                        functionalAreas.FirstOrDefault(fa => fa.Description.ContainsValue(slottingControlsFunctionalAreaDescription));

                    if (slottingControlsFunctionalArea is null)
                    {
                        var descriptionDictionary = new Dictionary<string, string>();

                        foreach (var language in Globals.SupportedLanguages)
                        {
                            descriptionDictionary.Add(language, slottingControlsFunctionalAreaDescription);
                        }

                        slottingControlsFunctionalArea = new FunctionalArea
                        {
                            Description = descriptionDictionary,
                            FaultTypes = new List<FaultType>()
                        };

                        session.Store(slottingControlsFunctionalArea);
                    }

                    if (slottingControlsFunctionalArea.FaultTypes.All(
                        ft => ft.Id != tarpsFaultTypeId))
                    {
                        var faultTypeDescriptionDictionary = new Dictionary<string, string>();

                        foreach (var language in Globals.SupportedLanguages)
                        {
                            faultTypeDescriptionDictionary.Add(language, tarpsFaultTypeDescription);
                        }

                        slottingControlsFunctionalArea.FaultTypes.Add(new FaultType
                        {
                            Id = tarpsFaultTypeId,
                            Description = faultTypeDescriptionDictionary,
                            IsSelected = true
                        });
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

        public string Name => "XGGT-10786 : TARPs Fault Type Insert";

        public bool SupportsRollback => false;

        private static void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, true);
        }
    }
}

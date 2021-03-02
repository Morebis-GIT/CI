using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT1838_FunctionalAreaAndFaultTypeInsert : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGT1838_FunctionalAreaAndFaultTypeInsert(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            ValidateParametersBeforeUse(tenantConnectionStrings, updatesFolder);

            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("58cb2efb-3ab9-4041-adec-cc0cf00d4c88");

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                using (var session = documentStore.OpenSession())
                {
                    const int programmeRequirementFaultTypeId = 78;
                    const string campaignRequirementFunctionalAreaName = "Campaign Requirement";

                    var functionalAreas = session.GetAll<FunctionalArea>();

                    var programmeRequirementFunctionalArea =
                        functionalAreas.FirstOrDefault(fa => fa.Description.ContainsValue(campaignRequirementFunctionalAreaName));

                    if (programmeRequirementFunctionalArea is null)
                    {
                        programmeRequirementFunctionalArea = new FunctionalArea
                        {
                            Description = new Dictionary<string, string>
                            {
                                { "ENG", campaignRequirementFunctionalAreaName },
                                { "ARA" , campaignRequirementFunctionalAreaName }
                            },
                            FaultTypes = new List<FaultType>()
                        };

                        session.Store(programmeRequirementFunctionalArea);
                    }

                    if (programmeRequirementFunctionalArea.FaultTypes.All(
                        ft => ft.Id != programmeRequirementFaultTypeId))
                    {
                        programmeRequirementFunctionalArea.FaultTypes.Add(new FaultType
                        {
                            Id = programmeRequirementFaultTypeId,
                            Description = new Dictionary<string, string>
                            {
                                { "ENG", "Programme Requirement (Achieved/Oversupply)" },
                                { "ARA" , "Programme Requirement (Achieved/Oversupply)" }
                            },
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

        public string Name => "XGGT-1838 : Functional Area And Fault Type Insert";

        public bool SupportsRollback => false;

        private static void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, true);
        }
    }
}


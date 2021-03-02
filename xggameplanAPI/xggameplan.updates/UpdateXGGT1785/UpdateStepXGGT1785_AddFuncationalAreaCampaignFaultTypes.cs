using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT1785_AddFuncationalAreaCampaignFaultTypes : PatchUpdateStepBase, IUpdateStep
    {
        private const string _campaignRequirements = "Campaign Requirement";

        private readonly Dictionary<int, string> _faultTypes = new Dictionary<int, string>()
        {
            { 79, "Sponsorship Restriction applied for Competitor Spot based on Competing <Clash>" },
            { 80, "Sponsorship Restriction applied for Competitor Spot based on Competing <Advertiser>" }
        };

        private readonly string _rollBackFolder;
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;

        public UpdateStepXGGT1785_AddFuncationalAreaCampaignFaultTypes(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            ValidateParametersBeforeUse(tenantConnectionStrings, updatesFolder);

            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("dee5aec6-c865-4e08-95ca-44ebc5b28e28");

        public string Name => "XGGT-1785 : Add Functional Area Campaign Failure Message IDs (79,80)";

        public int Sequence => 1;

        public bool SupportsRollback => false;

        public void Apply()
        {
            foreach (string tenantConnectionString in _tenantConnectionStrings)
            {
                using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                using (IDocumentSession session = documentStore.OpenSession())
                {
                    FunctionalArea functionalArea = getFunctionalAreaByDescription(session, _campaignRequirements);

                    if (functionalArea != null)
                    {
                        foreach (var ft in _faultTypes)
                        {
                            addFaultTypeToFunctionalArea(functionalArea,
                                                         generateFaultType(ft.Key, ft.Value));
                        }
                    }

                    session.SaveChanges();
                }
            }
        }

        public void RollBack() => throw new NotImplementedException();

        private static void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, true);
        }

        private void addFaultTypeToFunctionalArea(FunctionalArea functionalArea, FaultType faultType)
        {
            if (!functionalArea.FaultTypes.Exists(f => f.Id == faultType.Id))
            {
                functionalArea.FaultTypes.Add(faultType);
            }
        }

        private FaultType generateFaultType(int id, string description, bool isSelected = true)
        {
            var descriptionDictionary = new Dictionary<string, string>();
            foreach (var language in Globals.SupportedLanguages)
            {
                descriptionDictionary.Add(language, description);
            }

            return new FaultType()
            {
                Id = id,
                IsSelected = isSelected,
                Description = descriptionDictionary
            };
        }

        private FunctionalArea getFunctionalAreaByDescription(IDocumentSession session, string description)
        {
            return session.GetAll<FunctionalArea>()
                                    .Where(f => f.Description.ContainsValue(description))
                                    .FirstOrDefault();
        }
    }
}

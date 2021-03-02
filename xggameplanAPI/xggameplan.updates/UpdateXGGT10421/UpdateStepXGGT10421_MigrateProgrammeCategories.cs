using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT10421
{
    internal class UpdateStepXGGT10421_MigrateProgrammeCategories : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;

        public UpdateStepXGGT10421_MigrateProgrammeCategories(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _tenantConnectionStrings = tenantConnectionStrings;
            var connectionStrings = tenantConnectionStrings.ToList();
            ValidateParametersBeforeUse(connectionStrings, updatesFolder);

            _tenantConnectionStrings = connectionStrings;
            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }
        public Guid Id => new Guid("E8BFF4D4-66BF-4906-8471-017127B7AD3C");

        public int Sequence => 1;

        public string Name => "XGGT-10421";

        public bool SupportsRollback => false;

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString, null))
                using (var session = documentStore.OpenSession())
                {
                    var metadataRepository = new RavenMetadataRepository(session);
                    var metadataCategories = metadataRepository.GetByKey(MetaDataKeys.ProgramCategories) ;
                    if (metadataCategories!= null && metadataCategories.Any())
                    {
                        var existedProgrammeCategories = session.GetAll<ProgrammeCategoryHierarchy>();

                        var categoriesToAdd = metadataCategories.Where(x => !existedProgrammeCategories.Any(pc => pc
                            .Name
                            .Equals(x.Value.ToString(), StringComparison.OrdinalIgnoreCase))).ToList();

                        if (categoriesToAdd.Any())
                        {
                            foreach (var programmeCategory in categoriesToAdd)
                            {
                                session.Store(
                                    new ProgrammeCategoryHierarchy {Name = programmeCategory.Value.ToString()});
                            }
                        }
                        metadataRepository.DeleteByKey(MetaDataKeys.ProgramCategories);
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

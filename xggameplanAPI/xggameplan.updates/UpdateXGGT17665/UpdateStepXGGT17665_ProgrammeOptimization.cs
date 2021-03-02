using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.DbContext;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT17665
{
    internal class UpdateStepXGGT17665_ProgrammeOptimization : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;

        public UpdateStepXGGT17665_ProgrammeOptimization(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _tenantConnectionStrings = tenantConnectionStrings;

            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("210D37A4-0A53-4296-8E5D-15D6620553FB");

        public int Sequence => 1;

        public string Name => "XGGT-17665";

        public bool SupportsRollback => throw new NotImplementedException();

        public void Apply()
        {
            foreach (var cs in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(cs, null))
                using (var dbContext = new RavenDbContext(documentStore.OpenSession(), documentStore.OpenAsyncSession()))
                {
                    dbContext.Truncate<ProgrammeDictionary>();

                    var cache = new Dictionary<string, ProgrammeDictionary>();

                    foreach (var programme in dbContext.Specific.GetAllWithNoTracking<Programme>())
                    {
                        if (!cache.TryGetValue(programme.ExternalReference, out var dictionary))
                        {
                            dictionary = new ProgrammeDictionary { ExternalReference = programme.ExternalReference };
                            cache.Add(programme.ExternalReference, dictionary);
                        }

                        dictionary.ProgrammeName = programme.ProgrammeName;
                        dictionary.Description = programme.Description;
                        dictionary.Classification = programme.Classification;
                    }

                    dbContext.AddRange(cache.Values.ToArray());
                }
            }
        }

        public void RollBack() => throw new NotImplementedException();
    }
}

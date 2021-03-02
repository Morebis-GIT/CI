using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Raven.Client.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenProgrammeCategoryHierarchyRepository : IProgrammeCategoryHierarchyRepository
    {
        private readonly IDocumentSession _session;

        public RavenProgrammeCategoryHierarchyRepository(IDocumentSession session)
        {
            _session = session;
        }

        public IEnumerable<ProgrammeCategoryHierarchy> GetAll() => _session.GetAll<ProgrammeCategoryHierarchy>();

        public void AddRange(IEnumerable<ProgrammeCategoryHierarchy> programmeCategories)
        {
            lock (_session)
            {
                var options = new BulkInsertOptions {OverwriteExisting = true};
                using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert(null, options))
                {
                    programmeCategories.ForEach(item => bulkInsert.Store(item));
                }
            }
        }

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }

        public void Truncate() => _session.TryDelete("Raven/DocumentsByEntityName", "ProgrammeCategoryHierarchies");

        public IEnumerable<ProgrammeCategoryHierarchy> Search(IEnumerable<string> programmeCategoryNames) =>
            _session.GetAll<ProgrammeCategoryHierarchy>(currentItem => currentItem.Name.In(programmeCategoryNames));

        public ProgrammeCategoryHierarchy Get(int id) => _session.Load<ProgrammeCategoryHierarchy>(id);
    }
}

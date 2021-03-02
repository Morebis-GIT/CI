using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.LengthFactors;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.DbContext;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Abstractions.Extensions;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenLengthFactorRepository : ILengthFactorRepository
    {
        private readonly IDocumentSession _session;

        public RavenLengthFactorRepository(IDocumentSession session)
        {
            _session = session;
        }

        public void AddRange(IEnumerable<LengthFactor> items)
        {
            using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert(null, new BulkInsertOptions() { OverwriteExisting = true }))
            {
                items.ForEach(item => bulkInsert.Store(item));
            }
        }

        public void Delete(int id)
        {
            lock (_session)
            {
                var item = Get(id);
                if (item != null)
                {
                    _session.Delete(item);
                }
            }
        }

        public LengthFactor Get(int id) => _session.Load<LengthFactor>(id);

        public IEnumerable<LengthFactor> GetAll() => _session.GetAll<LengthFactor>();

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }

        public void Truncate() => _session.TryDelete("Raven/DocumentsByEntityName", "LengthFactors");

        public void Update(LengthFactor item)
        {
            lock (_session)
            {
                _session.Store(item);
            }
        }
    }
}

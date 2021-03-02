using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Objects;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenLockTypeRepository : ILockTypeRepository
    {
        private readonly IDocumentSession _session;

        public RavenLockTypeRepository(IDocumentSession session)
        {
            _session = session;
        }

        public IEnumerable<InventoryLockType> GetAll() => _session.GetAll<InventoryLockType>();

        public InventoryLockType Get(int id) => _session.Load<InventoryLockType>(id);

        public void AddRange(IEnumerable<InventoryLockType> lockTypes)
        {
            lock (_session)
            {
                var options = new BulkInsertOptions {OverwriteExisting = true};
                using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert(null, options))
                {
                    lockTypes.ForEach(item => bulkInsert.Store(item));
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

        public void Truncate() => _session.TryDelete("Raven/DocumentsByEntityName", "InventoryLockTypes");
    }
}

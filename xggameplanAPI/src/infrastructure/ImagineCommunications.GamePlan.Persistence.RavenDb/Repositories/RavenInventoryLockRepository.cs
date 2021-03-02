using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Objects;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Raven.Client.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenInventoryLockRepository : IInventoryLockRepository
    {
        private readonly IDocumentSession _session;

        public RavenInventoryLockRepository(IDocumentSession session)
        {
            _session = session;
        }

        public IEnumerable<InventoryLock> GetAll() => _session.GetAll<InventoryLock>();

        public InventoryLock Get(int id) => _session.Load<InventoryLock>(id);

        public void AddRange(IEnumerable<InventoryLock> lockTypes)
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

        public void Truncate() => _session.TryDelete("Raven/DocumentsByEntityName", "InventoryLocks");

        public void DeleteRange(IEnumerable<string> salesAreas)
        {
            lock (_session)
            {
                var inventoryLocks = _session.GetAll<InventoryLock>(s => s.SalesArea.In(salesAreas.ToList()));
                foreach (var inventoryLock in inventoryLocks)
                {
                    _session.Delete(inventoryLock);
                }
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Objects;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenInventoryTypeRepository : IInventoryTypeRepository
    {
        private const string IsSystemInventoryType = "Y";
        private readonly IDocumentSession _session;

        public RavenInventoryTypeRepository(IDocumentSession session)
        {
            _session = session;
        }

        public IEnumerable<InventoryType> GetAll() => _session.GetAll<InventoryType>();

        public InventoryType Get(int id) => _session.Load<InventoryType>(id);

        public IEnumerable<InventoryType> GetSystemInventories()
        {
            return _session.GetAll<InventoryType>(s => s.System == IsSystemInventoryType);
        }

        public void AddRange(IEnumerable<InventoryType> lockTypes)
        {
            lock (_session)
            {
                var options = new BulkInsertOptions { OverwriteExisting = true };
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

        public void Truncate() => _session.TryDelete("Raven/DocumentsByEntityName", "InventoryTypes");

    }
}

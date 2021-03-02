using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.DayParts.Objects;
using ImagineCommunications.GamePlan.Domain.DayParts.Repositories;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenStandardDayPartGroupRepository : IStandardDayPartGroupRepository
    {
        private readonly IDocumentSession _session;

        public RavenStandardDayPartGroupRepository(IDocumentSession session)
        {
            _session = session;
        }

        public IEnumerable<StandardDayPartGroup> GetAll() => _session.GetAll<StandardDayPartGroup>();

        public StandardDayPartGroup Get(int id) => _session.Load<StandardDayPartGroup>(id);

        public void AddRange(IEnumerable<StandardDayPartGroup> dayPartGroups)
        {
            lock (_session)
            {
                var options = new BulkInsertOptions {OverwriteExisting = true};
                using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert(null, options))
                {
                    dayPartGroups.ForEach(item => bulkInsert.Store(item));
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

        public void Truncate() => _session.TryDelete("Raven/DocumentsByEntityName", "StandardDayPartGroups");
    }
}

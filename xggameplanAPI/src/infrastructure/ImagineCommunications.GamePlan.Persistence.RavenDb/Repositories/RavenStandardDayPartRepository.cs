using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.DayParts.Objects;
using ImagineCommunications.GamePlan.Domain.DayParts.Repositories;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Raven.Client.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenStandardDayPartRepository : IStandardDayPartRepository
    {
        private readonly IDocumentSession _session;

        public RavenStandardDayPartRepository(IDocumentSession session)
        {
            _session = session;
        }

        public IEnumerable<StandardDayPart> GetAll() => _session.GetAll<StandardDayPart>();

        public StandardDayPart Get(int id) => _session.Load<StandardDayPart>(id);

        public void AddRange(IEnumerable<StandardDayPart> dayParts)
        {
            lock (_session)
            {
                var options = new BulkInsertOptions {OverwriteExisting = true};
                using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert(null, options))
                {
                    dayParts.ForEach(item => bulkInsert.Store(item));
                }
            }
        }

        public IEnumerable<StandardDayPart> FindByExternal(List<int> externalIds) =>
            _session.GetAll<StandardDayPart>(currentItem => currentItem.DayPartId.In(externalIds));

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }

        public void Truncate() => _session.TryDelete("Raven/DocumentsByEntityName", "StandardDayParts");
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenSmoothFailureMessageRepository : ISmoothFailureMessageRepository
    {
        private readonly IDocumentSession _session;

        public RavenSmoothFailureMessageRepository(IDocumentSession session)
        {
            _session = session;
        }

        public IEnumerable<SmoothFailureMessage> GetAll()
        {
            List<SmoothFailureMessage> failureTypes;
            lock (_session)
            {
                failureTypes = _session.Query<SmoothFailureMessage>().Take(Int32.MaxValue).ToList();
            }
            return failureTypes;
        }

        public void Add(IEnumerable<SmoothFailureMessage> items)
        {
            using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert())
            {
                items.ToList().ForEach(item => _session.Store(item));
            }
        }

        public void Truncate() => throw new NotImplementedException();

        public void SaveChanges() => throw new NotImplementedException();
    }
}

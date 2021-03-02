using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.IndexTypes;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Abstractions.Data;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenIndexTypeRepository : IIndexTypeRepository
    {
        private readonly IDocumentSession _session;

        public RavenIndexTypeRepository(IDocumentSession session)
        {
            _session = session;
        }

        public void Add(IEnumerable<IndexType> items)
        {
            using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert())
            {
                items.ToList().ForEach(item => _session.Store(item));
            }
        }

        public void Update(IndexType item)
        {
            lock(_session)
            {
                _session.Store(item);
            }
        }

        public IndexType Find(int id)
        {
            lock (_session)
            {
                return _session.Load<IndexType>(id);
            }
        }

        public IEnumerable<IndexType> GetAll()
        {
            lock( _session)
            {
                return _session.GetAll<IndexType>();
            }
        }

        public void Remove(int id)
        {
            lock(_session)
            {
                _session.Delete<IndexType>(id);
            }
        }

        public int CountAll
        {
            get
            {
                lock (_session)
                {
                    return _session.CountAll<IndexType>();
                }
            }
        }

        public void Truncate()
        {
            lock (_session)
            {
                _session.Advanced.DocumentStore.DatabaseCommands.DeleteByIndex("Raven/DocumentsByEntityName", new IndexQuery
                {
                    Query = "Tag:[[IndexTypes]]"
                }).WaitForCompletion();
            }
        }

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }
    }
}

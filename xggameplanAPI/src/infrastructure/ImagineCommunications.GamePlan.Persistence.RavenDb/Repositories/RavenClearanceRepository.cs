using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ImagineCommunications.GamePlan.Domain.Shared.ClearanceCodes;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenClearanceRepository : IClearanceRepository
    {
        private readonly IDocumentSession _session;

        public RavenClearanceRepository(IDocumentSession session)
        {
            _session = session;
        }

        public void Add(ClearanceCode item)
        {
            lock (_session)
            {
                _session.Store(item);
            }
        }

        public void Add(IEnumerable<ClearanceCode> item)
        {
            lock (_session)
            {
                var options = new BulkInsertOptions() { OverwriteExisting = true };
                using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert(null, options))
                {
                    item.ToList().ForEach(i =>

                        bulkInsert.Store(i));
                }
            }
        }

        public IEnumerable<ClearanceCode> GetAll()
        {
            lock (_session)
            {
                return _session.GetAll<ClearanceCode>();
            }
        }

        public ClearanceCode Find(int id)
        {
            lock (_session)
            {
                return _session.Load<ClearanceCode>(id);
            }
        }

        public int CountAll
        {
            get
            {
                lock (_session)
                {
                    return _session.CountAll<ClearanceCode>();
                }
            }
        }

        public ClearanceCode Find(Guid uid) =>
            throw new NotImplementedException();

        public void Remove(int id, out bool isDeleted)
        {
            lock (_session)
            {
                isDeleted = false;
                var item = Find(id);
                if (item != null)
                {
                    _session.Delete<ClearanceCode>(item);
                    isDeleted = true;
                }
            }
        }

        public void SaveChanges()
        {
            _session.SaveChanges();
        }

        public void Remove(Guid uid) =>
            throw new NotImplementedException();

        public IEnumerable<ClearanceCode> FindByExternal(string externalRef)
        {
            lock (_session)
            {
                return _session.GetAll<ClearanceCode>(c => c.Code.Equals(externalRef, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }

        public IEnumerable<ClearanceCode> FindByExternal(List<string> externalRefs)
        {
            lock (_session)
            {
                return _session.GetAll<ClearanceCode>(c => c.Code.In(externalRefs)).ToList();
            }
        }

        public int Count(Expression<Func<ClearanceCode, bool>> query)
        {
            lock (_session)
            {
                return _session.Query<ClearanceCode>().Where(query).Count();
            }
        }

        public void Truncate() =>
            _session.TryDelete("Raven/DocumentsByEntityName", "ClearanceCodes");
    }
}

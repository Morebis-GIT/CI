using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenDemographicRepository : IDemographicRepository
    {
        private readonly IDocumentSession _session;

        public RavenDemographicRepository(IDocumentSession session)
        {
            _session = session;
        }

        public void Add(IEnumerable<Demographic> items)
        {
            StoreRange(items);
        }

        public void Add(Demographic item)
        {
            lock (_session)
            {
                _session.Store(item);
            }
        }

        public Demographic GetByExternalRef(string externalRef)
        {
            lock (_session)
            {
                var item = _session.Query<Demographic>().FirstOrDefault(_ => _.ExternalRef == externalRef);
                return item;
            }
        }

        public IEnumerable<Demographic> GetByExternalRef(List<string> externalRefs)
        {
            var items = _session.GetAll<Demographic>(d => d.ExternalRef.In(externalRefs),
                indexName: Demographic_BySearch.DefaultIndexName, isMapReduce: false);
            return items.ToList();
        }

        public Demographic GetById(int id)
        {
            lock (_session)
            {
                var item = _session.Load<Demographic>(id);
                return item;
            }
        }

        public void Delete(int id)
        {
            lock (_session)
            {
                _session.Delete<Demographic>(id);
            }
        }

        public IEnumerable<Demographic> GetAll()
        {
            lock (_session)
            {
                return _session.GetAll<Demographic>().ToList();
            }
        }

        public List<string> GetAllGameplanDemographics()
        {
            var demographics = new List<string>();
            lock (_session)
            {
                demographics.AddRange(
                    GetAll()
                        .Where(demo => demo.Gameplan)
                        .Select(d => d.ExternalRef)
                );
            }
            return demographics;
        }

        public int CountAll
        {
            get
            {
                lock (_session)
                {
                    return _session.CountAll<Demographic>();
                }
            }
        }

        public void Update(Demographic demographic)
        {
            lock (_session)
            {
                _session.Store(demographic);
            }
        }

        public void UpdateRange(IEnumerable<Demographic> demographics)
        {
            StoreRange(demographics);
        }

        public void Truncate() =>
            _session.TryDelete("Raven/DocumentsByEntityName", "Demographics");

        public void DeleteRangeByExternalRefs(IEnumerable<string> externalRefs)
        {
            lock (_session)
            {
                var demographics = _session.GetAll<Demographic>(s => s.ExternalRef.In(externalRefs.ToList()));
                foreach (var demographic in demographics)
                {
                    _session.Delete(demographic);
                }
            }
        }

        public void InsertOrUpdate(IEnumerable<Demographic> items)
        {
            lock (_session)
            {
                var options = new BulkInsertOptions() { OverwriteExisting = true };
                using (_session.Advanced.DocumentStore.BulkInsert(null, options))
                {
                    items.ToList().ForEach(item => _session.Store(item));
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

        private void StoreRange(IEnumerable<Demographic> items)
        {
            items.ToList().ForEach(item => _session.Store(item));
        }
    }
}

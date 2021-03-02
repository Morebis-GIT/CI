using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenAutoBookRepository : IAutoBookRepository
    {
        private readonly IDocumentSession _session;

        const string collectionName = "AutoBooks";

        public RavenAutoBookRepository(IDocumentSession session)
            => _session = session;


        public AutoBook Get(string id)
        {
            lock (_session)
            {
                if (!id.Contains(collectionName))
                {
                    id = $"{collectionName}/{id}";
                }
                return _session.Load<AutoBook>(id);
            }
        }

        public IEnumerable<AutoBook> GetAll()
        {
            lock (_session)
            {
                return _session
                    .Query<AutoBook>()
                    .Take(int.MaxValue)
                    .ToList();
            }
        }

        public int CountAll => _session.CountAll<AutoBook>();

        public void Add(AutoBook autoBook)
        {
            lock (_session)
            {
                var autoBookId = autoBook.Id;
                if (!autoBookId.Contains(collectionName))
                {
                    autoBookId = $"{collectionName}/{autoBook.Id}";
                }
                _session.Store(autoBook, autoBookId);
            }
        }

        public void Update(AutoBook autoBook)
        {
            lock (_session)
            {
                _session.Store(autoBook);
            }
        }

        public void Delete(string id)
        {
            lock (_session)
            {
                var autobook = _session.Load<AutoBook>(id);
                if (autobook != null)
                {
                    _session.Delete(autobook);
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
    }
}

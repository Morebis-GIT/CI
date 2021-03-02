using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.TotalRatings;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Raven.Client.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenTotalRatingRepository : ITotalRatingRepository
    {
        private readonly IDocumentSession _session;

        public RavenTotalRatingRepository(IDocumentSession session)
        {
            _session = session;
        }

        public IEnumerable<TotalRating> GetAll() => _session.GetAll<TotalRating>();

        public TotalRating Get(int id) => _session.Load<TotalRating>(id);

        public void AddRange(IEnumerable<TotalRating> totalRatings)
        {
            lock (_session)
            {
                var options = new BulkInsertOptions {OverwriteExisting = true};
                using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert(null, options))
                {
                    totalRatings.ForEach(item => bulkInsert.Store(item));
                }
            }
        }

        public IEnumerable<TotalRating> Search(string salesArea, DateTime startDate, DateTime endDate) =>
            _session.GetAll<TotalRating>(c => c.SalesArea == salesArea && c.Date >= startDate && c.Date <= endDate);

        public IEnumerable<TotalRating> SearchByMonths(DateTime startDate, DateTime endDate) =>
            _session.GetAll<TotalRating>(c => c.Date.Month >= startDate.Month && c.Date.Month <= endDate.Month);

        public void DeleteRange(IEnumerable<int> ids)
        {
            lock (_session)
            {
                var totalRatings = _session.GetAll<TotalRating>(s => s.Id.In(ids.ToList()));
                foreach (var rating in totalRatings)
                {
                    _session.Delete(rating);
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

        public void Truncate() => _session.TryDelete("Raven/DocumentsByEntityName", "TotalRatings");
    }
}

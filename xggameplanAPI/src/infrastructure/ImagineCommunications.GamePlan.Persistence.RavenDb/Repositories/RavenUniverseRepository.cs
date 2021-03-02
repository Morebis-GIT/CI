using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenUniverseRepository : IUniverseRepository
    {
        private readonly IDocumentSession _session;

        public RavenUniverseRepository(IDocumentSession session)
        {
            _session = session;
        }

        public IEnumerable<Universe> GetAll() =>
            _session.GetAll<Universe>();

        public IEnumerable<Universe> GetBySalesAreaDemo(string salesarea, string demographic) =>
            _session.GetAll<Universe>(s => s.SalesArea == salesarea && s.Demographic == demographic)
                .OrderByDescending(s => s.EndDate).ToList();

        public IEnumerable<Universe> Search(List<string> demographics, List<string> salesAreas, DateTime startDate, DateTime endDate) =>
            //Demo code and / or SalesArea and / or DateRange(if no parameters included, return ALL)
            _session.GetAll<Universe>().Where(
                u => (salesAreas == null || salesAreas.Count == 0 || u.SalesArea.In(salesAreas)) &&
                (demographics == null || demographics.Count == 0 || u.Demographic.In(demographics)) &&
                (startDate.Date == DateTime.MinValue || u.StartDate.Date >= startDate.Date) &&
                (endDate.Date == DateTime.MinValue || u.EndDate.Date < endDate.Date.AddDays(1)));

        public Universe Find(Guid id) =>
            _session.Load<Universe>(id);

        public void Insert(IEnumerable<Universe> universes)
        {
            lock (_session)
            {
                var options = new BulkInsertOptions() { OverwriteExisting = true };
                using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert(null, options))
                {
                    universes.ToList().ForEach(item =>

                        bulkInsert.Store(item));
                }
            }
        }

        public void Update(Universe universe) =>
            _session.Store(universe);

        public void Remove(Guid id) =>
            _session.Delete<Universe>(id);

        public void Truncate() =>
            _session.TryDelete("Raven/DocumentsByEntityName", "Universes");

        public void DeleteByCombination(string salesArea,
            string demographic,
            DateTime? startDate,
            DateTime? endDate)
        {
            var startDateString = startDate?.ToString("yyyy-MM-ddTHH:mm:ss.0000000Z");
            var endDateString = endDate?.ToString("yyyy-MM-ddTHH:mm:ss.0000000Z");

            var query = (salesArea != null)
                            ? QuerySalesArea(salesArea)
                            : null;

            query = (demographic != null)
                        ? ((query != null)
                                ? (query + " AND " + QueryDemographic(demographic))
                                : QueryDemographic(demographic))
                        : query;

            query = (startDate != null && endDate != null)
                        ? ((query != null)
                                ? (query + " AND " + QueryStartAndEndDate(startDateString, endDateString))
                                : QueryStartAndEndDate(startDateString, endDateString))
                        : query;

            lock (_session)
            {
                _session.Advanced.DocumentStore.DatabaseCommands.DeleteByIndex(Universes_Default.DefaultIndexName,
                    new IndexQuery()
                    {
                        Query = query
                    }).WaitForCompletion();
                _session.SaveChanges();
            }
        }

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }

        private string QuerySalesArea(string salesArea) =>
            $"SalesArea:{salesArea}";

        private string QueryDemographic(string demographic) =>
            $"Demographic:{demographic}";

        private string QueryStartAndEndDate(string startDateString, string endDateString) =>
            $"StartDate:[{startDateString} TO {endDateString}] AND EndDate:[{startDateString} TO {endDateString}]";
    }
}

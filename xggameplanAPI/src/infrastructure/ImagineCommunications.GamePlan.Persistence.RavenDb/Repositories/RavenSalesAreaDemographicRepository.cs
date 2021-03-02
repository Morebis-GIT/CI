using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreaDemographics;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenSalesAreaDemographicRepository : ISalesAreaDemographicRepository
    {
        private readonly IDocumentSession _session;

        public RavenSalesAreaDemographicRepository(IDocumentSession session)
        {
            _session = session;
        }

        public void AddRange(IEnumerable<SalesAreaDemographic> entities)
        {
            lock (_session)
            {
                var options = new BulkInsertOptions {OverwriteExisting = true};
                using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert(null, options))
                {
                    entities.ToList().ForEach(entity => bulkInsert.Store(entity));
                }
            }
        }

        public void DeleteBySalesAreaName(string name)
        {
            lock (_session)
            {
                var salesAreaDemographics = _session.GetAll<SalesAreaDemographic>(x => x.SalesArea == name);

                foreach (var salesAreaDemographic in salesAreaDemographics)
                {
                    _session.Delete<SalesAreaDemographic>(salesAreaDemographic.Id);
                }
            }
        }

        public void DeleteBySalesAreaNames(IEnumerable<string> salesAreaNames)
        {
            lock (_session)
            {
                var salesAreaDemographics = _session.GetAll<SalesAreaDemographic>(s => s.SalesArea.In(salesAreaNames.ToList()));

                foreach (var salesAreaDemographic in salesAreaDemographics)
                {
                    _session.Delete<SalesAreaDemographic>(salesAreaDemographic.Id);
                }
            }
        }

        public IEnumerable<SalesAreaDemographic> GetAll() => _session.GetAll<SalesAreaDemographic>();

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }
    }
}

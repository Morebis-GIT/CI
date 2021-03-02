using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.SpotPlacements;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;
using xggameplan.common.Utilities;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenSpotPlacementRepository : ISpotPlacementRepository
    {
        private readonly IDocumentSession _session;

        public RavenSpotPlacementRepository(IDocumentSession session) => _session = session;

        public void Add(SpotPlacement item) => _session.Store(item);


        public void Insert(IEnumerable<SpotPlacement> spotPlacements)
        {
            var options = new BulkInsertOptions() { OverwriteExisting = true };
            using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert(null, options))
            {
                spotPlacements
                    .ToList()
                    .ForEach(item => bulkInsert.Store(item));
            }
        }

        public SpotPlacement GetByExternalSpotRef(string externalSpotRef)
        {
            lock (_session)
            {
                return _session.GetAll<SpotPlacement>(sp => sp.ExternalSpotRef == externalSpotRef,
                       indexName: SpotPlacements_Default.DefaultIndexName,
                       isMapReduce: false
                    ).FirstOrDefault();
            }
        }

        public List<SpotPlacement> GetByExternalSpotRefs(IEnumerable<string> externalSpotRefs)
        {
            lock (_session)
            {
                var spotPlacements = new List<SpotPlacement>();
                int page = 0;

                do
                {
                    // Limit is actually 1024
                    var pageExternalSpotRefs = ListUtilities.GetPageItems(externalSpotRefs.ToList(), 1000, page++);

                    if (pageExternalSpotRefs.Any())
                    {
                        var spotPlacementsPage = _session.GetAll<SpotPlacement>(sp => sp.ExternalSpotRef.In(pageExternalSpotRefs),
                                          indexName: SpotPlacements_Default.DefaultIndexName, isMapReduce: false);
                        spotPlacements.AddRange(spotPlacementsPage);
                    }
                    else
                    {
                        return spotPlacements.OrderBy(sp => sp.ExternalSpotRef).ToList();
                    }
                } while (true);
            }
        }

        public void Update(SpotPlacement spotPlacement)
        {
            lock (_session)
            {
                _session.Store(spotPlacement);
            }
        }

        public void Delete(int id)
        {
            lock (_session)
            {
                _session.Delete<SpotPlacement>(id);
            }
        }

        public void Delete(string externalSpotRef)
        {
            lock (_session)
            {
                var spotPlacement = GetByExternalSpotRef(externalSpotRef);
                if (spotPlacement != null)
                {
                    _session.Delete<SpotPlacement>(spotPlacement.Id);
                }
            }
        }

        public void DeleteBefore(DateTime modifiedTime)
        {
            lock (_session)
            {
                _session.Advanced.DocumentStore.DatabaseCommands.DeleteByIndex(
                    SpotPlacements_Default.DefaultIndexName,
                   new IndexQuery()
                   {
                       Query = string.Format("ModifiedTime:[{0} TO {1}]", modifiedTime.AddYears(-100).ToString("yyyy-MM-ddTHH:mm:ss.0000000"), modifiedTime.ToString("yyyy-MM-ddTHH:mm:ss.0000000"))
                   })
                   .WaitForCompletion();

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
    }
}

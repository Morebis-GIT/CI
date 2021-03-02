using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Optimizer.Facilities;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenFacilityRepository : IFacilityRepository, IDisposable
    {
        private readonly IDocumentSession _session;

        public RavenFacilityRepository(IDocumentSession session)
        {
            _session = session;
        }

        public void Add(Facility facility)
        {
            lock (_session)
            {
                _session.Store(facility);
            }
        }

        public void Delete(int id)
        {
            lock (_session)
            {
                var facility = Get(id);
                if (facility is null)
                {
                    return;
                }

                _session.Delete(facility);
            }
        }

        public Facility Get(int id) => _session.Load<Facility>(id);

        public Facility GetByCode(string code) => _session.Query<Facility>().FirstOrDefault(x => x.Code == code);

        public IEnumerable<Facility> GetAll() => _session.GetAll<Facility>();

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }

        public void Update(Facility facility)
        {
            lock (_session)
            {
                _session.Store(facility);
            }
        }

        public void Dispose()
        {
            _session?.Dispose();
        }
    }
}

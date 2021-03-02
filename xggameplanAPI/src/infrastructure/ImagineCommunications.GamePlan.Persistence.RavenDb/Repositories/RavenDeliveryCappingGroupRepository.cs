using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.DeliveryCappingGroup;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;
using Raven.Client.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenDeliveryCappingGroupRepository: IDeliveryCappingGroupRepository, IDisposable
    {
        private readonly IDocumentSession _session;

        public RavenDeliveryCappingGroupRepository(IDocumentSession session)
        {
            _session = session;
        }

        public void Add(DeliveryCappingGroup item)
        {
            lock (_session)
            {
                _session.Store(item);
            }
        }

        public void Update(DeliveryCappingGroup item)
        {
            lock (_session)
            {
                _session.Store(item);
            }
        }

        public void Delete(int id)
        {
            lock (_session)
            {
                var item = Get(id);
                if (item is null)
                {
                    return;
                }

                _session.Delete(item);
            }
        }

        public DeliveryCappingGroup Get(int id) => _session.Load<DeliveryCappingGroup>(id);

        public DeliveryCappingGroup GetByDescription(string description) => _session
            .Query<DeliveryCappingGroup>()
            .FirstOrDefault(x => x.Description == description);

        public IEnumerable<DeliveryCappingGroup> Get(IEnumerable<int> ids) =>
            _session.GetAll<DeliveryCappingGroup>(x => x.Id.In(ids));

        public IEnumerable<DeliveryCappingGroup> GetAll() => _session.GetAll<DeliveryCappingGroup>();

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }

        public void Dispose()
        {
            _session?.Dispose();
        }
    }
}

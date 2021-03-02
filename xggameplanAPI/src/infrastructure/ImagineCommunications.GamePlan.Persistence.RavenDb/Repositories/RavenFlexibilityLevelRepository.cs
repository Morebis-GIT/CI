using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Autopilot.FlexibilityLevels;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenFlexibilityLevelRepository : IFlexibilityLevelRepository, IDisposable
    {
        private readonly IDocumentSession _session;

        public RavenFlexibilityLevelRepository(IDocumentSession session)
        {
            _session = session;
        }

        public void Add(FlexibilityLevel flexibilityLevel)
        {
            lock (_session)
            {
                _session.Store(flexibilityLevel);
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

        public FlexibilityLevel Get(int id) => _session.Load<FlexibilityLevel>(id);

        public IEnumerable<FlexibilityLevel> GetAll() => _session.GetAll<FlexibilityLevel>();

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }

        public void Update(FlexibilityLevel flexibilityLevel)
        {
            lock (_session)
            {
                _session.Store(flexibilityLevel);
            }
        }

        public void Dispose()
        {
            _session?.Dispose();
        }
    }
}

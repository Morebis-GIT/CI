using System;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures.Objects;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenFailuresRepository : IFailuresRepository
    {
        private readonly IDocumentSession _session;

        public RavenFailuresRepository(IDocumentSession session)
            => _session = session;

        public void Add(Failures failures)
        {
            lock (_session)
            {
                _session.Store(failures);
            }
        }

        public Failures Get(Guid scenarioId)
        {
            lock (_session)
            {
                Failures failures = _session.Load<Failures>(scenarioId);
                return failures;
            }
        }

        public void Delete(Guid scenarioId)
        {
            lock (_session)
            {
                // If you delete by Id then Raven errors "Can't delete changed entity by Id,
                // try Delete<T>(id)". This has recently started happening. It doesn't happen with
                // other documents. The entity hasn't 'changed'.
                var failures = _session.Load<Failures>(scenarioId);
                if (failures != null)
                {
                    _session.Delete(failures);
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

        [Obsolete("Use the Add() method.")]
        public void Insert(Failures failures) => Add(failures);

        [Obsolete("Use the Delete() method.")]
        public void Remove(Guid scenarioId) => Delete(scenarioId);
    }
}

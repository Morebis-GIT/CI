using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;
using xggameplan.AuditEvents;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenPipelineAuditEventRepository : IPipelineAuditEventRepository
    {
        private readonly IDocumentSession _session;

        public RavenPipelineAuditEventRepository(IDocumentSession session)
        {
            _session = session;
        }

        public PipelineAuditEvent Get(int Id)
        {
            return _session.Load<PipelineAuditEvent>(Id);
        }

        public void Add(PipelineAuditEvent item)
        {
            lock (_session)
            {
                _session.Store(item);
            }
        }

        public IEnumerable<PipelineAuditEvent> GetAll()
        {
            lock (_session)
            {
                return _session.GetAll<PipelineAuditEvent>();
            }
        }

        public IEnumerable<PipelineAuditEvent> GetAllByRunId(Guid runId)
        {
            lock (_session)
            {
                var allPipelineEventsWithRunId = _session.GetAll<PipelineAuditEvent>().Where(p => p.RunId == runId);
                return allPipelineEventsWithRunId;
            }
        }

        public IEnumerable<PipelineAuditEvent> GetAllByScenarioId(Guid scenarioId)
        {
            lock (_session)
            {
                var allPipelineEventsWithScenarioId = _session.GetAll<PipelineAuditEvent>().Where(p => p.ScenarioId == scenarioId);
                return allPipelineEventsWithScenarioId;
            }
        }

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }

        public void DeleteRange(IEnumerable<PipelineAuditEvent> pipelineItems)
        {
            if (pipelineItems == null || !pipelineItems.Any())
            {
                return;
            }

            lock (_session)
            {
                foreach (PipelineAuditEvent p in pipelineItems)
                {
                    Delete(p.Id);
                }
            }
        }

        public void Delete(int id)
        {
            lock (_session)
            {
                var item = Get(id);
                if (item != null)
                {
                    _session.Delete(item);
                }
            }
        }
    }
}

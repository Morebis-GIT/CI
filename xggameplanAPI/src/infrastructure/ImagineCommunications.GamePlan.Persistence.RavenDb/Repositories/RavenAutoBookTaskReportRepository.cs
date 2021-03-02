using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;
using xggameplan.AutoBooks;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenAutoBookTaskReportRepository : IAutoBookTaskReportRepository
    {
        private readonly IDocumentSession _session;

        public RavenAutoBookTaskReportRepository(IDocumentSession session)
        {
            _session = session;
        }

        public AutoBookTaskReport Get(int Id)
        {
            return _session.Load<AutoBookTaskReport>(Id);
        }

        public void Add(AutoBookTaskReport item)
        {
            lock (_session)
            {
                _session.Store(item);
            }
        }

        public IEnumerable<AutoBookTaskReport> GetAll()
        {
            lock (_session)
            {
                return _session.GetAll<AutoBookTaskReport>();
            }
        }

        public IEnumerable<AutoBookTaskReport> GetAllByRunId(Guid runId)
        {
            lock (_session)
            {
                var allAutoBookTaskReportsWithRunId = _session.GetAll<AutoBookTaskReport>().Where(p => p.RunId == runId);
                return allAutoBookTaskReportsWithRunId;
            }
        }

        public AutoBookTaskReport GetByScenarioId(Guid scenarioId)
        {
            lock (_session)
            {
                var allAutoBookTaskReports = GetAll();

                var thisAutoBookTaskReport = allAutoBookTaskReports.Where(a => a.ScenarioId == scenarioId).FirstOrDefault();

                return thisAutoBookTaskReport;
            }
        }

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
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

        public void DeleteRange(IEnumerable<AutoBookTaskReport> autoBookTaskReports)
        {
            if (autoBookTaskReports == null || !autoBookTaskReports.Any())
            {
                return;
            }

            lock (_session)
            {
                foreach (AutoBookTaskReport a in autoBookTaskReports)
                {
                    Delete(a.Id);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;

namespace xggameplan.AuditEvents
{
    public interface IPipelineAuditEventRepository
    {
        PipelineAuditEvent Get(int Id);

        void Add(PipelineAuditEvent item);

        IEnumerable<PipelineAuditEvent> GetAll();

        IEnumerable<PipelineAuditEvent> GetAllByRunId(Guid runId);

        IEnumerable<PipelineAuditEvent> GetAllByScenarioId(Guid scenarioId);

        void DeleteRange(IEnumerable<PipelineAuditEvent> pipelineEvents);

        void Delete(int id);

        void SaveChanges();
    }
}

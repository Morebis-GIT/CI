using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using xggameplan.AuditEvents;
using SqlPipelineEvents = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.PipelineAuditEvents;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class PipelineAuditEventRepository : IPipelineAuditEventRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public PipelineAuditEventRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public PipelineAuditEvent Get(int Id) =>
            _mapper.Map<PipelineAuditEvent>(
                _dbContext.Find<SqlPipelineEvents.PipelineAuditEvent>(Id));

        public IEnumerable<PipelineAuditEvent> GetAll() =>
            _mapper.Map<List<PipelineAuditEvent>>(
                _dbContext.Query<SqlPipelineEvents.PipelineAuditEvent>()
                .ToArray());

        public IEnumerable<PipelineAuditEvent> GetAllByRunId(Guid runId) =>
            _mapper.Map<List<PipelineAuditEvent>>(
                _dbContext.Query<SqlPipelineEvents.PipelineAuditEvent>()
                .Where(x => x.RunId == runId)
                .ToArray());

        public IEnumerable<PipelineAuditEvent> GetAllByScenarioId(Guid scenarioId) =>
            _mapper.Map<List<PipelineAuditEvent>>(
                _dbContext.Query<SqlPipelineEvents.PipelineAuditEvent>()
                .Where(x => x.ScenarioId == scenarioId)
                .ToArray());

        public void Add(PipelineAuditEvent item)
        {
            if (item != null)
            {
                _dbContext.Add(_mapper.Map<SqlPipelineEvents.PipelineAuditEvent>(item));
            }
        }

        public void Delete(int id)
        {
            var entity = _dbContext.Find<SqlPipelineEvents.PipelineAuditEvent>(id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public void DeleteRange(IEnumerable<PipelineAuditEvent> items)
        {
            var ids = items.Select(a => a.Id);
            var toRemove = _dbContext
                .Query<SqlPipelineEvents.PipelineAuditEvent>()
                .Where(x => ids.Contains(x.Id))
                .ToArray();
            _dbContext.RemoveRange(toRemove);
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}

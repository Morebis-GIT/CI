using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using xggameplan.AutoBooks;
using SqlAutoBookTaskReports = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookTaskReports;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class AutoBookTaskReportRepository : IAutoBookTaskReportRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public AutoBookTaskReportRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public AutoBookTaskReport Get(int Id) => _mapper.Map<AutoBookTaskReport>(
                _dbContext.Find<SqlAutoBookTaskReports.AutoBookTaskReport>(Id));

        public IEnumerable<AutoBookTaskReport> GetAll() =>
            _mapper.Map<List<AutoBookTaskReport>>(
                _dbContext.Query<SqlAutoBookTaskReports.AutoBookTaskReport>()
                .ToArray());

        public IEnumerable<AutoBookTaskReport> GetAllByRunId(Guid runId) =>
            _mapper.Map<List<AutoBookTaskReport>>(
                _dbContext.Query<SqlAutoBookTaskReports.AutoBookTaskReport>()
                .Where(x => x.RunId == runId)
                .ToArray());

        public AutoBookTaskReport GetByScenarioId(Guid scenarioId) =>
            _mapper.Map<AutoBookTaskReport>(
                _dbContext.Query<SqlAutoBookTaskReports.AutoBookTaskReport>()
                .Where(x => x.ScenarioId == scenarioId)
                .FirstOrDefault());

        public void Add(AutoBookTaskReport item)
        {
            if (item != null)
            {
                _dbContext.Add(_mapper.Map<SqlAutoBookTaskReports.AutoBookTaskReport>(item));
            }
        }

        public void Delete(int id)
        {
            var entity = _dbContext.Find<SqlAutoBookTaskReports.AutoBookTaskReport>(id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public void DeleteRange(IEnumerable<AutoBookTaskReport> autobookTaskReports)
        {
            var ids = autobookTaskReports.Select(a => a.Id);
            var toRemove = _dbContext
                .Query<SqlAutoBookTaskReports.AutoBookTaskReport>()
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

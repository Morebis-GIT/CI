using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class FailuresRepository : IFailuresRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public FailuresRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Add(Failures failures)
        {
            var mappedFailure = _mapper.Map<Entities.Tenant.Failures.Failure>(failures);
            var mappedFailures = new List<Entities.Tenant.Failures.Failure>() { mappedFailure };

            using var transaction = _dbContext.Specific.Database.BeginTransaction();

            _dbContext.BulkInsertEngine.BulkInsert(mappedFailures, new BulkInsertOptions { PreserveInsertOrder = true, SetOutputIdentity = true, BulkCopyTimeout = 300 });

            mappedFailure.Items.ForEach(c => c.FailureId = mappedFailure.Id);

            _dbContext.BulkInsertEngine.BulkInsert(mappedFailure.Items, new BulkInsertOptions { BulkCopyTimeout = 300 });

            transaction.Commit();
        }

        public void Delete(Guid scenarioId)
        {
            var entity = _dbContext.Query<Entities.Tenant.Failures.Failure>()
                .FirstOrDefault(e => e.ScenarioId == scenarioId);

            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public Failures Get(Guid scenarioId) =>
            _dbContext.Query<Entities.Tenant.Failures.Failure>()
                .ProjectTo<Failures>(_mapper.ConfigurationProvider)
                .FirstOrDefault(e => e.Id == scenarioId);

        [Obsolete("Use the Add() method.")]
        public void Insert(Failures failures) => Add(failures);

        [Obsolete("Use the Delete() method.")]
        public void Remove(Guid scenarioId) => Delete(scenarioId);

        public void SaveChanges() => _dbContext.SaveChanges();
    }
}

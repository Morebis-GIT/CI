using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.PostProcessing.Builders;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth;
using SmoothFailure = ImagineCommunications.GamePlan.Domain.SmoothFailures.SmoothFailure;
using SmoothFailureEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.SmoothFailure;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class SmoothFailureRepository : ISmoothFailureRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public SmoothFailureRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void AddRange(IEnumerable<SmoothFailure> items)
        {
            using (var transaction = _dbContext.Specific.Database.BeginTransaction())
            {
                var smoothFailureEntities = _mapper.Map<List<SmoothFailureEntity>>(items);
                var ids = smoothFailureEntities.Where(x => x.Id > 0).Select(x => x.Id).Distinct().ToList();

                if (ids.Any())
                {
                    var relatedSmoothFailureSmoothFailureMessagesIds = _dbContext
                        .Query<SmoothFailureSmoothFailureMessage>()
                        .Where(x => ids.Contains(x.SmoothFailureId))
                        .Select(x => x.Id)
                        .ToArray();

                    if (relatedSmoothFailureSmoothFailureMessagesIds.Any())
                    {
                        _dbContext.Specific.RemoveByIdentityIds<SmoothFailureSmoothFailureMessage>(
                            relatedSmoothFailureSmoothFailureMessagesIds);
                    }
                }

                //Here InsertOrUpdate is used depending on Raven bulk insert option OverwriteExisting = true
                _dbContext.BulkInsertEngine.BulkInsertOrUpdate(
                    smoothFailureEntities, new BulkInsertOptions { PreserveInsertOrder = true, SetOutputIdentity = true });

                var newFailureMessagesList = smoothFailureEntities.SelectMany(x => x.FailureMessagesMap.Select(m =>
                {
                    m.SmoothFailureId = x.Id;
                    return m;
                })).ToList();

                _dbContext.BulkInsertEngine.BulkInsert(newFailureMessagesList,
                    new BulkInsertOptions { PreserveInsertOrder = true, SetOutputIdentity = true });

                transaction.Commit();

                var actionBuilder = new BulkInsertActionBuilder<SmoothFailureEntity>(smoothFailureEntities, _mapper);
                actionBuilder.TryToUpdate(items);
                actionBuilder.Build()?.Execute();
            }
        }

        public IEnumerable<SmoothFailure> GetByRunId(Guid runId)
        {
            return _dbContext.Query<SmoothFailureEntity>()
                .Where(x => x.RunId == runId)
                .OrderBy(x => x.SalesArea)
                .ThenBy(x => x.TypeId)
                .ThenBy(x => x.ExternalSpotRef)
                .ThenBy(x => x.ExternalBreakRef)
                .ProjectTo<SmoothFailure>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public void RemoveByRunId(Guid runId)
        {
            var smoothFailureIds = _dbContext.Query<SmoothFailureEntity>()
                .Where(x => x.RunId == runId)
                .Select(x => x.Id)
                .ToArray();

            _dbContext.Specific.RemoveByIdentityIds<SmoothFailureEntity>(smoothFailureIds);
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        public void Truncate() => throw new NotImplementedException();
    }
}

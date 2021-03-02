using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Extensions.AutoMapper;
using BreakEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Breaks.Break;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class BreakRepository : IBreakRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;
        private readonly IMapper _mapper;

        public BreakRepository(
            ISqlServerTenantDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
            _mapper = mapper;
        }

        public int CountAll => _dbContext.Query<BreakEntity>().Count();

        public void Add(Break @break)
        {
            if (string.IsNullOrWhiteSpace(@break.ExternalBreakRef))
            {
                throw new RepositoryException("Unable to create a break without an ExternalBreakRef.");
            }

            var breakEntity = _dbContext.Query<BreakEntity>()
                .FirstOrDefault(b => b.ExternalBreakRef == @break.ExternalBreakRef);

            if (breakEntity is null)
            {
                breakEntity = _mapper.Map<BreakEntity>(@break, opts => opts.UseEntityCache(_salesAreaByNameCache));
                _ = _dbContext.Add(breakEntity,
                    post => post.MapTo(@break, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);
            }
            else
            {
                @break.Id = breakEntity.Id;
                _ = _mapper.Map(@break, breakEntity, opts => opts.UseEntityCache(_salesAreaByNameCache));
                _ = _dbContext.Update(breakEntity,
                    post => post.MapTo(@break, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);
            }
        }

        public void Add(IEnumerable<Break> breaks)
        {
            int batchSize = 1000;
            int offset = 0;
            int totalBreaks = breaks.Count();

            var batch = breaks.Skip(offset).Take(batchSize);
            while (batch.Any())
            {
                var externalRefs = batch.Select(b => b.ExternalBreakRef).ToArray();

                var existingBreaksByRefs = _dbContext.Query<BreakEntity>()
                    .Where(b => externalRefs.Contains(b.ExternalBreakRef))
                    .ToDictionary(b => b.ExternalBreakRef, b => b);

                var newEntities = new List<BreakEntity>();
                var updatedEntities = new List<BreakEntity>();

                foreach (var @break in batch)
                {
                    if (existingBreaksByRefs.ContainsKey(@break.ExternalBreakRef))
                    {
                        var entity = existingBreaksByRefs[@break.ExternalBreakRef];
                        @break.Id = entity.Id;
                        _ = _mapper.Map(@break, entity, opts => opts.UseEntityCache(_salesAreaByNameCache));
                        updatedEntities.Add(entity);
                    }
                    else
                    {
                        var entity = _mapper.Map<BreakEntity>(@break, opts => opts.UseEntityCache(_salesAreaByNameCache));
                        newEntities.Add(entity);
                    }
                }

                if (newEntities.Any())
                {
                    _dbContext.BulkInsertEngine.BulkInsert(newEntities);
                }

                if (updatedEntities.Any())
                {
                    _dbContext.BulkInsertEngine.BulkUpdate(updatedEntities);
                }
                offset = offset + batchSize;
                batch = breaks.Skip(offset).Take(batchSize);
            }
        }

        public int Count() => _dbContext.Query<BreakEntity>().Count();

        public int Count(Expression<Func<Break, bool>> query) =>
            _dbContext.Query<BreakEntity>()
                .ProjectTo<Break>(_mapper.ConfigurationProvider)
                    .Count(query);

        public void Delete(Guid id)
        {
            var entity = _dbContext.Find<BreakEntity>(id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public void Delete(IEnumerable<Guid> ids)
        {
            var breaks = _dbContext.Query<BreakEntity>()
                .Where(x => ids.Contains(x.Id))
                .ToArray();

            _dbContext.RemoveRange(breaks);
        }

        public Break Find(Guid uid) => Get(uid);

        public IEnumerable<Break> FindByExternal(string externalRef) =>
            _mapper.Map<List<Break>>(
                BreakQuery()
                    .Where(x => x.ExternalBreakRef == externalRef), opts => opts.UseEntityCache(_salesAreaByIdCache));

        public IEnumerable<Break> FindByExternal(List<string> externalRefs) =>
            _mapper.Map<List<Break>>(BreakQuery()
                .Where(x => externalRefs.Contains(x.ExternalBreakRef))
                .AsNoTracking(), opts => opts.UseEntityCache(_salesAreaByIdCache));

        public Break Get(Guid id) =>
            _mapper.Map<Break>(_dbContext.Find<Entities.Tenant.Breaks.Break>(new object[] { id },
                    find => find.IncludeReference(x => x.SalesArea).IncludeCollection(x => x.BreakEfficiencies)),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public IEnumerable<Break> GetAll() =>
            _mapper.Map<List<Break>>(BreakQuery().AsNoTracking(), opts => opts.UseEntityCache(_salesAreaByIdCache));

        public void Remove(Guid uid) => Delete(uid);

        public void SaveChanges() =>
            _dbContext.SaveChanges();

        public Task SaveChangesAsync() =>
            _dbContext.SaveChangesAsync();

        public IEnumerable<Break> Search(DateTime dateFrom, DateTime dateTo, string salesArea) =>
            _mapper.Map<List<Break>>(BreakQuery()
                    .Where(x => x.SalesArea.Name == salesArea &&
                                x.ScheduledDate >= dateFrom &&
                                x.ScheduledDate <= dateTo).AsNoTracking(),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public IEnumerable<Break> Search(DateTime scheduledDate, string externalBreakRef, string salesArea) =>
            _mapper.Map<List<Break>>(
                BreakQuery()
                    .Where(x => x.SalesArea.Name == salesArea &&
                                x.ScheduledDate == scheduledDate &&
                                x.ExternalBreakRef == externalBreakRef).AsNoTracking(),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public IEnumerable<Break> Search(DateTimeRange scheduledDatesRange, IEnumerable<string> salesAreaNames) =>
            _mapper.Map<List<Break>>(
                BreakQuery()
                    .Where(x => x.ScheduledDate >= scheduledDatesRange.Start.ToUniversalTime()
                                && x.ScheduledDate <= scheduledDatesRange.End.ToUniversalTime()
                                && salesAreaNames.Contains(x.SalesArea.Name)).AsNoTracking(),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public IEnumerable<Break> SearchByBroadcastDateRange(DateTime dateFrom, DateTime dateTo,
            IEnumerable<string> salesAreaNames) =>
            _mapper.Map<List<Break>>(BreakQuery()
                    .Where(b => b.BroadcastDate >= dateFrom
                                && b.BroadcastDate <= dateTo
                                && salesAreaNames.Contains(b.SalesArea.Name)).AsNoTracking(),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public void Truncate()
        {
            _dbContext.Truncate<BreakEntity>();
        }

        public Task TruncateAsync() => Task.Run(Truncate);

        protected IQueryable<BreakEntity> BreakQuery() =>
            _dbContext.Query<BreakEntity>().Include(x => x.BreakEfficiencies);
    }
}

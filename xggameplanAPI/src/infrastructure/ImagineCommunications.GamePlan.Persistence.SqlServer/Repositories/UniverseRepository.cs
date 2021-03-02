using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Extensions.AutoMapper;
using xggameplan.Extensions;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class UniverseRepository : IUniverseRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;
        private readonly IMapper _mapper;

        public UniverseRepository(
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

        public void DeleteByCombination(string salesArea, string demographic, DateTime? startDate, DateTime? endDate)
        {
            var where = new List<Expression<Func<Entities.Tenant.Universe, bool>>>();
            if (!string.IsNullOrEmpty(salesArea))
            {
                where.Add(x => x.SalesArea.Name == salesArea);
            }

            if (!string.IsNullOrEmpty(demographic))
            {
                where.Add(x => x.Demographic == demographic);
            }

            if (startDate != null && endDate != null)
            {
                where.Add(x => (x.StartDate >= startDate && x.StartDate <= endDate) &&
                               (x.EndDate >= startDate && x.EndDate <= endDate));
            }

            if (where.Any())
            {
                var entities = _dbContext.Query<Entities.Tenant.Universe>().Where(where.AggregateAnd()).ToArray();
                _dbContext.RemoveRange(entities);
            }
        }

        public Universe Find(Guid id) =>
            _mapper.Map<Universe>(_dbContext.Find<Entities.Tenant.Universe>(id),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public IEnumerable<Universe> GetAll() =>
            _mapper.Map<List<Universe>>(UniverseQuery().AsNoTracking(),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public IEnumerable<Universe> GetBySalesAreaDemo(string salesarea, string demographic) =>
            _mapper.Map<List<Universe>>(UniverseQuery()
                .Where(x => x.SalesArea.Name == salesarea && x.Demographic == demographic)
                .OrderByDescending(s => s.EndDate).AsNoTracking(), opts => opts.UseEntityCache(_salesAreaByIdCache));

        public void Insert(IEnumerable<Universe> universes) =>
            _dbContext.BulkInsertEngine.BulkInsertOrUpdate(
                _mapper.Map<List<Entities.Tenant.Universe>>(universes,
                    opts => opts.UseEntityCache(_salesAreaByNameCache)),
                post => post.TryToUpdate(universes, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);

        public void Remove(Guid id)
        {
            var entity = _dbContext.Find<Entities.Tenant.Universe>(id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public IEnumerable<Universe> Search(List<string> demographics, List<string> salesAreas, DateTime startDate,
            DateTime endDate) =>
            _mapper.Map<List<Universe>>(UniverseQuery()
                    .Where(u =>
                        (salesAreas == null || salesAreas.Count == 0 || salesAreas.Contains(u.SalesArea.Name)) &&
                        (demographics == null || demographics.Count == 0 || demographics.Contains(u.Demographic)) &&
                        (startDate.Date == DateTime.MinValue || u.StartDate.Date >= startDate.Date) &&
                        (endDate.Date == DateTime.MinValue || u.EndDate.Date < endDate.Date.AddDays(1))).AsNoTracking(),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public void Truncate() =>
             _dbContext.Truncate<Entities.Tenant.Universe>();

        public void Update(Universe universe)
        {
            var entity = _dbContext.Find<Entities.Tenant.Universe>(
                (universe ?? throw new ArgumentNullException(nameof(universe))).Id);
            if (entity != null)
            {
                _ = _mapper.Map(universe, entity, opts => opts.UseEntityCache(_salesAreaByNameCache));
                _ = _dbContext.Update(entity,
                    post => post.MapTo(universe, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);
            }
        }

        public void SaveChanges() =>
          _dbContext.SaveChanges();

        private IQueryable<Entities.Tenant.Universe> UniverseQuery() => _dbContext.Query<Entities.Tenant.Universe>();
    }
}

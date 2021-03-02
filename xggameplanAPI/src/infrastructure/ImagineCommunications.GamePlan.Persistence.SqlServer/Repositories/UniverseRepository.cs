using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using xggameplan.Extensions;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class UniverseRepository : IUniverseRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public UniverseRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void DeleteByCombination(string salesArea, string demographic, DateTime? startDate, DateTime? endDate)
        {
            var where = new List<Expression<Func<Entities.Tenant.Universe, bool>>>();
            if (!string.IsNullOrEmpty(salesArea))
            {
                where.Add(x => x.SalesArea == salesArea);
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
            _mapper.Map<Universe>(_dbContext.Find<Entities.Tenant.Universe>(id));

        public IEnumerable<Universe> GetAll() =>
            _dbContext.Query<Entities.Tenant.Universe>().ProjectTo<Universe>(_mapper.ConfigurationProvider).ToList();

        public IEnumerable<Universe> GetBySalesAreaDemo(string salesarea, string demographic) =>
            _dbContext.Query<Entities.Tenant.Universe>()
                      .Where(x => x.SalesArea == salesarea && x.Demographic == demographic)
                      .OrderByDescending(s => s.EndDate)
                      .ProjectTo<Universe>(_mapper.ConfigurationProvider).ToList();

        public void Insert(IEnumerable<Universe> universes) =>
            _dbContext.BulkInsertEngine.BulkInsertOrUpdate(_mapper.Map<List<Entities.Tenant.Universe>>(universes),
                post => post.TryToUpdate(universes), _mapper);

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
            _dbContext.Query<Entities.Tenant.Universe>()
                .Where(u => (salesAreas == null || salesAreas.Count == 0 || salesAreas.Contains(u.SalesArea)) &&
                            (demographics == null || demographics.Count == 0 || demographics.Contains(u.Demographic)) &&
                            (startDate.Date == DateTime.MinValue || u.StartDate.Date >= startDate.Date) &&
                            (endDate.Date == DateTime.MinValue || u.EndDate.Date < endDate.Date.AddDays(1)))
                .ProjectTo<Universe>(_mapper.ConfigurationProvider).ToList();

        public void Truncate() =>
             _dbContext.Truncate<Entities.Tenant.Universe>();

        public void Update(Universe universe)
        {
            var entity = _dbContext.Find<Entities.Tenant.Universe>(
                (universe ?? throw new ArgumentNullException(nameof(universe))).Id);
            if (entity != null)
            {
                _mapper.Map(universe, entity);
                _dbContext.Update(entity, post => post.MapTo(universe), _mapper);
            }
        }

        public void SaveChanges() =>
          _dbContext.SaveChanges();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Passes.Queries;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Extensions;
using xggameplan.core.Extensions.AutoMapper;
using PassEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes.Pass;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class PassRepository : IPassRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IFullTextSearchConditionBuilder _searchConditionBuilder;
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;
        private readonly IMapper _mapper;

        public IQueryable<PassEntity> PassQuery =>
            _dbContext.Query<PassEntity>()
            .Include(x => x.General)
            .Include(x => x.Rules)
            .Include(x => x.Tolerances)
            .Include(x => x.Weightings)
            .Include(x => x.BreakExclusions)
            .Include(x => x.RatingPoints)
                .ThenInclude(x => x.SalesAreas)
            .Include(x => x.ProgrammeRepetitions)
            .Include(x => x.SlottingLimits)
            .Include(x => x.PassSalesAreaPriorities)
                .ThenInclude(x => x.SalesAreaPriorities);

        public PassRepository(
            ISqlServerTenantDbContext dbContext,
            IFullTextSearchConditionBuilder searchConditionBuilder,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _searchConditionBuilder = searchConditionBuilder;
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
            _mapper = mapper;
        }

        private IQueryable<PassEntity> GetPassByExpression(Expression<Func<PassEntity, bool>> expression) =>
            PassQuery.Where(expression);

        public Pass Get(int id)
        {
            return _mapper.Map<Pass>(
                PassQuery.FirstOrDefault(x => x.Id == id),
                opt => opt.UseEntityCache(_salesAreaByIdCache));
        }

        public Pass FindByName(string name, bool isLibraried)
        {
            var query = _mapper.Map<List<Pass>>(
                GetPassByExpression(p => p.Name == name),
                opt => opt.UseEntityCache(_salesAreaByIdCache));

            return (isLibraried)
                ? query.FirstOrDefault(p => p.IsLibraried == true)
                : query.FirstOrDefault();
        }

        public IEnumerable<Pass> FindByIds(IEnumerable<int> ids) =>
            _mapper.Map<List<Pass>>(
                GetPassByExpression(p => ids.Contains(p.Id)).AsNoTracking(),
                opt => opt.UseEntityCache(_salesAreaByIdCache));

        public IEnumerable<Pass> GetAll() =>
            _mapper.Map<List<Pass>>(
                PassQuery.AsNoTracking(),
                opt => opt.UseEntityCache(_salesAreaByIdCache));

        public IEnumerable<int> GetLibraryIds() =>
            _dbContext.Query<PassEntity>()
                .Where(x => x.IsLibraried == true)
                .Select(x => x.Id)
                .ToList();

        public IEnumerable<Pass> FindByScenarioId(Guid scenarioId) =>
            _mapper.Map<List<Pass>>(
                _dbContext.Query<Entities.Tenant.Scenarios.ScenarioPassReference>()
                .Where(x => x.ScenarioId == scenarioId)
                .Select(x => x.Pass).AsNoTracking(),
            opt => opt.UseEntityCache(_salesAreaByIdCache));

        public SearchResultModel<PassDigestListItem> MinimalDataSearch(
            SearchQueryDto queryModel,
            bool isLibraried)
        {
            List<PassDigestListItem> allRecords = _dbContext.Query<PassEntity>()
                .Where(p => p.IsLibraried.HasValue && p.IsLibraried.Value == isLibraried)
                .ProjectTo<PassDigestListItem>(_mapper.ConfigurationProvider)
                .ToList();

            if (!string.IsNullOrWhiteSpace(queryModel.Title))
            {
                allRecords.RemoveAll(x => DoesNotContainTitle(queryModel.Title, x.Name));
            }

            IOrderedEnumerable<PassDigestListItem> orderQuery =
                   queryModel.OrderDirection == OrderDirection.Desc
                   ? allRecords.OrderByDescending(OrderByProperty(queryModel.OrderBy))
                   : allRecords.OrderBy(OrderByProperty(queryModel.OrderBy));

            var searchResults = new SearchResultModel<PassDigestListItem>();
            searchResults.Items = orderQuery
                .Skip(queryModel.Skip)
                .Take(queryModel.Top)
                .ToList();
            searchResults.TotalCount = orderQuery.Count();

            return searchResults;

            Func<PassDigestListItem, object> OrderByProperty(OrderBy orderBy)
            {
                if (OrderBy.Date == orderBy)
                {
                    return x => x.DateModified;
                }
                else
                {
                    return x => x.Name;
                }
            }

            bool DoesNotContainTitle(string title, string passDigestName) =>
                passDigestName.IndexOf(title, StringComparison.CurrentCultureIgnoreCase) < 0;
        }

        public PagedQueryResult<Pass> Search(PassSearchQueryModel queryModel, StringMatchRules freeTextMatchRules)
        {
            var query = PassQuery;

            if (queryModel.IsLibraried != null)
            {
                query = queryModel.IsLibraried.Value
                    ? query.Where(e => e.IsLibraried == true)
                    : query.Where(e => e.IsLibraried == null || e.IsLibraried == false);
            }

            if (!string.IsNullOrWhiteSpace(queryModel.Name))
            {
                var searchCondition = _searchConditionBuilder
                    .StartAnyWith(queryModel.Name.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries))
                    .Build();

                query = query.Where(p => EF.Functions.Contains(p.Name, searchCondition));
            }

            query = (queryModel.OrderBy?.Any() ?? false)
                ? query.OrderByMultipleItems(queryModel.OrderBy)
                : query.OrderBy(p => p.Name);

            List<Pass> passList = _mapper.Map<List<Pass>>(
                query.Where(e => freeTextMatchRules.IsMatches(e.Name, queryModel.Name)).AsNoTracking(),
                opt => opt.UseEntityCache(_salesAreaByIdCache));

            return new PagedQueryResult<Pass>(passList.Count,
                passList.ApplyPaging(queryModel.Skip, queryModel.Top).ToList());
        }

        public void Add(Pass pass)
        {
            _ = _dbContext.Add(
                   _mapper.Map<PassEntity>(
                       pass,
                       opt => opt.UseEntityCache(_salesAreaByNameCache)),
                   post => post.MapTo(
                       pass,
                       opt => opt.UseEntityCache(_salesAreaByIdCache)),
                   _mapper);
        }

        public void Add(IEnumerable<Pass> items) =>
            _dbContext.AddRange(
                _mapper.Map<PassEntity[]>(
                    items,
                    opt => opt.UseEntityCache(_salesAreaByNameCache)),
                post => post.MapToCollection(
                    items,
                    opt => opt.UseEntityCache(_salesAreaByIdCache)),
                _mapper);

        public void Delete(int id)
        {
            _dbContext.Remove(_dbContext.Find<PassEntity>(id));
        }

        public void Remove(IEnumerable<int> ids)
        {
            var entities = PassQuery
                .Where(p => ids.Contains(p.Id))
                .ToArray();

            _dbContext.RemoveRange(entities);
        }

        public void RemoveByScenarioId(Guid scenarioId)
        {
            List<int> entityIds = _dbContext.Query<Entities.Tenant.Scenarios.ScenarioPassReference>()
                 .Where(x => x.ScenarioId == scenarioId)
                 .Select(x => x.Pass.Id)
                 .ToList();
            if (entityIds.Any())
            {
                Remove(entityIds);
            }
        }

        public void Update(Pass pass)
        {
            var entity = PassQuery.FirstOrDefault(x => x.Id == pass.Id);
            if (entity != null)
            {
                _ = _mapper.Map(
                    pass,
                    entity,
                    opt => opt.UseEntityCache(_salesAreaByNameCache));
                _ = _dbContext.Update(
                    entity,
                    p => p.MapTo(
                        pass,
                        opt => opt.UseEntityCache(_salesAreaByIdCache)),
                    _mapper);
            }
        }

        public void Update(IEnumerable<Pass> passes)
        {
            var ids = (passes ?? throw new ArgumentNullException(nameof(passes))).Select(x => x.Id).ToArray();
            var entities = GetPassByExpression(p => ids.Contains(p.Id))
                .ToList();

            foreach (var entity in entities)
            {
                var pass = passes.FirstOrDefault(x => x.Id == entity.Id);
                if (pass != null)
                {
                    _ = _mapper.Map(
                        pass,
                        entity,
                        opt => opt.UseEntityCache(_salesAreaByNameCache));
                    _ = _dbContext.Update(
                        entity,
                        p => p.MapTo(
                            pass,
                            opt => opt.UseEntityCache(_salesAreaByIdCache)),
                        _mapper);
                }
            }
        }

        public void SaveChanges() => _dbContext.SaveChanges();
    }
}

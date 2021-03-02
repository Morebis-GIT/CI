using System;
using System.Collections.Generic;
using System.Linq;
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
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Extensions;
using PassEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes.Pass;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class PassRepository : IPassRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IFullTextSearchConditionBuilder _searchConditionBuilder;
        private readonly IMapper _mapper;

        public PassRepository(ISqlServerTenantDbContext dbContext,
            IFullTextSearchConditionBuilder searchConditionBuilder, IMapper mapper)
        {
            _dbContext = dbContext;
            _searchConditionBuilder = searchConditionBuilder;
            _mapper = mapper;
        }

        public Pass Get(int id) => _dbContext.Query<PassEntity>()
            .ProjectTo<Pass>(_mapper.ConfigurationProvider).FirstOrDefault(p => p.Id == id);

        public Pass FindByName(string name, bool isLibraried)
        {
            var query = _dbContext.Query<PassEntity>()
                .Where(p => p.Name == name)
                .ProjectTo<Pass>(_mapper.ConfigurationProvider);

            return (isLibraried)
                ? query.FirstOrDefault(p => p.IsLibraried == true)
                : query.FirstOrDefault();
        }

        public IEnumerable<Pass> FindByIds(IEnumerable<int> ids) =>
            _dbContext.Query<PassEntity>()
                .Where(p => ids.Contains(p.Id))
                .ProjectTo<Pass>(_mapper.ConfigurationProvider)
                .ToList();

        public IEnumerable<Pass> GetAll() =>
            _dbContext.Query<PassEntity>()
                .ProjectTo<Pass>(_mapper.ConfigurationProvider)
                .ToList();

        public IEnumerable<int> GetLibraryIds() =>
            _dbContext.Query<PassEntity>()
                .Where(x => x.IsLibraried == true)
                .Select(x => x.Id)
                .ToList();

        public IEnumerable<Pass> FindByScenarioId(Guid scenarioId)
        {
            return _dbContext.Query<Entities.Tenant.Scenarios.ScenarioPassReference>()
                .Where(x => x.ScenarioId == scenarioId)
                .Select(x => x.Pass).ProjectTo<Pass>(_mapper.ConfigurationProvider).ToList();
        }

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
            var query = _dbContext.Query<PassEntity>();

            if (queryModel.IsLibraried != null)
            {
                query = queryModel.IsLibraried.Value
                    ? query.Where(e => e.IsLibraried == true)
                    : query.Where(e => e.IsLibraried == null || e.IsLibraried == false);
            }

            // TODO: discuss the business requirements for Pass search before uncommenting the code

            if (!string.IsNullOrWhiteSpace(queryModel.Name) )
            {
                switch (freeTextMatchRules.HowManyWordsToMatch)
                {
                    case StringMatchHowManyWordsToMatch.AllWords:
                        query = query.Where(p => p.Name.Contains(queryModel.Name)).MakeContainsAll();
                        break;
                    case StringMatchHowManyWordsToMatch.AnyWord:
                        query = query.Where(p => p.Name.Contains(queryModel.Name)).MakeContainsAny();
                        break;
                }
                if (!freeTextMatchRules.CaseSensitive)
                {
                    query = query.MakeCaseInsensitive();
                }
            }

            query = (queryModel.OrderBy?.Any() ?? false)
                ? query.OrderByMultipleItems(queryModel.OrderBy)
                : query.OrderBy(p => p.Name);

            var passList = query.ProjectTo<Pass>(_mapper.ConfigurationProvider).AsEnumerable()
                .Where(e => freeTextMatchRules.IsMatches(e.Name, queryModel.Name))
                .ToList();

            return new PagedQueryResult<Pass>(passList.Count,
                passList.ApplyPaging(queryModel.Skip, queryModel.Top).ToList());
        }

        public void Add(Pass pass)
        {
            _dbContext.Add(_mapper.Map<PassEntity>(pass),
                post => post.MapTo(pass), _mapper);
        }

        public void Add(IEnumerable<Pass> items) =>
            _dbContext.AddRange(_mapper.Map<PassEntity[]>(items),
                post => post.MapToCollection(items), _mapper);

        public void Delete(int id)
        {
            _dbContext.Remove(_dbContext.Find<PassEntity>(id));
        }

        public void Remove(IEnumerable<int> ids)
        {
            var entities = _dbContext.Query<PassEntity>()
                .Where(p => ids.Contains(p.Id))
                .ToArray();

            _dbContext.RemoveRange(entities);
        }

        public void RemoveByScenarioId(Guid scenarioId)
        {
            var entities = _dbContext.Query<Entities.Tenant.Scenarios.ScenarioPassReference>()
                .Where(x => x.ScenarioId == scenarioId)
                .Select(x => x.Pass).ToArray();
            if (entities.Any())
            {
                _dbContext.RemoveRange(entities);
            }
        }

        public void Update(Pass pass)
        {
            var entity = GetPassQueryWithAllIncludes().FirstOrDefault(x => x.Id == pass.Id);
            if (entity != null)
            {
                _mapper.Map(pass, entity);
                _dbContext.Update(entity, p => p.MapTo(pass), _mapper);
            }
        }

        public void Update(IEnumerable<Pass> passes)
        {
            var ids = (passes ?? throw new ArgumentNullException(nameof(passes))).Select(x => x.Id).ToArray();
            var entities = GetPassQueryWithAllIncludes()
                .Where(p => ids.Contains(p.Id))
                .ToList();

            foreach (var entity in entities)
            {
                var pass = passes.FirstOrDefault(x => x.Id == entity.Id);
                if (pass != null)
                {
                    _mapper.Map(pass, entity);
                    _dbContext.Update(entity, post => post.MapTo(pass), _mapper);
                }
            }
        }

        public void SaveChanges() => _dbContext.SaveChanges();

        private IQueryable<PassEntity> GetPassQueryWithAllIncludes() =>
            _dbContext.Query<PassEntity>()
                .Include(x => x.General)
                .Include(x => x.Rules)
                .Include(x => x.Tolerances)
                .Include(x => x.Weightings)
                .Include(x => x.BreakExclusions)
                .Include(x => x.RatingPoints)
                .Include(x => x.ProgrammeRepetitions)
                .Include(x => x.SlottingLimits)
                .Include(x => x.PassSalesAreaPriorities)
                    .ThenInclude(x => x.SalesAreaPriorities);
    }
}

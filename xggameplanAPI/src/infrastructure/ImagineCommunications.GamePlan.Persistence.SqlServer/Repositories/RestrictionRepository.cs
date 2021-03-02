using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Restrictions;
using xggameplan.core.Extensions;
using xggameplan.Extensions;
using Restriction = ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects.Restriction;
using RestrictionEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Restrictions.Restriction;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class RestrictionRepository : IRestrictionRepository
    {
        private const int RestrictionsBatchSize = 1000;
        private readonly ISqlServerLongRunningTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public RestrictionRepository(ISqlServerLongRunningTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Add(Restriction item)
        {
            var entity =
                _dbContext.Find<RestrictionEntity>(new object[]
                {
                    (item ?? throw new ArgumentNullException(nameof(item))).Id
                }, post => post.IncludeCollection(e => e.SalesAreas));
            if (entity == null)
            {
                _dbContext.Add(_mapper.Map<RestrictionEntity>(item),
                    post => post.MapTo(item), _mapper);
            }
            else
            {
                _mapper.Map(item, entity);
                _dbContext.Update(entity, post => post.MapTo(item), _mapper);
            }
        }

        public void Add(IEnumerable<Restriction> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var entities = _mapper.Map<IEnumerable<RestrictionEntity>>(items)
                .ToArray();

            _dbContext.AddRange(entities,
                x => x.MapToCollection(items), _mapper);
        }

        public void UpdateRange(IEnumerable<Restriction> restrictions)
        {
            if (restrictions != null && restrictions.Any())
            {
                using (var transaction = _dbContext.Specific.Database.BeginTransaction())
                {
                    var ids = restrictions.Select(s => s.ExternalIdentifier);
                    var dataCount = ids.Count();
                    for (int i = 0; i <= dataCount / RestrictionsBatchSize; i++)
                    {
                        var externalRefs = ids.Skip(i * RestrictionsBatchSize).Take(RestrictionsBatchSize);
                        var restIds = _dbContext.Query<RestrictionEntity>()
                            .Where(x => externalRefs.Contains(x.ExternalIdentifier)).Select(r => r.Id)
                            .ToArray();

                        var restSalesAreaIds = _dbContext.Query<RestrictionSalesArea>()
                            .Where(x => restIds.Contains(x.RestrictionId)).Select(r => r.Id)
                            .ToArray();

                        _dbContext.Specific.RemoveByIdentityIds<RestrictionEntity>(restIds);
                        var salesAreaCount = restSalesAreaIds.Length;
                        for (int j = 0; j <= salesAreaCount / 1000; j++)
                        {
                            var salesAreaIds = restSalesAreaIds.Skip(j * 1000).Take(1000).ToArray();
                            _dbContext.Specific.RemoveByIdentityIds<RestrictionSalesArea>(salesAreaIds);
                        }
                    }

                    var entities = _mapper.Map<IEnumerable<RestrictionEntity>>(restrictions).ToList();

                    _dbContext.BulkInsertEngine.BulkInsert(entities,
                        new BulkInsertOptions { PreserveInsertOrder = true, SetOutputIdentity = true });

                    var restSalesArea = entities.SelectMany(x => x.SalesAreas.Select(
                        r =>
                        {
                            r.RestrictionId = x.Id;
                            return r;
                        })).ToList();

                    _dbContext.BulkInsertEngine.BulkInsert(restSalesArea, new BulkInsertOptions { BatchSize = 500000 });

                    transaction.Commit();
                }
            }
        }

        public void Delete(List<string> salesAreaNames, bool matchAllSpecifiedSalesAreas, DateTime? dateRangeStart, DateTime? dateRangeEnd, RestrictionType? restrictionType)
        {
            const int size = 10000;
            var ids = GetQueryableFilter(salesAreaNames, matchAllSpecifiedSalesAreas, dateRangeStart, dateRangeEnd, restrictionType)
                            .Select(e => e.Id)
                            .ToArray();

            if (!ids.Any())
            {
                return;
            }

            using (var transaction = _dbContext.Specific.Database.BeginTransaction())
            {
                for (int i = 0, page = 0; i < ids.Length; i += size, page++)
                {
                    var batch = ids.Skip(size * page).Take(size).ToArray();
                    _dbContext.Specific.RemoveByIdentityIds<RestrictionEntity>(batch);
                }
                transaction.Commit();
            }
        }

        public void Delete(Guid uid)
        {
            var entity = _dbContext.Query<RestrictionEntity>().FirstOrDefault(x => x.Uid == uid);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public void DeleteRange(IEnumerable<Guid> ids)
        {
            var restrictions = _dbContext.Query<RestrictionEntity>()
                .Where(x => ids.Contains(x.Uid))
                .ToArray();

            if (restrictions.Any())
            {
                _dbContext.RemoveRange(restrictions);
            }
        }

        public void DeleteRangeByExternalRefs(IEnumerable<string> externalRefs)
        {
            var restrictions = _dbContext.Query<RestrictionEntity>()
                .Where(x => externalRefs.Contains(x.ExternalIdentifier))
                .ToArray();

            if (restrictions.Any())
            {
                _dbContext.RemoveRange(restrictions);
            }
        }

        public Restriction Get(Guid uid)
        {
            return _dbContext.Query<RestrictionEntity>()
                .ProjectTo<Restriction>(_mapper.ConfigurationProvider)
                .FirstOrDefault(x => x.Uid == uid);
        }

        public Restriction Get(string externalIdentifier) =>
            _dbContext.Query<RestrictionEntity>()
                .ProjectTo<Restriction>(_mapper.ConfigurationProvider)
                .FirstOrDefault(x => x.ExternalIdentifier == externalIdentifier);

        public IEnumerable<Restriction> Get(List<string> externalIdentifiers) =>
            _dbContext.Query<RestrictionEntity>()
                .Where(x => externalIdentifiers.Contains(x.ExternalIdentifier))
                .ProjectTo<Restriction>(_mapper.ConfigurationProvider)
                .ToList();

        public IEnumerable<Restriction> Get(List<string> salesAreaNames, bool matchAllSpecifiedSalesAreas, DateTime? dateRangeStart, DateTime? dateRangeEnd, RestrictionType? restrictionType)
        {
            var queryable = GetQueryableFilter(salesAreaNames, matchAllSpecifiedSalesAreas, dateRangeStart, dateRangeEnd, restrictionType);

            return queryable
                    .ProjectTo<Restriction>(_mapper.ConfigurationProvider)
                    .ToList();
        }

        public IEnumerable<Restriction> GetAll() =>
            _dbContext.Query<RestrictionEntity>()
                .ProjectTo<Restriction>(_mapper.ConfigurationProvider)
                .ToList();

        public Tuple<Restriction, RestrictionDescription> GetDesc(Guid id)
        {
            var query = GetRestrictionSearchQueryable();
            var item = query.FirstOrDefault(x => x.Uid == id);
            return item != null
                ? Tuple.Create(_mapper.Map<Restriction>(item), _mapper.Map<RestrictionDescription>(item))
                : null;
        }

        public PagedQueryResult<Tuple<Restriction, RestrictionDescription>> Get(RestrictionSearchQueryModel searchQueryModel)
        {
            if (searchQueryModel == null)
            {
                throw new ArgumentNullException(nameof(searchQueryModel));
            }

            var where = new List<Expression<Func<RestrictionSearchDto, bool>>>();

            if (searchQueryModel.SalesAreaNames != null && searchQueryModel.SalesAreaNames.Any())
            {
                if (searchQueryModel.MatchAllSpecifiedSalesAreas)
                {
                    where.Add(x => x.SalesAreas == null || !x.SalesAreas.Any()
                                                        || x.SalesAreas.Count(y => searchQueryModel.SalesAreaNames.Contains(y.SalesArea)) == searchQueryModel.SalesAreaNames.Count);
                }
                else
                {
                    where.Add(x => x.SalesAreas == null || !x.SalesAreas.Any()
                                                        || x.SalesAreas.Any(y => searchQueryModel.SalesAreaNames.Contains(y.SalesArea)));
                }
            }

            if (searchQueryModel.DateRangeStart != null)
            {
                where.Add(x => x.EndDate == null || x.EndDate.Value.Date >= searchQueryModel.DateRangeStart.Value.Date);
            }

            if (searchQueryModel.DateRangeEnd != null)
            {
                where.Add(x => x.StartDate.Date <= searchQueryModel.DateRangeEnd.Value.Date);
            }

            if (searchQueryModel.RestrictionType != null)
            {
                where.Add(x => x.RestrictionType == _mapper.Map<Entities.RestrictionType>(searchQueryModel.RestrictionType.Value));
            }

            var query = GetRestrictionSearchQueryable();

            var queryable = !where.Any() ?
                query :
                query.Where(where.AggregateAnd());

            var sortedItems = queryable
                .OrderBySingleItem(searchQueryModel.OrderBy.ToString(), searchQueryModel.OrderDirection)
                .ApplyPaging(searchQueryModel.Skip, searchQueryModel.Top)
                .ToList();

            var result = sortedItems
                .Select(x => Tuple.Create(_mapper.Map<Restriction>(x), _mapper.Map<RestrictionDescription>(x))).ToList();

            return new PagedQueryResult<Tuple<Restriction, RestrictionDescription>>(queryable.Count(), result);
        }

        public void Truncate()
        {
            _dbContext.Truncate<RestrictionEntity>();
        }

        public void SaveChanges() => _dbContext.SaveChanges();

        protected virtual IQueryable<RestrictionSearchDto> GetRestrictionSearchQueryable()
        {
            var query =
                (from restriction in _dbContext.Query<RestrictionEntity>()
                 join prdct in _dbContext.Query<Product>() on restriction.ProductCode equals prdct.Externalidentifier
                     into products
                 from product in products.DefaultIfEmpty()
                 join cl in _dbContext.Query<Clash>() on restriction.ClashCode equals cl.Externalref
                     into clashes
                 from clash in clashes.DefaultIfEmpty()
                 join paj in _dbContext.Query<ProductAdvertiser>() on product.Id equals paj.ProductId into paJoin
                 from pa in paJoin.DefaultIfEmpty()
                 join aj in _dbContext.Query<Advertiser>() on pa.AdvertiserId equals aj.Id into aJoin
                 from a in aJoin.DefaultIfEmpty()

                 let programme = _dbContext.Query<ProgrammeDictionary>()
                     .FirstOrDefault(x => restriction.ExternalProgRef == x.ExternalReference)

                 select new RestrictionSearchDto
                 {
                     Id = restriction.Id,
                     Uid = restriction.Uid,
                     StartDate = restriction.StartDate,
                     EndDate = restriction.EndDate,
                     StartTime = restriction.StartTime,
                     EndTime = restriction.EndTime,
                     RestrictionDays = restriction.RestrictionDays,
                     SchoolHolidayIndicator = restriction.SchoolHolidayIndicator,
                     PublicHolidayIndicator = restriction.PublicHolidayIndicator,
                     LiveProgrammeIndicator = restriction.LiveProgrammeIndicator,
                     RestrictionType = restriction.RestrictionType,
                     RestrictionBasis = restriction.RestrictionBasis,
                     ExternalProgRef = restriction.ExternalProgRef,
                     ProgrammeCategory = restriction.ProgrammeCategory,
                     ProgrammeClassification = restriction.ProgrammeClassification,
                     ProgrammeClassificationIndicator = restriction.ProgrammeClassificationIndicator,
                     TimeToleranceMinsBefore = restriction.TimeToleranceMinsBefore,
                     TimeToleranceMinsAfter = restriction.TimeToleranceMinsAfter,
                     IndexType = restriction.IndexType,
                     IndexThreshold = restriction.IndexThreshold,
                     ProductCode = Convert.ToInt32(restriction.ProductCode),
                     ClashCode = restriction.ClashCode,
                     ClearanceCode = restriction.ClearanceCode,
                     ClockNumber = restriction.ClockNumber,
                     ExternalIdentifier = restriction.ExternalIdentifier,
                     SalesAreas = restriction.SalesAreas,
                     ProgrammeDescription = programme.Name,
                     ProductDescription = product.Name,
                     AdvertiserName = a.Name,
                     ClashDescription = clash.Description
                 })
                .AsQueryable();
            return query;
        }

        private IQueryable<RestrictionEntity> GetQueryableFilter(List<string> salesAreaNames, bool matchAllSpecifiedSalesAreas, DateTime? dateRangeStart, DateTime? dateRangeEnd, RestrictionType? restrictionType)
        {
            var where = new List<Expression<Func<RestrictionEntity, bool>>>();

            if (salesAreaNames != null && salesAreaNames.Any())
            {
                if (matchAllSpecifiedSalesAreas)
                {
                    where.Add(p => p.SalesAreas == null || !p.SalesAreas.Any()
                                        || p.SalesAreas.Count(x => salesAreaNames.Contains(x.SalesArea)) == salesAreaNames.Count);
                }
                else
                {
                    where.Add(p => p.SalesAreas == null || !p.SalesAreas.Any()
                                        || p.SalesAreas.Any(x => salesAreaNames.Contains(x.SalesArea)));
                }
            }

            if (dateRangeStart != null)
            {
                where.Add(p => p.StartDate >= dateRangeStart.Value.Date);
            }

            if (dateRangeEnd != null)
            {
                where.Add(p => p.EndDate < dateRangeEnd.Value.Date.AddDays(1) || p.EndDate == null);
            }

            if (restrictionType != null)
            {
                where.Add(p => p.RestrictionType == _mapper.Map<Entities.RestrictionType>(restrictionType.Value));
            }

            if (where.Any() == false)
            {
                return _dbContext.Query<RestrictionEntity>();
            }

            return !where.Any() ?
                _dbContext.Query<RestrictionEntity>() :
                _dbContext.Query<RestrictionEntity>().Where(where.AggregateAnd());
        }
    }
}

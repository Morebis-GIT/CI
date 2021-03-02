using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
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
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Extensions;
using xggameplan.core.Extensions.AutoMapper;
using xggameplan.Extensions;
using Restriction = ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects.Restriction;
using RestrictionEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Restrictions.Restriction;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class RestrictionRepository : IRestrictionRepository
    {
        private const int RestrictionsBatchSize = 1000;
        private readonly ISqlServerLongRunningTenantDbContext _dbContext;
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;
        private readonly IMapper _mapper;

        public RestrictionRepository(
            ISqlServerLongRunningTenantDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
            _mapper = mapper;
        }

        protected IQueryable<RestrictionEntity> RestrictionsQuery =>
            _dbContext.Query<RestrictionEntity>().Include(x => x.SalesAreas);

        public void Add(Restriction item)
        {
            var entity =
                _dbContext.Find<RestrictionEntity>(new object[]
                {
                    (item ?? throw new ArgumentNullException(nameof(item))).Id
                }, post => post.IncludeCollection(e => e.SalesAreas));
            if (entity == null)
            {
                _ = _dbContext.Add(_mapper.Map<RestrictionEntity>(item, opts => opts.UseEntityCache(_salesAreaByNameCache)),
                    post => post.MapTo(item, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);
            }
            else
            {
                _ = _mapper.Map(item, entity, opts => opts.UseEntityCache(_salesAreaByNameCache));
                _ = _dbContext.Update(entity, post => post.MapTo(item, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);
            }
        }

        public void Add(IEnumerable<Restriction> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var entities = _mapper.Map<IEnumerable<RestrictionEntity>>(items, opts => opts.UseEntityCache(_salesAreaByNameCache))
                .ToArray();

            _dbContext.AddRange(entities,
                x => x.MapToCollection(items, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);
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

                        _ = _dbContext.Specific.RemoveByIdentityIds<RestrictionEntity>(restIds);
                        var salesAreaCount = restSalesAreaIds.Length;
                        for (int j = 0; j <= salesAreaCount / 1000; j++)
                        {
                            var salesAreaIds = restSalesAreaIds.Skip(j * 1000).Take(1000).ToArray();
                            _ = _dbContext.Specific.RemoveByIdentityIds<RestrictionSalesArea>(salesAreaIds);
                        }
                    }

                    var entities = _mapper.Map<IEnumerable<RestrictionEntity>>(restrictions, opts => opts.UseEntityCache(_salesAreaByNameCache)).ToList();

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
                    _ = _dbContext.Specific.RemoveByIdentityIds<RestrictionEntity>(batch);
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

        public Restriction Get(Guid uid) =>
            _mapper.Map<Restriction>(RestrictionsQuery
                    .FirstOrDefault(x => x.Uid == uid), opts => opts.UseEntityCache(_salesAreaByIdCache));

        public Restriction Get(string externalIdentifier) =>
            _mapper.Map<Restriction>(RestrictionsQuery
                .FirstOrDefault(x => x.ExternalIdentifier == externalIdentifier), opts => opts.UseEntityCache(_salesAreaByIdCache));

        public IEnumerable<Restriction> Get(List<string> externalIdentifiers) =>
            _mapper.Map<List<Restriction>>(RestrictionsQuery
                .Where(x => externalIdentifiers.Contains(x.ExternalIdentifier))
                .Select(RestrictionSqlEntitySelect).AsNoTracking(), opts => opts.UseEntityCache(_salesAreaByIdCache));

        public IEnumerable<Restriction> Get(List<string> salesAreaNames, bool matchAllSpecifiedSalesAreas, DateTime? dateRangeStart, DateTime? dateRangeEnd, RestrictionType? restrictionType)
        {
            var queryable = GetQueryableFilter(salesAreaNames, matchAllSpecifiedSalesAreas, dateRangeStart, dateRangeEnd, restrictionType);
            return _mapper.Map<List<Restriction>>(queryable.Select(RestrictionSqlEntitySelect).AsNoTracking(), opts => opts.UseEntityCache(_salesAreaByIdCache));
        }

        public IEnumerable<Restriction> GetAll() =>
            _mapper.Map<List<Restriction>>(RestrictionsQuery.Select(RestrictionSqlEntitySelect).AsNoTracking(),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public Tuple<Restriction, RestrictionDescription> GetDesc(Guid id)
        {
            var query = GetRestrictionSearchQueryable();
            var item = query.FirstOrDefault(x => x.Uid == id);
            return item != null
                ? Tuple.Create(_mapper.Map<Restriction>(item, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper.Map<RestrictionDescription>(item))
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
                                                        || x.SalesAreas.Count(y => searchQueryModel.SalesAreaNames.Contains(y.SalesArea.Name)) == searchQueryModel.SalesAreaNames.Count);
                }
                else
                {
                    where.Add(x => x.SalesAreas == null || !x.SalesAreas.Any()
                                                        || x.SalesAreas.Any(y => searchQueryModel.SalesAreaNames.Contains(y.SalesArea.Name)));
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
                .Select(x => Tuple.Create(_mapper.Map<Restriction>(x, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper.Map<RestrictionDescription>(x))).ToList();

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
                 join prdct in _dbContext.Query<Product>() on restriction.ProductCode.ToString() equals prdct.Externalidentifier
                     into products
                 from product in products.DefaultIfEmpty()
                 join cl in _dbContext.Query<Clash>() on restriction.ClashCode equals cl.Externalref
                     into clashes
                 from clash in clashes.DefaultIfEmpty()
                 join paj in _dbContext.Query<ProductAdvertiser>() on product.Uid equals paj.ProductId into paJoin
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
                     ProductCode = restriction.ProductCode,
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
                                        || p.SalesAreas.Count(x => salesAreaNames.Contains(x.SalesArea.Name)) == salesAreaNames.Count);
                }
                else
                {
                    where.Add(p => p.SalesAreas == null || !p.SalesAreas.Any()
                                        || p.SalesAreas.Any(x => salesAreaNames.Contains(x.SalesArea.Name)));
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
                return RestrictionsQuery;
            }

            return !where.Any() ?
                RestrictionsQuery :
                RestrictionsQuery.Where(where.AggregateAnd());
        }

        /// <summary>
        /// Represents projection of EF Core Restriction model to itself, contains all its properties.
        /// Is used to prevent EF Core 2.2.6 identity resolution (even for no-tracking queries)
        /// that kills performance for large data-sets.
        /// </summary>
        /// <remarks>
        /// Can be removed after migrating to EF Core 3.0 or higher
        /// </remarks>>
        protected static Expression<Func<RestrictionEntity, RestrictionEntity>> RestrictionSqlEntitySelect => x =>
            new RestrictionEntity
            {
                Id = x.Id,
                Uid = x.Uid,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                RestrictionDays = x.RestrictionDays,
                SchoolHolidayIndicator = x.SchoolHolidayIndicator,
                PublicHolidayIndicator = x.PublicHolidayIndicator,
                LiveProgrammeIndicator = x.LiveProgrammeIndicator,
                RestrictionType = x.RestrictionType,
                RestrictionBasis = x.RestrictionBasis,
                ExternalProgRef = x.ExternalProgRef,
                ProgrammeCategory = x.ProgrammeCategory,
                ProgrammeClassification = x.ProgrammeClassification,
                ProgrammeClassificationIndicator = x.ProgrammeClassificationIndicator,
                TimeToleranceMinsBefore = x.TimeToleranceMinsBefore,
                TimeToleranceMinsAfter = x.TimeToleranceMinsAfter,
                IndexType = x.IndexType,
                IndexThreshold = x.IndexThreshold,
                ProductCode = x.ProductCode,
                ClashCode = x.ClashCode,
                ClearanceCode = x.ClearanceCode,
                ClockNumber = x.ClockNumber,
                ExternalIdentifier = x.ExternalIdentifier,
                EpisodeNumber = x.EpisodeNumber,
                SalesAreas = x.SalesAreas.Select(rs => new RestrictionSalesArea
                {
                    Id = rs.Id,
                    RestrictionId = rs.RestrictionId,
                    SalesAreaId = rs.SalesAreaId
                }).ToList()
            };
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Extensions;
using xggameplan.core.Extensions.AutoMapper;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class ClashRepository : IClashRepository
    {
        private const int MaxClauseCount = 2000;
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly ISqlServerSalesAreaByNullableIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;
        private readonly IFullTextSearchConditionBuilder _searchConditionBuilder;
        private readonly IMapper _mapper;

        public ClashRepository(
            ISqlServerTenantDbContext dbContext,
            ISqlServerSalesAreaByNullableIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IFullTextSearchConditionBuilder searchConditionBuilder,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
            _searchConditionBuilder = searchConditionBuilder;
            _mapper = mapper;
        }

        public int CountAll => _dbContext.Query<Entities.Tenant.Clash>().Count();

        public void Add(Clash item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var entity = ClashQuery()
                .FirstOrDefault(x => x.Uid == item.Uid);

            if (entity == null)
            {
                _ = _dbContext.Add(_mapper.Map<Entities.Tenant.Clash>(item, opts => opts.UseEntityCache(_salesAreaByNameCache)),
                    post => post.MapTo(item, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);
            }
            else
            {
                _ = _mapper.Map(item, entity, opts => opts.UseEntityCache(_salesAreaByNameCache));
                _ = _dbContext.Update(entity, post => post.MapTo(item, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);
            }
        }

        /// <summary>
        /// BulkInsert method. It doesn't need SaveChanges for applying changes.
        /// </summary>
        /// <param name="items"></param>
        public void Add(IEnumerable<Clash> items)
        {
            if (items == null || !items.Any())
            {
                return;
            }

            var entities = _mapper.Map<List<Entities.Tenant.Clash>>(items, opts => opts.UseEntityCache(_salesAreaByNameCache));
            using var transaction = _dbContext.Specific.Database.BeginTransaction();
            _dbContext.BulkInsertEngine.BulkInsert(entities);

            var ratingEntities = entities.SelectMany(x => x.Differences.Select(
                r =>
                {
                    r.ClashId = x.Uid;
                    return r;
                })).ToList();

            _dbContext.BulkInsertEngine.BulkInsert(ratingEntities,
                new BulkInsertOptions {PreserveInsertOrder = true, SetOutputIdentity = true});

             var timeAndDows = ratingEntities.Where(x => x.TimeAndDow != null).Select(x =>
                {
                        x.TimeAndDow.ClashDifferenceId = x.Id;
                        return x.TimeAndDow;
                }).ToList();

            _dbContext.BulkInsertEngine.BulkInsert(timeAndDows);

            transaction.Commit();
        }

        public IEnumerable<ClashNameModel> GetDescriptionByExternalRefs(ICollection<string> externalRefs)
        {
            var distinctExternalRefs = externalRefs.Distinct().ToList();
            var result = new List<ClashNameModel>();
            for (int i = 0, page = 0; i < distinctExternalRefs.Count; i += MaxClauseCount, page++)
            {
                var ids = distinctExternalRefs.Skip(MaxClauseCount * page).Take(MaxClauseCount).ToArray();
                result.AddRange(_dbContext.Query<Entities.Tenant.Clash>()
                    .Where(c => ids.Contains(c.Externalref)).ProjectTo<ClashNameModel>(_mapper.ConfigurationProvider).ToArray());
            }

            return result;
        }

        public int Count() =>
            _dbContext.Query<Entities.Tenant.Clash>().Count();

        public int Count(Expression<Func<Clash, bool>> query) =>
            _dbContext.Query<Entities.Tenant.Clash>().ProjectTo<Clash>(_mapper.ConfigurationProvider).Count(query);

        public void Delete(Guid uid)
        {
            var entity = _dbContext.Find<Entities.Tenant.Clash>(uid);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public void DeleteRange(IEnumerable<Guid> ids)
        {
            var clashes = _dbContext.Query<Entities.Tenant.Clash>()
                .Where(x => ids.Contains(x.Uid))
                .ToArray();

            if (clashes.Any())
            {
                _dbContext.RemoveRange(clashes);
            }
        }

        public Clash Find(Guid uid) => Get(uid);

        public IEnumerable<Clash> FindByExternal(string externalRef) =>
            _mapper.Map<List<Clash>>(ClashQuery()
                .Where(x => x.Externalref == externalRef), opts => opts.UseEntityCache(_salesAreaByIdCache));

        public IEnumerable<Clash> FindByExternal(List<string> externalRefs) =>
            _mapper.Map<List<Clash>>(ClashQuery()
                .Where(x => externalRefs.Contains(x.Externalref)), opts => opts.UseEntityCache(_salesAreaByIdCache));

        public Clash Get(Guid uid) =>
            _mapper.Map<Clash>(ClashQuery()
                .FirstOrDefault(x => x.Uid == uid), opts => opts.UseEntityCache(_salesAreaByIdCache));

        public IEnumerable<Clash> GetAll() =>
            _mapper.Map<List<Clash>>(ClashQuery(), opts => opts.UseEntityCache(_salesAreaByIdCache));

        public void Remove(Guid id, out bool isDeleted)
        {
            isDeleted = false;
            var entity = _dbContext.Find<Entities.Tenant.Clash>(id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
                isDeleted = true;
            }
        }

        public void Remove(Guid uid) => Delete(uid);

        public void SaveChanges() => _dbContext.SaveChanges();

        public PagedQueryResult<ClashNameModel> Search(ClashSearchQueryModel queryModel)
        {
            if (queryModel == null)
            {
                throw new ArgumentNullException(nameof(queryModel));
            }

            var query = _dbContext.Query<Entities.Tenant.Clash>();
            if (!String.IsNullOrWhiteSpace(queryModel.NameOrRef))
            {
                query = query.Where(p =>
                        p.Description.StartsWith(queryModel.NameOrRef)
                        || p.Externalref.StartsWith(queryModel.NameOrRef)).MakeCaseInsensitive();
            }

            return new PagedQueryResult<ClashNameModel>(query.Count(),
                query.ApplyPaging(queryModel.Skip, queryModel.Top).ProjectTo<ClashNameModel>(_mapper.ConfigurationProvider).ToList());
        }

        public void Truncate() => _dbContext.Truncate<Entities.Tenant.Clash>();

        public Task TruncateAsync() => Task.Run(Truncate);

        public void UpdateRange(IEnumerable<Clash> clashes)
        {
            var ids = clashes.Select(x => x.Uid).ToArray();
            var entities = ClashQuery()
                .Where(x => ids.Contains(x.Uid))
                .ToArray();

            foreach (var entity in entities)
            {
                var clash = clashes.FirstOrDefault(x => x.Uid == entity.Uid);

                if (clash != null)
                {
                    _ = _mapper.Map(clash, entity, opts => opts.UseEntityCache(_salesAreaByNameCache));
                }
            }

            _dbContext.UpdateRange(entities, post => post.MapToCollection(clashes, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);
        }

        protected IQueryable<Entities.Tenant.Clash> ClashQuery() =>
            _dbContext.Query<Entities.Tenant.Clash>()
                .Include(x => x.Differences).ThenInclude(x => x.TimeAndDow);
    }
}

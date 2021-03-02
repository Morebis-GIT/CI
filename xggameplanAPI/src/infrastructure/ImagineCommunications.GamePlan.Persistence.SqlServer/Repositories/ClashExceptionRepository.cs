using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Queries;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.PostProcessing.Builders;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ClashExceptions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Views.Tenant;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Extensions;
using ClashException = ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects.ClashException;
using ClashExceptionEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ClashExceptions.ClashException;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class ClashExceptionRepository : IClashExceptionRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public ClashExceptionRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public int CountAll => _dbContext.Query<ClashExceptionEntity>().Count();

        public ClashException Find(int id) =>
            _mapper.Map<ClashException>(_dbContext.Find<ClashExceptionEntity>(new object[] { id },
                post => post.IncludeCollection(e => e.ClashExceptionsTimeAndDows)));

        public ClashExceptionModel GetWithDescriptions(int id)
        {
            return GetWithDescriptions(new[] { id }).FirstOrDefault();
        }

        public List<ClashExceptionModel> GetWithDescriptions(IEnumerable<int> ids)
        {
            return
                (from clashException in _dbContext.Query<ClashExceptionEntity>()
                 join clashExceptionDescriptions in _dbContext.Specific.View<ClashExceptionDescriptions>() on
                     clashException.Id equals clashExceptionDescriptions.Id
                 where ids.Contains(clashException.Id)
                 select new ClashExceptionModel
                 {
                     Id = clashException.Id,
                     StartDate = clashException.StartDate,
                     EndDate = clashException.EndDate,
                     ExternalRef = clashException.ExternalRef,
                     FromType = (ClashExceptionType)(int)clashException.FromType,
                     ToType = (ClashExceptionType)(int)clashException.ToType,
                     FromValue = clashException.FromValue,
                     ToValue = clashException.ToValue,
                     IncludeOrExclude = (IncludeOrExclude)(int)clashException.IncludeOrExclude,
                     FromValueDescription = clashExceptionDescriptions.FromValueDescription,
                     ToValueDescription = clashExceptionDescriptions.ToValueDescription,
                     TimeAndDows = clashException.ClashExceptionsTimeAndDows.Select(x => new TimeAndDow
                     {
                         StartTime = x.StartTime,
                         EndTime = x.EndTime,
                         DaysOfWeek = x.DaysOfWeek
                     }).ToList()
                 }).ToList();
        }

        public void Remove(int id)
        {
            var entity = _dbContext.Find<ClashExceptionEntity>(id);
            if (!(entity is null))
            {
                _dbContext.Remove(entity);
            }
        }

        public void Add(ClashException item)
        {
            var entity =
                _dbContext.Find<ClashExceptionEntity>(new object[]
                {
                    (item ?? throw new ArgumentNullException(nameof(item))).Id
                }, post => post.IncludeCollection(e => e.ClashExceptionsTimeAndDows));
            if (entity is null)
            {
                _dbContext.Add(_mapper.Map<ClashExceptionEntity>(item),
                    post => post.MapTo(item), _mapper);
            }
            else
            {
                _mapper.Map(item, entity);
                _dbContext.Update(entity, post => post.MapTo(item), _mapper);
            }
        }

        public void Add(IEnumerable<ClashException> item)
        {
            using var transaction = _dbContext.Specific.Database.BeginTransaction();
            var entities = _mapper.Map<List<ClashExceptionEntity>>(item);
            var ids = entities.Where(x => x.Id > 0).Select(x => x.Id).Distinct().ToList();

            _dbContext.BulkInsertEngine.BulkInsertOrUpdate(
                entities, new BulkInsertOptions { PreserveInsertOrder = true, SetOutputIdentity = true });

            var refIds = ids.Any()
                ? _dbContext.Query<ClashExceptionsTimeAndDow>().Where(x => ids.Contains(x.ClashExceptionId))
                    .Select(x => x.Id).AsEnumerable().ToArray()
                : Array.Empty<int>();

            if (refIds.Any())
            {
                _dbContext.Specific.RemoveByIdentityIds<ClashExceptionsTimeAndDow>(refIds);
            }

            var refList = entities.SelectMany(x => x.ClashExceptionsTimeAndDows.Select(r =>
            {
                r.ClashExceptionId = x.Id;
                return r;
            })).ToList();

            _dbContext.BulkInsertEngine.BulkInsert(refList,
                new BulkInsertOptions { PreserveInsertOrder = true, SetOutputIdentity = true });

            transaction.Commit();

            var actionBuilder = new BulkInsertActionBuilder<ClashExceptionEntity>(entities, _mapper);
            actionBuilder.TryToUpdate(item);
            actionBuilder.Build()?.Execute();
        }

        public IEnumerable<ClashException> GetAll() =>
            _dbContext.Query<ClashExceptionEntity>().ProjectTo<ClashException>(_mapper.ConfigurationProvider).ToList();

        public int Count(Expression<Func<ClashException, bool>> query) =>
            _dbContext.Query<ClashExceptionEntity>().ProjectTo<ClashException>(_mapper.ConfigurationProvider).Count(query);

        public void Truncate()
        {
            _dbContext.Truncate<ClashExceptionEntity>();
        }

        public PagedQueryResult<ClashException> Search(ClashExceptionSearchQueryModel searchQuery)
        {
            if (searchQuery is null)
            {
                throw new ArgumentNullException(nameof(searchQuery));
            }

            var items = _dbContext.Query<ClashExceptionEntity>();

            if (searchQuery.StartDate != DateTime.MinValue && searchQuery.EndDate != DateTime.MinValue)
            {
                items = items.Where(c =>
                    c.StartDate.Date >= searchQuery.StartDate.Date &&
                    c.StartDate.Date < searchQuery.EndDate.Date.AddDays(1) ||
                    c.EndDate != null && c.EndDate.Value.Date >= searchQuery.StartDate.Date &&
                    c.EndDate.Value.Date < searchQuery.EndDate.Date.AddDays(1) ||
                    c.StartDate.Date < searchQuery.EndDate.Date.AddDays(1) &&
                    (c.EndDate == null || c.EndDate.Value.Date >= searchQuery.EndDate.Date));
            }
            else
            {
                if (searchQuery.StartDate != DateTime.MinValue && searchQuery.EndDate == DateTime.MinValue)
                {
                    items = items.Where(c =>
                        c.StartDate.Date >= searchQuery.StartDate.Date ||
                        c.StartDate.Date < searchQuery.StartDate.Date.AddDays(1) &&
                        (c.EndDate == null || c.EndDate.Value.Date >= searchQuery.StartDate.Date));
                }
                else
                {
                    if (searchQuery.StartDate == DateTime.MinValue && searchQuery.EndDate != DateTime.MinValue)
                    {
                        items = items.Where(c =>
                            c.EndDate != null &&
                            c.EndDate.Value.Date < searchQuery.EndDate.Date.AddDays(1) ||
                            c.StartDate.Date < searchQuery.EndDate.Date.AddDays(1) &&
                            (c.EndDate == null || c.EndDate.Value.Date >= searchQuery.EndDate.Date));
                    }
                }
            }

            var sortedItems =
                items.OrderBySingleItem(searchQuery.OrderBy.ToString(),
                        searchQuery.OrderByDirection).ApplyPaging(searchQuery.Skip, searchQuery.Top)
                    .Include(e => e.ClashExceptionsTimeAndDows).ToList();

            return new PagedQueryResult<ClashException>(items.Count(), _mapper.Map<List<ClashException>>(sortedItems));
        }

        public PagedQueryResult<ClashExceptionModel> SearchWithDescriptions(ClashExceptionSearchQueryModel searchQuery)
        {
            if (searchQuery is null)
            {
                throw new ArgumentNullException(nameof(searchQuery));
            }

            var query =
                from clashException in _dbContext.Query<ClashExceptionEntity>()
                join clashExceptionDescriptions in _dbContext.Specific.View<ClashExceptionDescriptions>() on clashException.Id equals clashExceptionDescriptions.Id
                select new { clashException, clashExceptionDescriptions };

            if (searchQuery.StartDate != DateTime.MinValue && searchQuery.EndDate != DateTime.MinValue)
            {
                query = query.Where(q =>
                    q.clashException.StartDate.Date >= searchQuery.StartDate.Date &&
                    q.clashException.StartDate.Date < searchQuery.EndDate.Date.AddDays(1) ||
                    q.clashException.EndDate != null && q.clashException.EndDate.Value.Date >= searchQuery.StartDate.Date &&
                    q.clashException.EndDate.Value.Date < searchQuery.EndDate.Date.AddDays(1) ||
                    q.clashException.StartDate.Date < searchQuery.EndDate.Date.AddDays(1) &&
                    (q.clashException.EndDate == null || q.clashException.EndDate.Value.Date >= searchQuery.EndDate.Date));
            }
            else
            {
                if (searchQuery.StartDate != DateTime.MinValue && searchQuery.EndDate == DateTime.MinValue)
                {
                    query = query.Where(q =>
                        q.clashException.StartDate.Date >= searchQuery.StartDate.Date ||
                        q.clashException.StartDate.Date < searchQuery.StartDate.Date.AddDays(1) &&
                        (q.clashException.EndDate == null || q.clashException.EndDate.Value.Date >= searchQuery.StartDate.Date));
                }
                else
                {
                    if (searchQuery.StartDate == DateTime.MinValue && searchQuery.EndDate != DateTime.MinValue)
                    {
                        query = query.Where(q =>
                            q.clashException.EndDate != null &&
                            q.clashException.EndDate.Value.Date < searchQuery.EndDate.Date.AddDays(1) ||
                            q.clashException.StartDate.Date < searchQuery.EndDate.Date.AddDays(1) &&
                            (q.clashException.EndDate == null || q.clashException.EndDate.Value.Date >= searchQuery.EndDate.Date));
                    }
                }
            }

            var items = query.Select(q => new ClashExceptionModel
            {
                Id = q.clashException.Id,
                StartDate = q.clashException.StartDate,
                EndDate = q.clashException.EndDate,
                ExternalRef = q.clashException.ExternalRef,
                FromType = (ClashExceptionType)(int)q.clashException.FromType,
                ToType = (ClashExceptionType)(int)q.clashException.ToType,
                FromValue = q.clashException.FromValue,
                ToValue = q.clashException.ToValue,
                IncludeOrExclude = (IncludeOrExclude)(int)q.clashException.IncludeOrExclude,
                FromValueDescription = q.clashExceptionDescriptions.FromValueDescription,
                ToValueDescription = q.clashExceptionDescriptions.ToValueDescription,
                TimeAndDows = q.clashException.ClashExceptionsTimeAndDows.Select(x => new TimeAndDow
                {
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    DaysOfWeek = x.DaysOfWeek
                }).ToList()
            });

            var sortedItems =
                items.OrderBySingleItem(searchQuery.OrderBy.ToString(),
                        searchQuery.OrderByDirection).ApplyPaging(searchQuery.Skip, searchQuery.Top)
                    .ToList();

            return new PagedQueryResult<ClashExceptionModel>(items.Count(), sortedItems);
        }

        // Set NotImplementedException for next methods due to the same
        // implementation in RavenClashExceptionRepository
        public IEnumerable<ClashException> FindByExternal(string externalRef) => throw new NotImplementedException();

        public IEnumerable<ClashException> FindByExternal(List<string> externalRefs) => throw new NotImplementedException();

        public IEnumerable<ClashException> GetActive() =>
            _dbContext.Query<ClashExceptionEntity>()
                .Where(e => e.EndDate == null || e.EndDate >= DateTime.Today.Date)
                .ProjectTo<ClashException>(_mapper.ConfigurationProvider)
                .ToList();

        public ClashException Find(Guid uid) => throw new NotImplementedException();

        public void Remove(Guid uid) => throw new NotImplementedException();

        public void SaveChanges() => _dbContext.SaveChanges();

        public void Delete(ClashExceptionType fromType, ClashExceptionType toType, string fromValue, string toValue)
        {
            var ft = (Entities.ClashExceptionType)(int)fromType;
            var tt = (Entities.ClashExceptionType)(int)toType;
            var clashExceptions = _dbContext.Query<ClashExceptionEntity>()
                .Where(x => x.FromType == ft && x.ToType == tt && x.FromValue == fromValue && x.ToValue == toValue)
                .ToArray();

            if (clashExceptions.Any())
            {
                _dbContext.RemoveRange(clashExceptions);
            }
        }

        public void DeleteRangeByExternalRefs(IEnumerable<string> externalRefs)
        {
            var clashExceptions = _dbContext.Query<ClashExceptionEntity>()
                .Where(x => externalRefs.Contains(x.ExternalRef))
                .ToArray();

            if (clashExceptions.Any())
            {
                _dbContext.RemoveRange(clashExceptions);
            }
        }
    }
}

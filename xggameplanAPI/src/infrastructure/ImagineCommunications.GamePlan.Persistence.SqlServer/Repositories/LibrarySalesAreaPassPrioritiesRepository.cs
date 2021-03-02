using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.LibrarySalesAreaPassPriorities;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Extensions.AutoMapper;
using xggameplan.Extensions;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class LibrarySalesAreaPassPrioritiesRepository : ILibrarySalesAreaPassPrioritiesRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;
        private readonly IMapper _mapper;

        public LibrarySalesAreaPassPrioritiesRepository(
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

        public async Task AddAsync(LibrarySalesAreaPassPriority item)
        {
            _ = await _dbContext.AddAsync(
                    _mapper.Map<Entities.Tenant.LibrarySalesAreaPassPriority>(item,
                        opts => opts.UseEntityCache(_salesAreaByNameCache)),
                    post => post.MapTo(item, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper)
                .ConfigureAwait(false);
        }

        public async Task UpdateAsync(LibrarySalesAreaPassPriority item)
        {
            var entity = await _dbContext.FindAsync<Entities.Tenant.LibrarySalesAreaPassPriority>(
                    new object[] { item?.Uid },
                    find => find.IncludeCollection(e => e.SalesAreaPriorities))
                .ConfigureAwait(false);

            if (entity != null)
            {
                _ = _mapper.Map(item, entity, opts => opts.UseEntityCache(_salesAreaByNameCache));

                _ = _dbContext.Update(entity,
                    post => post.MapTo(item, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);
            }
        }

        public async Task Delete(Guid id)
        {
            var entity = await _dbContext.FindAsync<Entities.Tenant.LibrarySalesAreaPassPriority>(id)
                .ConfigureAwait(false);

            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<LibrarySalesAreaPassPriority, bool>> condition)
        {
            return await _dbContext.Query<Entities.Tenant.LibrarySalesAreaPassPriority>()
                .ProjectTo<LibrarySalesAreaPassPriority>(_mapper.ConfigurationProvider)
                .AnyAsync(condition)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<LibrarySalesAreaPassPriority>> GetAllAsync()
        {
            return _mapper.Map<List<LibrarySalesAreaPassPriority>>(
                await _dbContext.Query<Entities.Tenant.LibrarySalesAreaPassPriority>()
                    .Include(e => e.SalesAreaPriorities)
                    .AsNoTracking()
                    .ToListAsync()
                    .ConfigureAwait(false), opts => opts.UseEntityCache(_salesAreaByIdCache));
        }

        public async Task<LibrarySalesAreaPassPriority> GetAsync(Guid uId)
        {
            return _mapper.Map<LibrarySalesAreaPassPriority>(await _dbContext
                    .Query<Entities.Tenant.LibrarySalesAreaPassPriority>()
                    .Include(x => x.SalesAreaPriorities).ThenInclude(x => x.SalesArea)
                    .Where(x => x.Uid == uId).FirstOrDefaultAsync().ConfigureAwait(false),
                opts => opts.UseEntityCache(_salesAreaByIdCache));
        }

        public async Task<bool> IsNameUniqueForCreateAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            name = name.ReduceExcessSpace().Trim();
            var exist = await ExistsAsync(a => a.Name == name).ConfigureAwait(false);
            return !exist;
        }

        public async Task<bool> IsNameUniqueForUpdateAsync(string name, Guid uId)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            name = name.ReduceExcessSpace().Trim();
            var exist = await ExistsAsync(a => a.Uid != uId && a.Name == name).ConfigureAwait(false);
            return !exist;
        }

        public async Task SaveChanges()
        {
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}

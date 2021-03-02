using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ImagineCommunications.GamePlan.Domain.Generic.DbContext
{
    public interface IDbContext : IDisposable
    {
        TEntity Add<TEntity>(TEntity entity) where TEntity : class;

        object Add(object entity);

        Task<TEntity> AddAsync<TEntity>(TEntity entity) where TEntity : class;

        Task<object> AddAsync(object entity);

        void AddRange<TEntity>(params TEntity[] entities) where TEntity : class;

        void AddRange(params object[] entities);

        void AddRange(IEnumerable<object> entities);

        Task AddRangeAsync<TEntity>(params TEntity[] entities) where TEntity : class;

        Task AddRangeAsync(params object[] entities);

        Task AddRangeAsync(IEnumerable<object> entities);

        void Remove<TEntity>(TEntity entity) where TEntity : class;

        void Remove(object entity);

        void RemoveRange<TEntity>(params TEntity[] entities) where TEntity : class;

        void RemoveRange(params object[] entities);

        void RemoveRange(IEnumerable<object> entities);

        TEntity Update<TEntity>(TEntity entity) where TEntity : class;

        object Update(object entity);

        void UpdateRange<TEntity>(params TEntity[] entities) where TEntity : class;

        void UpdateRange(params object[] entities);

        void UpdateRange(IEnumerable<object> entities);

        TEntity Find<TEntity>(params object[] ids) where TEntity : class;

        object Find(Type entityType, params object[] ids);

        Task<TEntity> FindAsync<TEntity>(params object[] ids) where TEntity : class;

        Task<object> FindAsync(Type entityType, params object[] ids);

        IQueryable<TEntity> Query<TEntity>() where TEntity : class;

        void Truncate<TEntity>() where TEntity : class;

        void Truncate(Type entityType);

        Task TruncateAsync<TEntity>() where TEntity : class;

        Task TruncateAsync(Type entityType);

        void SaveChanges();

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    public interface IDbContext<out TSpecificDbAdapter> : IDbContext
        where TSpecificDbAdapter : class
    {
        TSpecificDbAdapter Specific { get; }
    }
}

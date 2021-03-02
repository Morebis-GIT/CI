using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Generic.Interfaces
{
    /// <summary>
    /// Exposes the db entity cache functionality.
    /// </summary>
    public interface IEntityCache<TKey, TEntity> : IEntityCacheAccessor<TKey, TEntity>, IEnumerable<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Gets the entity by the specified key or add it by calling factory method.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="factory">The factory.</param>
        /// <returns></returns>
        TEntity GetOrAdd(TKey key, Func<TKey, TEntity> factory = null);

        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Add(TEntity entity);

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        bool Remove(TEntity entity);

        /// <summary>
        /// Loads <see cref="TEntity"/> entities from database.
        /// </summary>
        void Load();
    }
}

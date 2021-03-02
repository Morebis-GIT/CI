namespace ImagineCommunications.GamePlan.Domain.Generic.Interfaces
{
    /// <summary>
    /// Exposes the db entity cache functionality.
    /// </summary>
    public interface IEntityCacheAccessor<in TKey, out TEntity> where TEntity : class
    {
        /// <summary>
        /// Gets the entity by the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        TEntity Get(TKey key);
    }
}

using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Shared.Metadatas
{
    /// <summary>
    /// Meta data store Repository
    /// </summary>
    public interface IMetadataRepository
    {
        /// <summary>
        /// Get all values
        /// </summary>
        /// <returns></returns>
        MetadataModel GetAll();

        /// <summary>
        /// Add new metadata
        /// </summary>
        /// <param name="metadata"></param>
        void Add(MetadataModel metadata);

        /// <summary>
        /// Get metadata by Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        List<Data> GetByKey(MetaDataKeys key);

        MetadataModel GetByKeys(List<MetaDataKeys> keys);

        /// <summary>
        /// Remove metadata by key
        /// </summary>
        /// <param name="key"></param>
        void DeleteByKey(MetaDataKeys key);

        void SaveChanges();
    }
}

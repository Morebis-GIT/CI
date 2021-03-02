using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    /// <summary>
    /// Repository class to store metadata in RavenDB
    /// </summary>
    public class RavenMetadataRepository : IMetadataRepository
    {
        private readonly IDocumentSession _session;

        /// <summary>
        /// Load Raven session value
        /// </summary>
        /// <param name="session">Raven DocumentSession</param>
        public RavenMetadataRepository(IDocumentSession session)
        {
            _session = session;
        }

        /// <summary>
        /// Get all metadata from DB
        /// </summary>
        /// <returns></returns>
        public MetadataModel GetAll() =>
            _session.Query<Metadata>().FirstOrDefault()?.ToMetadataModel();

        /// <summary>
        /// Add new entry in metadata
        /// </summary>
        /// <param name="metadataModel">metadata item</param>
        public void Add(MetadataModel metadataModel)
        {
            var metadata = _session.Query<Metadata>().FirstOrDefault() ?? GetDefaultMetadata();

            metadataModel.ApplyToMetadata(metadata);
            _session.Store(metadata);
        }

        /// <summary>
        /// Get metadata by key
        /// </summary>
        /// <param name="key">metadata key</param>
        /// <returns></returns>
        public List<Data> GetByKey(MetaDataKeys key)
        {
            List<Data> value = null;
            GetAll()?.TryGetValue(key, out value);
            return value;
        }

        public MetadataModel GetByKeys(List<MetaDataKeys> keys)
        {
            var result = GetAll()?.Where(pair => keys.Contains(pair.Key)).ToDictionary(k => k.Key, v => v.Value);
            return result == null ? null : new MetadataModel(result);
        }

        public void DeleteByKey(MetaDataKeys key)
        {
            var metadataModel = GetAll();
            if(metadataModel.ContainsKey(key))
            {
                metadataModel[key] = new List<Data>();
            }
            var metadata = _session.Query<Metadata>().FirstOrDefault() ?? GetDefaultMetadata();
            metadataModel.ApplyToMetadata(metadata);
            _session.Store(metadata);
        }

        public void SaveChanges() => _session.SaveChanges();

        private Metadata GetDefaultMetadata()
        {
            return new Metadata
            {
                Dictionary = new Dictionary<MetaDataKeys, string>()
            };
        }
    }
}

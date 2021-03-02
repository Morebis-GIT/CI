using System.Collections.Generic;
using System.Linq;
using Raven.Imports.Newtonsoft.Json;

namespace ImagineCommunications.GamePlan.Domain.Shared.Metadatas
{
    public static class Extensions
    {
        public static MetadataModel ToMetadataModel(this Metadata metadata)
        {
            return metadata?.Dictionary == null
                ? null
                : new MetadataModel(
                    metadata.Dictionary.ToDictionary(k => k.Key,
                        v => JsonConvert.DeserializeObject<List<Data>>(v.Value)));
        }

        public static void ApplyToMetadata(this MetadataModel metadataModel, Metadata metadata)
        {
            if (metadata?.Dictionary == null || metadataModel == null)
            {
                return;
            }

            foreach (var pair in metadataModel)
            {
                var value = JsonConvert.SerializeObject(pair.Value);
                if (metadata.Dictionary.ContainsKey(pair.Key))
                {
                    metadata.Dictionary[pair.Key] = value;
                }
                else
                {
                    metadata.Dictionary.Add(pair.Key, value);
                }
            }

            var keys = metadata.Dictionary.Keys.ToList();
            foreach (var key in keys)
            {
                if (!metadataModel.ContainsKey(key))
                {
                    _ = metadata.Dictionary.Remove(key);
                }
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Metadatas;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping.TypeConverters.MetadataConverters
{
    public class MetadataModelToEnumerableMetadataCategoryTypeConverter : ITypeConverter<MetadataModel, IEnumerable<MetadataCategory>>
    {
        public IEnumerable<MetadataCategory> Convert(MetadataModel source, IEnumerable<MetadataCategory> destination, ResolutionContext context) =>
            source.Select(pair => new MetadataCategory
            {
                Name = pair.Key.ToString(),
                MetadataValues = context.Mapper.Map<List<MetadataValue>>(pair.Value)
            });
    }
}

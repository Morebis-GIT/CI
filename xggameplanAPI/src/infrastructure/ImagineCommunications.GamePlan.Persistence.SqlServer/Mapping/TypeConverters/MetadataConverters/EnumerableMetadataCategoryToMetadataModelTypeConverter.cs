using System;
using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Metadatas;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping.TypeConverters.MetadataConverters
{
    public class EnumerableMetadataCategoryToMetadataModelTypeConverter : ITypeConverter<IEnumerable<MetadataCategory>, MetadataModel>
    {
        public MetadataModel Convert(IEnumerable<MetadataCategory> source, MetadataModel destination, ResolutionContext context)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var metadataModel = new MetadataModel();
            foreach (var pair in source)
            {
                metadataModel.Add((MetaDataKeys)Enum.Parse(typeof(MetaDataKeys), pair.Name),
                    context.Mapper.Map<List<Data>>(pair.MetadataValues));
            }

            return metadataModel;
        }
    }
}

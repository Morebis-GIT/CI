using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Metadatas;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping.TypeConverters.MetadataConverters;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class MetadataProfile : Profile
    {
        public MetadataProfile()
        {
            CreateMap<Data, MetadataValue>()
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ValueId, opt => opt.MapFrom(s => s.Id))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(s => s.ValueId));

            CreateMap<MetadataModel, IEnumerable<MetadataCategory>>()
                .ConvertUsing<MetadataModelToEnumerableMetadataCategoryTypeConverter>();
            CreateMap<IEnumerable<MetadataCategory>, MetadataModel>()
                .ConvertUsing<EnumerableMetadataCategoryToMetadataModelTypeConverter>();
        }
    }
}

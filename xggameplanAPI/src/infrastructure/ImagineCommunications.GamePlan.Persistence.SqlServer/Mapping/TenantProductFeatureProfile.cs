using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.System.Features;
using TenantProductFeatureEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.TenantProductFeatures.TenantProductFeature;
using TenantProductFeatureReferenceEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.TenantProductFeatures.TenantProductFeatureReference;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class TenantProductFeatureProfile : Profile
    {
        public TenantProductFeatureProfile()
        {
            CreateMap<TenantProductFeatureEntity, TenantProductFeature>()
            .ForMember(d => d.ParentIds, o => o.MapFrom(src => src.ParentFeatures.Select(x => x.TenantProductFeatureParentId)))
            .ReverseMap()
            .ForMember(d => d.ParentFeatures, o => o.MapFrom(src =>
                src.ParentIds == null ?
                    new List<TenantProductFeatureReferenceEntity>() :
                    src.ParentIds.Select(x => new TenantProductFeatureReferenceEntity
                    {
                        TenantProductFeatureParentId = x,
                        TenantProductFeatureChildId = src.Id
                    })));
        }
    }
}

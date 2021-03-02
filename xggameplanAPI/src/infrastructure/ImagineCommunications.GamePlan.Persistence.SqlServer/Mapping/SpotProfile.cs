using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Dto;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using xggameplan.core.Extensions.AutoMapper;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class SpotProfile : Profile
    {
        public SpotProfile()
        {
            _ = CreateMap<Spot, Domain.Spots.Spot>()
                .ForMember(dest => dest.CustomId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SalesArea,
                    opts => opts.FromEntityCache(src => src.SalesAreaId,
                        s => s.Entity<SalesArea>(x => x.Name).CheckNavigationPropertyFirst(x => x.SalesArea)))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.CustomId))
                .ForMember(dest => dest.SalesArea, o => o.Ignore())
                .ForMember(dest => dest.SalesAreaId,
                    o => o.FromEntityCache(src => src.SalesArea, opts => opts.Entity<SalesArea>(x => x.Id)));

            _ = CreateMap<Spot, ReducedSpotDTO>();
        }
    }
}

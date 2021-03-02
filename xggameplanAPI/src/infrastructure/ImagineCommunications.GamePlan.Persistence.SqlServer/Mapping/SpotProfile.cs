using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Dto;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class SpotProfile : Profile
    {
        public SpotProfile()
        {
            CreateMap<Spot, Domain.Spots.Spot>()
                .ForMember(dest => dest.CustomId, opt => opt.MapFrom(src => src.Id))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.CustomId));

            CreateMap<Spot, ReducedSpotDTO>();
        }
    }
}

using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class SpotPlacementProfile : Profile
    {
        public SpotPlacementProfile()
        {
            CreateMap<SpotPlacement, Domain.SpotPlacements.SpotPlacement>().ReverseMap();
        }
    }
}

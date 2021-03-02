using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class UniverseProfile : Profile
    {
        public UniverseProfile()
        {
            CreateMap<Universe, Domain.Shared.Universes.Universe>().ReverseMap();
        }
    }
}

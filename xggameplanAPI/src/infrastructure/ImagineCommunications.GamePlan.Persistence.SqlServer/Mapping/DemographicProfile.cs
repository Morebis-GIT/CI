using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class DemographicProfile : Profile
    {
        public DemographicProfile()
        {
            CreateMap<Demographic, Domain.Shared.Demographics.Demographic>().ReverseMap();
        }
    }
}

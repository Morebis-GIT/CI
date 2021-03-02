using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class SalesAreaDemographicProfile : Profile
    {
        public SalesAreaDemographicProfile()
        {
            CreateMap<SalesAreaDemographic, Domain.Shared.SalesAreaDemographics.SalesAreaDemographic>().ReverseMap();
        }
    }
}

using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using xggameplan.core.Extensions.AutoMapper;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class SalesAreaDemographicProfile : Profile
    {
        public SalesAreaDemographicProfile()
        {
            _ = CreateMap<SalesAreaDemographic, Domain.Shared.SalesAreaDemographics.SalesAreaDemographic>()
               .ForMember(d => d.SalesArea,
                   opts => opts.FromEntityCache(src => src.SalesAreaId,
                       s => s.Entity<SalesArea>(x => x.Name).CheckNavigationPropertyFirst(x => x.SalesArea)))
               .ReverseMap()
               .ForMember(d => d.SalesArea, o => o.Ignore())
               .ForMember(d => d.SalesAreaId,
                   o => o.FromEntityCache(src => src.SalesArea, opts => opts.Entity<SalesArea>(x => x.Id)));
        }
    }
}

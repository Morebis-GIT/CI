using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using xggameplan.core.Extensions.AutoMapper;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class SalesAreaPriorityProfile : Profile
    {
        public SalesAreaPriorityProfile()
        {
            _ = CreateMap<SalesAreaPriority, Domain.Shared.System.Models.SalesAreaPriority>()
                .ForMember(x => x.Priority,
                    opt => opt.MapFrom(x => (Domain.Shared.System.Models.SalesAreaPriorityType)(int)x.Priority))
                .ForMember(dest => dest.SalesArea,
                    opt => opt.FromEntityCache(x => x.SalesAreaId,
                        opts => opts.Entity<SalesArea>(x => x.Name).CheckNavigationPropertyFirst(x => x.SalesArea)))
                .ReverseMap()
                .ForMember(e => e.Id, opt => opt.Ignore())
                .ForMember(d => d.SalesArea, o => o.Ignore())
                .ForMember(d => d.SalesAreaId,
                    o => o.FromEntityCache(src => src.SalesArea, opts => opts.Entity<SalesArea>(x => x.Id)));
        }
    }
}

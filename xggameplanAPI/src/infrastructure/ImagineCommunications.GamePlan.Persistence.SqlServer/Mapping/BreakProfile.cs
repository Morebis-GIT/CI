using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Breaks;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using xggameplan.core.Extensions.AutoMapper;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class BreakProfile : Profile
    {
        public BreakProfile()
        {
            _ = CreateMap<Break, Domain.Breaks.Objects.Break>()
                .ForMember(d => d.PositionInProg, o => o.MapFrom(src => (BreakPosition)src.PositionInProg))
                .ForMember(d => d.BreakEfficiencyList, o => o.MapFrom(src => src.BreakEfficiencies))
                .ForMember(d => d.SalesArea,
                    opts => opts.FromEntityCache(src => src.SalesAreaId,
                        s => s.Entity<SalesArea>(x => x.Name).CheckNavigationPropertyFirst(x => x.SalesArea)))
                .ReverseMap()
                .ForMember(d => d.BreakEfficiencies, o => o.MapFrom(src => src.BreakEfficiencyList))
                .ForMember(d => d.SalesArea, o => o.Ignore())
                .ForMember(d => d.SalesAreaId,
                    o => o.FromEntityCache(src => src.SalesArea, opts => opts.Entity<SalesArea>(x => x.Id)));

            _ = CreateMap<BreakEfficiency, Domain.Breaks.Objects.BreakEfficiency>().ReverseMap();

            _ = CreateMap<Break, Domain.Breaks.Objects.BreakSimple>();
        }
    }
}

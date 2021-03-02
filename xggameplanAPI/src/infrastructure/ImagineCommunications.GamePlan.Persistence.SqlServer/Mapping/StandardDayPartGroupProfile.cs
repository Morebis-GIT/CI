using AutoMapper;
using ImagineCommunications.GamePlan.Domain.DayParts.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using xggameplan.core.Extensions.AutoMapper;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class StandardDayPartGroupProfile : Profile
    {
        public StandardDayPartGroupProfile()
        {
            _ = CreateMap<StandardDayPartGroup, Entities.Tenant.DayParts.StandardDayPartGroup>()
                .ForMember(d => d.Splits, o => o.MapFrom(src => src.Splits))
                .ForMember(d => d.SalesArea, o => o.Ignore())
                .ForMember(d => d.SalesAreaId,
                    o => o.FromEntityCache(src => src.SalesArea, opts => opts.Entity<SalesArea>(x => x.Id)))
                .ReverseMap()
                .ForMember(d => d.SalesArea,
                    opts => opts.FromEntityCache(src => src.SalesAreaId,
                    s => s.Entity<SalesArea>(x => x.Name).CheckNavigationPropertyFirst(x => x.SalesArea)));

            _ = CreateMap<StandardDayPartSplit, Entities.Tenant.DayParts.StandardDayPartSplit>().ReverseMap();
        }
    }
}

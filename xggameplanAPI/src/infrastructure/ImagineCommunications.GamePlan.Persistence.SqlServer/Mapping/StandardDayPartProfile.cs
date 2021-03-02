using AutoMapper;
using ImagineCommunications.GamePlan.Domain.DayParts.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using xggameplan.core.Extensions.AutoMapper;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class StandardDayPartProfile : Profile
    {
        public StandardDayPartProfile()
        {
            _ = CreateMap<StandardDayPart, Entities.Tenant.DayParts.StandardDayPart>()
                .ForMember(d => d.Timeslices, o => o.MapFrom(src => src.Timeslices))
                .ForMember(d => d.SalesArea, o => o.Ignore())
                .ForMember(d => d.SalesAreaId,
                    o => o.FromEntityCache(src => src.SalesArea, opts => opts.Entity<SalesArea>(x => x.Id)))
                .ReverseMap()
                .ForMember(d => d.SalesArea,
                    opts => opts.FromEntityCache(src => src.SalesAreaId,
                    s => s.Entity<SalesArea>(x => x.Name).CheckNavigationPropertyFirst(x => x.SalesArea)));

            _ = CreateMap<StandardDayPartTimeslice, Entities.Tenant.DayParts.StandardDayPartTimeslice>()
                .ReverseMap();
        }
    }
}

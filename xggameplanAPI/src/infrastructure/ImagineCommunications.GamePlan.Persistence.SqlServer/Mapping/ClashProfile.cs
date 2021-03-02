using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using xggameplan.core.Extensions.AutoMapper;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping.Clashes
{
    public class ClashProfile : Profile
    {
        public ClashProfile()
        {
            _ = CreateMap<Clash, Domain.BusinessRules.Clashes.Objects.Clash>().ReverseMap();
            _ = CreateMap<Clash, Domain.BusinessRules.Clashes.Objects.ClashNameModel>().ReverseMap();

            _ = CreateMap<ClashDifference, Domain.BusinessRules.Clashes.Objects.ClashDifference>()
                .ForMember(d => d.SalesArea,
                    opts => opts.FromEntityCache(src => src.SalesAreaId,
                        s => s.Entity<SalesArea>(x => x.Name).CheckNavigationPropertyFirst(x => x.SalesArea)))
                .ReverseMap()
                .ForMember(d => d.SalesArea, o => o.Ignore())
                .ForMember(d => d.SalesAreaId,
                    o => o.FromEntityCache(src => src.SalesArea, opts => opts.Entity<SalesArea>(x => x.Id)));

            _ = CreateMap<Domain.Generic.Types.TimeAndDowAPI, ClashDifferenceTimeAndDow>()
                .ForMember(d => d.DaysOfWeek, opt => opt.MapFrom(s => s.DaysOfWeekBinary));

            _ = CreateMap<ClashDifferenceTimeAndDow, Domain.Generic.Types.TimeAndDowAPI>()
                .ConstructUsing((ClashDifferenceTimeAndDow x) => new TimeAndDowAPI());
        }
    }
}

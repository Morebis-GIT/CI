using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using xggameplan.core.Extensions.AutoMapper;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class TotalRatingProfile : Profile
    {
        public TotalRatingProfile()
        {
            _ = CreateMap<TotalRating, Domain.TotalRatings.TotalRating>()
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

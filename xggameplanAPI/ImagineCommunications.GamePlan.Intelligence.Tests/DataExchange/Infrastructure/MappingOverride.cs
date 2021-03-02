using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using xggameplan.core.Extensions.AutoMapper;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure
{
    public class MappingOverride : Profile
    {
        public MappingOverride()
        {
            _ = CreateMap<Universe, Domain.Shared.Universes.Universe>()
                .ForMember(d => d.SalesArea,
                    opts => opts.FromEntityCache(src => src.SalesAreaId,
                        s => s.Entity<SalesArea>(x => x.Name).CheckNavigationPropertyFirst(x => x.SalesArea)))
                .ReverseMap()
                .ForMember(d => d.SalesArea,
                    o => o.FromEntityCache(src => src.SalesArea, opts => opts.Entity<SalesArea>()))
                .ForMember(d => d.SalesAreaId,
                    o => o.FromEntityCache(src => src.SalesArea, opts => opts.Entity<SalesArea>(x => x.Id)));
        }
    }
}

using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SpotBookingRules;
using xggameplan.core.Extensions.AutoMapper;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class SpotBookingRuleProfile : Profile
    {
        public SpotBookingRuleProfile()
        {
            
            CreateMap<string, SpotBookingRuleSalesArea>()
                .ForMember(dest => dest.SalesAreaId,
                    opts => opts.FromEntityCache(opt => opt.Entity<SalesArea>(x => x.Id)))
                .ReverseMap()
                .FromEntityCache(x => x.SalesAreaId, opt => opt.Entity<SalesArea>(x => x.Name));

            _ = CreateMap<Domain.SpotBookingRules.SpotBookingRule, SpotBookingRule>()
                .ForMember(dest => dest.SalesAreas,
                    opt =>
                    {
                        opt.PreCondition(s => (s.SalesAreas != null));
                        opt.MapFrom(x => x.SalesAreas);
                    })
                .ReverseMap()
                .ForMember(dest => dest.SalesAreas,
                    opt =>
                    {
                        opt.PreCondition(s => s.SalesAreas != null);
                        opt.MapFrom(x => x.SalesAreas);
                    });
        }
    }
}

using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SpotBookingRules;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class SpotBookingRuleProfile : Profile
    {
        public SpotBookingRuleProfile()
        {
            CreateMap<Domain.SpotBookingRules.SpotBookingRule, SpotBookingRule>()
                .ForMember(dest => dest.SalesAreas,
                    opt =>
                    {
                        opt.PreCondition(s => (s.SalesAreas != null));
                        opt.MapFrom(s => s.SalesAreas.Select(sa => new SpotBookingRuleSalesArea() { Name = sa }));
                    })
                .ReverseMap()
                .ForMember(dest => dest.SalesAreas,
                    opt =>
                    {
                        opt.PreCondition(s => s.SalesAreas != null);
                        opt.MapFrom(s => s.SalesAreas.Select(sa => sa.Name));
                    });
        }
    }
}

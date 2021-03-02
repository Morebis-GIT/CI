using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Rules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class RuleProfile : Profile
    {
        public RuleProfile()
        {
            CreateMap<Rule, Entities.Tenant.Rule>()
                .ForMember(dest => dest.CampaignType, opt => opt.MapFrom(src => (CampaignDeliveryType?)src.CampaignType))
                .ReverseMap()
                .ForMember(dest => dest.CampaignType, opt => opt.MapFrom(src => (Domain.Campaigns.CampaignDeliveryType?)src.CampaignType));
        }
    }
}

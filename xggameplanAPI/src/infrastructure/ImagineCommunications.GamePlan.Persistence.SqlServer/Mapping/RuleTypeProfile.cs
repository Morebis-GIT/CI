using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BusinessRules.RuleTypes;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class RuleTypeProfile : Profile
    {
        public RuleTypeProfile()
        {
            CreateMap<RuleType, Entities.Tenant.RuleType>().ReverseMap();
        }
    }
}

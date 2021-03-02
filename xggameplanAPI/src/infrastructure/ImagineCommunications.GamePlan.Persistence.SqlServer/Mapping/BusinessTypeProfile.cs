using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BusinessTypes;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class BusinessTypeProfile : Profile
    {
        public BusinessTypeProfile()
        {
            CreateMap<BusinessType, Domain.BusinessTypes.Objects.BusinessType>().ReverseMap();
        }
    }
}

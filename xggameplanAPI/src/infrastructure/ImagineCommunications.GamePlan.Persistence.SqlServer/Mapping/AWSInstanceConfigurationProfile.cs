using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class AWSInstanceConfigurationProfile : Profile
    {
        public AWSInstanceConfigurationProfile()
        {
            CreateMap<AWSInstanceConfiguration, Domain.AWSInstanceConfigurations.AWSInstanceConfiguration>().ReverseMap();
        }
    }
}

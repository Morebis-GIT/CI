using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BRS;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class BRSProfile : AutoMapper.Profile
    {
        public BRSProfile()
        {
            CreateMap<BRSConfigurationTemplate, Domain.BRS.BRSConfigurationTemplate>().ReverseMap();
            CreateMap<BRSConfigurationForKPI, Domain.BRS.BRSConfigurationForKPI>().ReverseMap();
            CreateMap<KPIPriority, Domain.BRS.KPIPriority>().ReverseMap();
        }
    }
}

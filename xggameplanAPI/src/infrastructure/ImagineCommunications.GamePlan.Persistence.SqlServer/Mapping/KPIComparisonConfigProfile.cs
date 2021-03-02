using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class KPIComparisonConfigProfile : Profile
    {
        public KPIComparisonConfigProfile()
        {
            CreateMap<KPIComparisonConfig, Domain.KPIComparisonConfigs.KPIComparisonConfig>().ReverseMap();
        }
    }
}

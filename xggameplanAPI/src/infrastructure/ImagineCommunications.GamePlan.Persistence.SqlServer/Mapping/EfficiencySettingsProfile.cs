using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class EfficiencySettingsProfile : Profile
    {
        public EfficiencySettingsProfile()
        {
            CreateMap<EfficiencySettings, Domain.EfficiencySettings.EfficiencySettings>().ReverseMap();
        }
    }
}

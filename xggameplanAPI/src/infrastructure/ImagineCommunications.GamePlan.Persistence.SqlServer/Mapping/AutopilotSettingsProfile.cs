using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class AutopilotSettingsProfile : Profile
    {
        public AutopilotSettingsProfile()
        {
            CreateMap<AutopilotSettings, Domain.Autopilot.Settings.AutopilotSettings>().ReverseMap();
        }
    }
}

using AutoMapper;
using ImagineCommunications.GamePlan.Domain.RunTypes.Objects;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class RunLandmarkScheduleSettingsProfile : Profile
    {
        public RunLandmarkScheduleSettingsProfile()
        {
            CreateMap<RunLandmarkScheduleSettings, Entities.Tenant.Runs.RunLandmarkScheduleSettings>().ReverseMap();
        }
    }
}

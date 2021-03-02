using ImagineCommunications.GamePlan.Domain.RunTypes.Objects;
using xggameplan.model.External;
using xggameplan.Model;

namespace xggameplan.Profile
{
    public class RunTypeProfile : AutoMapper.Profile
    {
        public RunTypeProfile()
        {
            CreateMap<CreateRunTypeAnalysisGroupModel, RunTypeAnalysisGroup>();
            CreateMap<CreateRunTypeModel, RunType>();
            CreateMap<RunTypeAnalysisGroupModel, RunTypeAnalysisGroup>().ReverseMap();
            CreateMap<RunTypeModel, RunType>().ReverseMap();
            CreateMap<CreateRunLandmarkScheduleSettingsModel, RunLandmarkScheduleSettings>().ReverseMap();
            CreateMap<RunLandmarkScheduleSettingsModel, RunLandmarkScheduleSettings>().ReverseMap();
        }
    }
}

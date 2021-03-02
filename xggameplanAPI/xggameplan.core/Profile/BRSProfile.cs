using ImagineCommunications.GamePlan.Domain.BRS;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.model.External;

namespace xggameplan.core.Profile
{
    public class BRSProfile : AutoMapper.Profile
    {
        public BRSProfile()
        {
            CreateMap<BRSConfigurationTemplate, BRSConfigurationTemplateModel>().ReverseMap();
            CreateMap<BRSConfigurationTemplate, CreateOrUpdateBRSConfigurationTemplateModel>().ReverseMap();
            CreateMap<BRSConfigurationForKPI, BRSConfigurationForKPIModel>().ReverseMap();
            CreateMap<ScenarioResult, BRSIndicatorValueForScenarioResultModel>()
                .ForMember(x => x.ScenarioId, opt => opt.MapFrom(y => y.Id));
        }
    }
}

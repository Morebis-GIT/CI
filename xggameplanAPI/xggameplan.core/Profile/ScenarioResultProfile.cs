using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.Model;

namespace xggameplan.Profile
{
    public class ScenarioResultProfile : AutoMapper.Profile
    {
        public ScenarioResultProfile()
        {
            CreateMap<ScenarioResult, ScenarioResultModel>();
            CreateMap<ScenarioResult, ScenarioMetricsResultModel>()
                .ForMember(srm => srm.ScenarioId, expression => expression.MapFrom(sr => sr.Id));

            CreateMap<KPI, KPIModel>();
            CreateMap<AnalysisGroupTargetMetric, AnalysisGroupTargetMetricModel>();
        }
    }
}

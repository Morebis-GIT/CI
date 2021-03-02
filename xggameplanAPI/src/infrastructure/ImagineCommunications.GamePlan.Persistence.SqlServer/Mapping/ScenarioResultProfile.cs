using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignMetrics.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioResults;
using AnalysisGroupTargetMetric = ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects.AnalysisGroupTargetMetric;
using AnalysisGroupTargetMetricEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioResults.AnalysisGroupTargetMetric;
using ScenarioCampaignMetricEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioResults.ScenarioCampaignMetric;
using ScenarioCampaignResult = ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects.ScenarioCampaignResult;
using ScenarioCampaignResultEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioResults.ScenarioCampaignResult;
using ScenarioResult = ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects.ScenarioResult;
using ScenarioResultEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioResults.ScenarioResult;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class ScenarioResultProfile : Profile
    {
        public ScenarioResultProfile()
        {
            CreateMap<ScenarioResult, ScenarioResultEntity>()
                .ForMember(dest => dest.ScenarioId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ScenarioId));

            CreateMap<KPI, ScenarioResultMetric>()
                .ForMember(dest => dest.DisplayFormat, opt => opt.MapFrom(src => src.Displayformat))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ScenarioResultId, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.Displayformat, opt => opt.MapFrom(src => src.DisplayFormat));

            CreateMap<KPI, GameplanScenarioResultMetric>()
                .ForMember(dest => dest.DisplayFormat, opt => opt.MapFrom(src => src.Displayformat))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ScenarioResultId, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.Displayformat, opt => opt.MapFrom(src => src.DisplayFormat));

            CreateMap<KPI, LandmarkScenarioResultMetric>()
                .ForMember(dest => dest.DisplayFormat, opt => opt.MapFrom(src => src.Displayformat))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ScenarioResultId, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.Displayformat, opt => opt.MapFrom(src => src.DisplayFormat));

            CreateMap<AnalysisGroupTargetMetric, AnalysisGroupTargetMetricEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ScenarioResultId, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<ScenarioCampaignResultItem, ScenarioCampaignResultEntity>()
                .ReverseMap();

            CreateMap<ScenarioCampaignResult, List<ScenarioCampaignResultEntity>>().ConvertUsing((model, d, rc) =>
            {
                var entities = rc.Mapper.Map<List<ScenarioCampaignResultEntity>>(model.Items);
                entities.ForEach(e => e.ScenarioId = model.Id);

                return entities;
            });

            CreateMap<List<ScenarioCampaignResultEntity>, ScenarioCampaignResult>().ConvertUsing((entities, d, rc) =>
            {
                if (!entities.Any())
                {
                    return null;
                }

                return new ScenarioCampaignResult
                {
                    Id = entities.First().ScenarioId,
                    Items = rc.Mapper.Map<List<ScenarioCampaignResultItem>>(entities)
                };
            });

            CreateMap<ScenarioCampaignResultItem, ScenarioCampaignExtendedResultItem>();

            CreateMap<ScenarioCampaignLevelResultItem, ScenarioCampaignExtendedResultItem>();

            CreateMap<ScenarioCampaignMetricItem, ScenarioCampaignMetricEntity>()
                .ReverseMap();

            CreateMap<Domain.ScenarioCampaignMetrics.Objects.ScenarioCampaignMetric, List<ScenarioCampaignMetricEntity>>().ConvertUsing((model, d, rc) =>
            {
                var entities = rc.Mapper.Map<List<ScenarioCampaignMetricEntity>>(model.Metrics);
                entities.ForEach(e => e.ScenarioId = model.Id);

                return entities;
            });

            CreateMap<List<ScenarioCampaignMetricEntity>, Domain.ScenarioCampaignMetrics.Objects.ScenarioCampaignMetric>().ConvertUsing((entities, d, rc) =>
            {
                if (!entities.Any())
                {
                    return null;
                }

                return new Domain.ScenarioCampaignMetrics.Objects.ScenarioCampaignMetric
                {
                    Id = entities.First().ScenarioId,
                    Metrics = rc.Mapper.Map<List<ScenarioCampaignMetricItem>>(entities)
                };
            });
        }
    }
}

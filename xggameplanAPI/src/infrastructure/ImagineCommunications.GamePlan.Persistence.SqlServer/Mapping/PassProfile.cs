using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Passes.Queries;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios;
using Pass = ImagineCommunications.GamePlan.Domain.Passes.Objects.Pass;
using PassEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes.Pass;
using PassRule = ImagineCommunications.GamePlan.Domain.Passes.Objects.PassRule;
using PassRuleEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes.PassRule;
using RatingPointEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes.PassRatingPoint;
using PassSalesAreaPriority = ImagineCommunications.GamePlan.Domain.Passes.Objects.PassSalesAreaPriority;
using SalesAreaPriorityType = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.SalesAreaPriorityType;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class PassProfile : Profile
    {
        public PassProfile()
        {
            CreateMap<Pass, PassEntity>()
                .ReverseMap();
            CreateMap<PassSalesAreaPriority, PassSalesAreaPriorityCollection>()
                .ReverseMap();
            CreateMap<General, PassRuleGeneral>()
                .ReverseMap();
            CreateMap<Weighting, PassRuleWeighting>()
                .ReverseMap();
            CreateMap<PassEntity, PassDigestListItem>()
                .ReverseMap();
            CreateMap<ScenarioPassReference, PassDigestListItem>()
                .ForMember(digest => digest.Id, scenarioPassReference => scenarioPassReference.MapFrom(s => s.PassId))
                .ReverseMap()
                .ForMember(scenarioPassReference => scenarioPassReference.PassId, digest => digest.MapFrom(d => d.Id));

            CreateMap<Tolerance, PassRuleTolerance>()
                .ForMember(dest => dest.ForceOverUnder, opt => opt.MapFrom(src => (ForceOverUnder)src.ForceOverUnder))
                .ForMember(dest => dest.CampaignType, opt => opt.MapFrom(src => (CampaignDeliveryType)src.CampaignType))
                .ReverseMap()
                .ForMember(dest => dest.ForceOverUnder, opt => opt.MapFrom(src => (Domain.Passes.ForceOverUnder)src.ForceOverUnder))
                .ForMember(dest => dest.CampaignType, opt => opt.MapFrom(src => (Domain.Campaigns.CampaignDeliveryType)src.CampaignType));

            CreateMap<PassRule, PassRuleEntity>()
                .ForMember(dest => dest.CampaignType, opt => opt.MapFrom(src => (CampaignDeliveryType)src.CampaignType))
                .ReverseMap()
                .ForMember(dest => dest.CampaignType, opt => opt.MapFrom(src => (Domain.Campaigns.CampaignDeliveryType)src.CampaignType));
            CreateMap<RatingPoint, RatingPointEntity>()
                .ForMember(e => e.Id, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(x => x.Id, opt => opt.MapFrom(e => e.Id));
            CreateMap<ProgrammeRepetition, PassProgrammeRepetition>()
                .ReverseMap();
            CreateMap<BreakExclusion, PassBreakExclusion>()
                .ReverseMap();
            CreateMap<SlottingLimit, PassSlottingLimit>()
                .ReverseMap();
            CreateMap<SalesAreaPriority, Entities.Tenant.Passes.PassSalesAreaPriority>()
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => (SalesAreaPriorityType)src.Priority))
                .ReverseMap()
                .ForMember(dest => dest.Priority,
                    opt => opt.MapFrom(src => (Domain.Shared.System.Models.SalesAreaPriorityType)src.Priority));
        }
    }
}

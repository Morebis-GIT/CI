using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Passes.Queries;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios;
using xggameplan.core.Extensions.AutoMapper;
using Pass = ImagineCommunications.GamePlan.Domain.Passes.Objects.Pass;
using PassEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes.Pass;
using PassRule = ImagineCommunications.GamePlan.Domain.Passes.Objects.PassRule;
using PassRuleEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes.PassRule;
using PassSalesAreaPriority = ImagineCommunications.GamePlan.Domain.Passes.Objects.PassSalesAreaPriority;
using RatingPointEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes.PassRatingPoint;
using SalesAreaPriorityType = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.SalesAreaPriorityType;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class PassProfile : Profile
    {
        public PassProfile()
        {
            _ = CreateMap<Pass, PassEntity>()
                     .ReverseMap();
            _ = CreateMap<PassSalesAreaPriority, PassSalesAreaPriorityCollection>()
                .ReverseMap();
            _ = CreateMap<General, PassRuleGeneral>()
                .ReverseMap();
            _ = CreateMap<Weighting, PassRuleWeighting>()
                .ReverseMap();
            _ = CreateMap<PassEntity, PassDigestListItem>()
                .ReverseMap();
            _ = CreateMap<ScenarioPassReference, PassDigestListItem>()
                .ForMember(digest => digest.Id, scenarioPassReference => scenarioPassReference.MapFrom(s => s.PassId))
                .ReverseMap()
                .ForMember(scenarioPassReference => scenarioPassReference.PassId, digest => digest.MapFrom(d => d.Id));

            _ = CreateMap<Tolerance, PassRuleTolerance>()
                .ForMember(dest => dest.ForceOverUnder, opt => opt.MapFrom(src => (ForceOverUnder)src.ForceOverUnder))
                .ForMember(dest => dest.CampaignType, opt => opt.MapFrom(src => (CampaignDeliveryType)src.CampaignType))
                .ReverseMap()
                .ForMember(dest => dest.ForceOverUnder, opt => opt.MapFrom(src => (Domain.Passes.ForceOverUnder)src.ForceOverUnder))
                .ForMember(dest => dest.CampaignType, opt => opt.MapFrom(src => (Domain.Campaigns.CampaignDeliveryType)src.CampaignType));

            _ = CreateMap<PassRule, PassRuleEntity>()
                .ForMember(dest => dest.CampaignType, opt => opt.MapFrom(src => (CampaignDeliveryType)src.CampaignType))
                .ReverseMap()
                .ForMember(dest => dest.CampaignType, opt => opt.MapFrom(src => (Domain.Campaigns.CampaignDeliveryType)src.CampaignType));
            CreateMap<string, PassRatingPointSalesAreaRef>()
                .ForMember(
                    dest => dest.SalesAreaId,
                    opts => opts.FromEntityCache(opt => opt.Entity<SalesArea>(x => x.Id)))
                .ReverseMap()
                .FromEntityCache(
                    x => x.SalesAreaId,
                    opt => opt.Entity<SalesArea>(x => x.Name));
            _ = CreateMap<RatingPoint, RatingPointEntity>()
                .ForMember(e => e.Id, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(x => x.Id, opt => opt.MapFrom(e => e.Id));
            _ = CreateMap<ProgrammeRepetition, PassProgrammeRepetition>()
                .ReverseMap();
            _ = CreateMap<BreakExclusion, PassBreakExclusion>()
                .ForMember(dest => dest.SalesArea, opt => opt.Ignore())
                .ForMember(dest => dest.SalesAreaId,
                    opt => opt.FromEntityCache(
                        src => src.SalesArea,
                        opt => opt.Entity<SalesArea>(x => x.Id)
                        )
                    )
                .ReverseMap()
                .ForMember(dest => dest.SalesArea,
                    opt => opt.FromEntityCache(
                        src => src.SalesAreaId,
                        s => s.Entity<SalesArea>(x => x.Name)
                        .CheckNavigationPropertyFirst(x => x.SalesArea)
                        )
                    );
            _ = CreateMap<SlottingLimit, PassSlottingLimit>()
                .ReverseMap();
            _ = CreateMap<SalesAreaPriority, Entities.Tenant.Passes.PassSalesAreaPriority>()
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => (SalesAreaPriorityType)src.Priority))
                .ForMember(dest => dest.SalesArea, opt => opt.Ignore())
                .ForMember(dest => dest.SalesAreaId,
                    opt => opt.FromEntityCache(
                        src => src.SalesArea,
                        opt => opt.Entity<SalesArea>(x => x.Id)
                        )
                    )
                .ReverseMap()
                .ForMember(dest => dest.Priority,
                    opt => opt.MapFrom(src => (Domain.Shared.System.Models.SalesAreaPriorityType)src.Priority))
                .ForMember(dest => dest.SalesArea,
                    opt => opt.FromEntityCache(
                        src => src.SalesAreaId,
                        s => s.Entity<SalesArea>(x => x.Name)
                        .CheckNavigationPropertyFirst(x => x.SalesArea)
                        )
                    );
        }
    }
}

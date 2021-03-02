using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Queries;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities;
using ScenarioCampaignPassPriorityEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios.ScenarioCampaignPassPriority;
using ScenarioCampaignPriorityRoundCollectionEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios.ScenarioCampaignPriorityRoundCollection;
using ScenarioCampaignPriorityRoundEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios.ScenarioCampaignPriorityRound;
using ScenarioCompactCampaignEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios.ScenarioCompactCampaign;
using ScenarioCompactCampaignPaybackEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios.ScenarioCompactCampaignPayback;
using ScenarioEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios.Scenario;
using ScenarioPassPriorityEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios.ScenarioPassPriority;
using ScenarioPassReferenceEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios.ScenarioPassReference;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class ScenarioProfile : Profile
    {
        public ScenarioProfile()
        {
            CreateMap<ScenarioEntity, Scenario>()
                .ForMember(x => x.Passes, opt => opt.MapFrom(e => e.PassReferences.OrderBy(x => x.Order)))
                .ReverseMap()
                .ForMember(e => e.PassReferences, opt => opt.MapFrom(x => x.Passes));

            CreateMap<ScenarioEntity, ScenarioDigestListItem>()
                .ForMember(digest => digest.Passes, entity => entity.MapFrom(e => e.PassReferences))
                .ReverseMap()
                .ForMember(entity => entity.PassReferences, digest => digest.MapFrom(e => e.Passes));

            CreateMap<ScenarioPassReferenceEntity, PassReference>()
                .ForMember(x => x.Id, opt => opt.MapFrom(e => e.PassId))
                .ReverseMap()
                .ForMember(e => e.Id, opt => opt.Ignore())
                .ForMember(e => e.PassId, opt => opt.MapFrom(x => x.Id));

            CreateMap<ScenarioCampaignPriorityRoundCollectionEntity, CampaignPriorityRounds>().ReverseMap();

            CreateMap<ScenarioCampaignPriorityRoundEntity, PriorityRound>().ReverseMap();

            CreateMap<ScenarioCampaignPassPriorityEntity, CampaignPassPriority>().ReverseMap();

            CreateMap<ScenarioCompactCampaignEntity, CompactCampaign>()
                .ForMember(dest => dest.DeliveryType, opt => opt.MapFrom(src => (Domain.Campaigns.CampaignDeliveryType)src.DeliveryType))
                .ForMember(dest => dest.TopTail, opt => opt.MapFrom(src => (Domain.Campaigns.TopTail?)src.TopTail))
                .ForMember(dest => dest.AgencyGroup, opt => opt.MapFrom(src =>
                    src.AgencyGroupCode != null || src.AgencyGroupShortName != null
                        ? new AgencyGroupModel { ShortName = src.AgencyGroupShortName, Code = src.AgencyGroupCode }
                        : null))
                .ForMember(dest => dest.SalesExecutiveName, opt => opt.MapFrom(src => src.SalesExecutiveName))
                .ReverseMap()
                .ForMember(dest => dest.DeliveryType, opt => opt.MapFrom(src => (CampaignDeliveryType)src.DeliveryType))
                .ForMember(dest => dest.TopTail, opt => opt.MapFrom(src => (TopTail?)src.TopTail))
                .ForMember(dest => dest.AgencyGroupShortName, opt => opt.MapFrom(src => src.AgencyGroup == null ? null : src.AgencyGroup.ShortName))
                .ForMember(dest => dest.AgencyGroupCode, opt => opt.MapFrom(src => src.AgencyGroup == null ? null : src.AgencyGroup.Code));

            CreateMap<ScenarioPassPriorityEntity, PassPriority>().ReverseMap();

            CreateMap<ScenarioCompactCampaignPaybackEntity, CampaignPayback>()
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ScenarioCompactCampaignId, opt => opt.Ignore());
        }
    }
}

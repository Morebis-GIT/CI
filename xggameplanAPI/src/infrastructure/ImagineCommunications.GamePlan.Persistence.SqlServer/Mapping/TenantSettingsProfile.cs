using System.Linq;
using AutoMapper;
using TenantSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.TenantSettings.TenantSettings;
using RunRestrictionsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.TenantSettings.RunRestrictions;
using MinimumRunSizeDocumentRestrictionEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.TenantSettings.MinimumRunSizeDocumentRestriction;
using MinimumDocumentRestrictionEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.TenantSettings.MinimumDocumentRestriction;
using RunEventSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.TenantSettings.RunEventSettings;
using HTTPMethodSettings = ImagineCommunications.GamePlan.Domain.Shared.System.Settings.HTTPMethodSettings;
using HTTPNotificationSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.TenantSettings.HTTPNotificationSettings;
using HTTPMethodSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.TenantSettings.HTTPMethodSettings;
using HTTPNotificationSettings = ImagineCommunications.GamePlan.Domain.Shared.System.Settings.HTTPNotificationSettings;
using MinimumDocumentRestriction = ImagineCommunications.GamePlan.Domain.Runs.Objects.MinimumDocumentRestriction;
using MinimumRunSizeDocumentRestriction = ImagineCommunications.GamePlan.Domain.Runs.Objects.MinimumRunSizeDocumentRestriction;
using RunEventSettings = ImagineCommunications.GamePlan.Domain.Runs.Objects.RunEventSettings;
using RunRestrictions = ImagineCommunications.GamePlan.Domain.Runs.Objects.RunRestrictions;
using TenantSettings = ImagineCommunications.GamePlan.Domain.Shared.System.Tenants.TenantSettings;
using FeatureEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.TenantSettings.Feature;
using Feature = ImagineCommunications.GamePlan.Domain.Shared.System.Settings.Feature;
using WebhookSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.TenantSettings.WebhookSettings;
using WebhookSettings = ImagineCommunications.GamePlan.Domain.Shared.System.Settings.WebhookSettings;
using xggameplan.core.Extensions;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class TenantSettingsProfile : Profile
    {
        public TenantSettingsProfile()
        {
            CreateMap<TenantSettings, TenantSettingsEntity>()
                .ForMember(dest => dest.PeakStartTime,
                    opt => opt.MapFrom(src => AgConversions.ParseHHMMSSFormat(src.PeakStartTime)))
                .ForMember(dest => dest.PeakEndTime,
                    opt => opt.MapFrom(src => AgConversions.ParseHHMMSSFormat(src.PeakEndTime)))
                .ForMember(dest => dest.MidnightStartTime,
                    opt => opt.MapFrom(src => AgConversions.ParseTotalHHMMSSFormat(src.MidnightStartTime, true)))
                .ForMember(dest => dest.MidnightEndTime,
                    opt => opt.MapFrom(src => AgConversions.ParseTotalHHMMSSFormat(src.MidnightEndTime, true)))
                .ReverseMap()
                .ForMember(dest => dest.PeakStartTime,
                    opt => opt.MapFrom(src => AgConversions.ToAgTimeAsHHMMSS(src.PeakStartTime)))
                .ForMember(dest => dest.PeakEndTime,
                    opt => opt.MapFrom(src => AgConversions.ToAgTimeAsHHMMSS(src.PeakEndTime)))
                .ForMember(dest => dest.MidnightStartTime,
                    opt => opt.MapFrom(src => AgConversions.ToAgTimeAsTotalHHMMSS(src.MidnightStartTime)))
                .ForMember(dest => dest.MidnightEndTime,
                    opt => opt.MapFrom(src => AgConversions.ToAgTimeAsTotalHHMMSS(src.MidnightEndTime)));

            CreateMap<RunRestrictions, RunRestrictionsEntity>()
                .ReverseMap();

            CreateMap<MinimumRunSizeDocumentRestriction, MinimumRunSizeDocumentRestrictionEntity>()
                .ReverseMap();

            CreateMap<MinimumDocumentRestriction, MinimumDocumentRestrictionEntity>()
                .ReverseMap();

            CreateMap<RunEventSettings, RunEventSettingsEntity>()
                .ReverseMap();

            CreateMap<HTTPNotificationSettings, HTTPNotificationSettingsEntity>()
                .ReverseMap();

            CreateMap<HTTPMethodSettings, HTTPMethodSettingsEntity>()
                .ReverseMap();

            CreateMap<Feature, FeatureEntity>()
                .ForMember(dest => dest.IdValue, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdValue))
                .ForMember(dest => dest.Settings,
                    opt => opt.MapFrom(src => src.Settings.ToDictionary(x => x.Key, x => (object)x.Value)))
                ;

            CreateMap<WebhookSettings, WebhookSettingsEntity>()
                .ReverseMap();
        }
    }
}

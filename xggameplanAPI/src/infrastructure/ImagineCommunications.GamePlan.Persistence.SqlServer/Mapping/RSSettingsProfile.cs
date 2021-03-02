using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RSSettings;
using RSDemographicSettings = ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings.Objects.RSDemographicSettings;
using RSDemographicSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RSSettings.RSDemographicSettings;
using RSSettings = ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings.Objects.RSSettings;
using RSSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RSSettings.RSSettings;
using RSSettingsDemographicsDeliverySettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RSSettings.RSSettingsDemographicsDeliverySettings;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class RSSettingsProfile : Profile
    {
        public RSSettingsProfile()
        {
            CreateMap<RSSettings, RSSettingsEntity>()
                .ReverseMap();

            CreateMap<RSDemographicSettings, RSDemographicSettingsEntity>()
                .ReverseMap();

            CreateMap<RSDeliverySettings, RSSettingsDemographicsDeliverySettingsEntity>()
                .ReverseMap();

            CreateMap<RSDeliverySettings, RSSettingsDefaultDeliverySettings>()
                .ReverseMap();
        }
    }
}

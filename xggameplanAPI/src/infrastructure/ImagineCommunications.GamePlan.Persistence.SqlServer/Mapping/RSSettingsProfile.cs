using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RSSettings;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using xggameplan.core.Extensions.AutoMapper;
using RSDemographicSettings = ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings.Objects.RSDemographicSettings;
using RSDemographicSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RSSettings.RSDemographicSettings;
using RSSettings = ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings.Objects.RSSettings;
using RSSettingsDemographicsDeliverySettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RSSettings.RSSettingsDemographicsDeliverySettings;
using RSSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RSSettings.RSSettings;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class RSSettingsProfile : Profile
    {
        public RSSettingsProfile()
        {
            _ = CreateMap<RSSettings, RSSettingsEntity>()
               .ForMember(d => d.SalesArea, o => o.Ignore())
               .ForMember(d => d.SalesAreaId,
                   o => o.FromEntityCache(src => src.SalesArea, opts => opts.Entity<SalesArea>(x => x.Id)))
               .ReverseMap()
               .ForMember(d => d.SalesArea,
                   opts => opts.FromEntityCache(src => src.SalesAreaId,
                       s => s.Entity<SalesArea>(x => x.Name).CheckNavigationPropertyFirst(x => x.SalesArea)));

            _ = CreateMap<RSDemographicSettings, RSDemographicSettingsEntity>()
                .ReverseMap();

            _ = CreateMap<RSDeliverySettings, RSSettingsDemographicsDeliverySettingsEntity>()
                .ReverseMap();

            _ = CreateMap<RSDeliverySettings, RSSettingsDefaultDeliverySettings>()
                .ReverseMap();
        }
    }
}

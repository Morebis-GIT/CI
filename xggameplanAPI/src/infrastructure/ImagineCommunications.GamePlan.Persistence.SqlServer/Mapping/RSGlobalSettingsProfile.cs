using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSGlobalSettings.Objects;
using RSGlobalSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RSGlobalSettings;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class RSGlobalSettingsProfile: Profile
    {
        public RSGlobalSettingsProfile()
        {
            CreateMap<RSGlobalSettingsEntity, RSGlobalSettings>();
            CreateMap<RSGlobalSettings, RSGlobalSettingsEntity>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .AfterMap((s, d) => d.Id = s.Id == default ? d.Id : s.Id);
        }
    }
}

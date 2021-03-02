using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRGlobalSettings.Objects;
using ISRGlobalSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ISRGlobalSettings;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class ISRGlobalSettingsProfile: Profile
    {
        public ISRGlobalSettingsProfile()
        {
            CreateMap<ISRGlobalSettingsEntity, ISRGlobalSettings>();
            CreateMap<ISRGlobalSettings, ISRGlobalSettingsEntity>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .AfterMap((s, d) => d.Id = s.Id == default ? d.Id : s.Id);
        }
    }
}

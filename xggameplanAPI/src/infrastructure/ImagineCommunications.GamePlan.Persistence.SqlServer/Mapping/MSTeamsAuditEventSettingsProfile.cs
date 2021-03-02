using AutoMapper;
using xggameplan.AuditEvents;
using MSTeamsAuditEventSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.MSTeamsAuditEventSettings.MSTeamsAuditEventSettings;
using MSTeamsPostMessageSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.MSTeamsAuditEventSettings.MSTeamsPostMessageSettings;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class MSTeamsAuditEventSettingsProfile : Profile
    {
        public MSTeamsAuditEventSettingsProfile()
        {
            CreateMap<MSTeamsAuditEventSettings, MSTeamsAuditEventSettingsEntity>().ReverseMap();
            CreateMap<MSTeamsPostMessageSettings, MSTeamsPostMessageSettingsEntity>().ReverseMap();
        }
    }
}

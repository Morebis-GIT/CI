using AutoMapper;
using xggameplan.AuditEvents;
using EmailAuditEventSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.EmailAuditEventSettings;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class EmailAuditEventSettingsProfile : Profile
    {
        public EmailAuditEventSettingsProfile()
        {
            CreateMap<EmailAuditEventSettings, EmailAuditEventSettingsEntity>()
                .ReverseMap();
        }
    }
}

using xggameplan.AuditEvents;

namespace xggameplan.Profile
{
    internal class AuditEventProfile : AutoMapper.Profile
    {
        public AuditEventProfile()
        {
            CreateMap<AuditEventModel, AuditEvent>();
            CreateMap<AuditEvent, AuditEventModel>();            
        }
    }

    internal class AuditEventValueProfile : AutoMapper.Profile
    {
        public AuditEventValueProfile()
        {
            CreateMap<AuditEventValueModel, AuditEventValue>();
            CreateMap<AuditEventValue, AuditEventValueModel>();            
        }
    }
}

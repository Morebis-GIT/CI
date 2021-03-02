using xggameplan.AuditEvents;

namespace xggameplan.Profile
{
    internal class AuditEventFilterProfile : AutoMapper.Profile
    {
        public AuditEventFilterProfile()
        {
            CreateMap<AuditEventFilterModel, AuditEventFilter>();
            CreateMap<AuditEventFilter, AuditEventFilterModel>();
        }
    }

    internal class AuditEventValueFilterProfile : AutoMapper.Profile
    {
        public AuditEventValueFilterProfile()
        {
            CreateMap<AuditEventValueFilterModel, AuditEventValueFilter>();
            CreateMap<AuditEventValueFilter, AuditEventValueFilterModel>();
        }
    }
}

using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.PipelineAuditEvents;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    class PipelineAuditEventProfile : Profile
    {
            public PipelineAuditEventProfile()
            {
            CreateMap<PipelineAuditEvent, xggameplan.AuditEvents.PipelineAuditEvent>().ReverseMap();
            }
    }
}

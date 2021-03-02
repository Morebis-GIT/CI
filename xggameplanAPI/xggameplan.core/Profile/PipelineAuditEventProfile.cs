using xggameplan.AuditEvents;

namespace xggameplan.Profile
{
    internal class PipeLineAuditEventProfile : AutoMapper.Profile
    {
        public PipeLineAuditEventProfile()
        {
            CreateMap<PipelineAuditEventModel, PipelineAuditEvent>().ReverseMap();
        }
    }
}

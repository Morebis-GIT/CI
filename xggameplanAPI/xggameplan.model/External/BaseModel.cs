using System;

namespace xggameplan.Model
{
    public abstract class BaseModel : AuditBaseModel
    {
        public Guid Uid { get; set; }
    }
}

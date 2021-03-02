using System;
using System.Collections.Generic;
using System.Text;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Sponsorships
{
    public abstract class BaseModel : AuditBase
    {
        public Guid Uid { get; set; }
    }
}

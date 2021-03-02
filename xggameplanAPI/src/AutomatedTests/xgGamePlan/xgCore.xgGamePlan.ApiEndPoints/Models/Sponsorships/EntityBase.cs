using System;
using System.Collections.Generic;
using System.Text;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Sponsorships
{
    public abstract class EntityBase : AuditBase
    {
        public int Id { get; set; }

        public Guid Uid { get; set; }
    }
}

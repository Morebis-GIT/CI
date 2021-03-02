using System;
using System.Collections.Generic;
using System.Text;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Sponsorships
{
    public abstract class AuditBase
    {
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}

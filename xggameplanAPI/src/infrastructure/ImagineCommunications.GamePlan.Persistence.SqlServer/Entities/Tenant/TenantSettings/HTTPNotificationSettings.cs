
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.TenantSettings
{
    public class HTTPNotificationSettings
    {
        public bool Enabled { get; set; }

        public HTTPMethodSettings MethodSettings { get; set; }

        public List<int> SucccessStatusCodes  { get; set; } 
    }
}

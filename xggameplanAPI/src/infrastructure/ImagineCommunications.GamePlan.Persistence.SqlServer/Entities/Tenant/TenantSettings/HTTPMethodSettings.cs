using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.TenantSettings
{
    public class HTTPMethodSettings
    {
        public string Method { get; set; }

        public string URL { get; set; }

        public string ContentTemplate { get; set; }

        public Dictionary<string, string> Headers  { get; set; } 
    }
}

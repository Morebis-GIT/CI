using System.Collections.Generic;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.Tenants;

namespace xgCore.xgGamePlan.AutomationTests.Contexts
{
    [Binding]
    public class TenantsContext
    {
        #pragma warning disable CA2227
        public List<Tenant> InitialTenants { get; set; }
        #pragma warning restore CA2227

        public Tenant GivenTenant { get; set; }

        public Tenant ReturnedTenant { get; set; }
    }

}

using System;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Tenants
{
    public class Tenant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DefaultTheme { get; set; }
        public ProviderConfiguration SchedulingDb { get; set; }
        public ProviderConfiguration TenantDb { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Tenant tenant)
            {
                return Equals(tenant);
            }

            return false;
        }

        protected bool Equals(Tenant other)
        {
            return Id == other.Id && string.Equals(Name, other.Name, StringComparison.InvariantCulture) &&
                   string.Equals(DefaultTheme, other.DefaultTheme, StringComparison.InvariantCulture) &&
                   Equals(SchedulingDb, other.SchedulingDb) &&
                   Equals(TenantDb, other.TenantDb);
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}

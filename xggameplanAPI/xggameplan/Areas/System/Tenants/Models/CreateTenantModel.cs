using ImagineCommunications.GamePlan.Domain.Shared.System;

namespace xggameplan.Areas.System.Tenants.Models
{
    /// <summary>
    /// Representation of the tenant.
    /// </summary>
    public class CreateTenantModel
    {
        /// <summary>
        /// Tenant Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Theme Name
        /// </summary>
        public string DefaultTheme { get; set; }

        /// <summary>
        /// Tenant Db Provider.
        /// </summary>
        public DbProviderType TenantDbProvider { get; set; }

        /// <summary>
        /// Tenant Db Configuration.
        /// </summary>
        public string TenantDbConfiguration { get; set; }

        public string TenantDbConnectionString { get; set; }

    }
}

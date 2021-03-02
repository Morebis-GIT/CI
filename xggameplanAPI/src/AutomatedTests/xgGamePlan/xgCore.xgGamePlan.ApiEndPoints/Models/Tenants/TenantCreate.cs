namespace xgCore.xgGamePlan.ApiEndPoints.Models.Tenants
{
    public class TenantCreate
    {
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

        public string TenantDbConnectionString { get; set; }    }
}

public enum DbProviderType
{
    RavenDb,
    SqlServer
}

using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;

namespace xggameplan.Areas.System.Tenants.Models
{
    /// <summary>
    /// Representation of the tenant.
    /// </summary>
    public class TenantModel
    {
        /// <summary>
        /// Tenant id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Tenant Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Theme Name
        /// </summary>
        public string DefaultTheme { get; set; }

        /// <summary>
        /// Tenant Db.
        /// </summary>
        public DatabaseProviderConfiguration TenantDb { get; set; }
    }



    public class TenantModelProfile : AutoMapper.Profile       // CMF Made public
    {
        public TenantModelProfile() => CreateMap<Tenant, TenantModel>();
    }
}

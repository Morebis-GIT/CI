using ImagineCommunications.GamePlan.Domain.Shared.System;
using Microsoft.Extensions.Configuration;

namespace xggameplan.common.Extensions
{
    public static class ApplicationConfigurationExtensions
    {
        public static DbProviderType GetDbProvider(this IConfiguration applicationConfiguration) =>
            applicationConfiguration.GetValue<DbProviderType>("db:master:provider");

        public static bool IsTempTenantDefined(this IConfiguration applicationConfiguration) =>
            applicationConfiguration.GetValue<string>("db:tempTenant:connectionString", null) != null;

        public static string GetTempTenantConnectionString(this IConfiguration applicationConfiguration) =>
            applicationConfiguration.GetValue<string>("db:tempTenant:connectionString", null);
    }
}

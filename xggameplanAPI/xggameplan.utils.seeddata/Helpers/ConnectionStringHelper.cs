using System;
using System.Data.Common;
using ImagineCommunications.GamePlan.Domain.Shared.System;

namespace xggameplan.utils.seeddata.Helpers
{
    public static class ConnectionStringHelper
    {
        public static DbProviderType RecognizeDbProviderType(string connectionString)
        {
            var csBuilder = new DbConnectionStringBuilder
            {
                ConnectionString = connectionString
            };

            var isRavenCs = csBuilder.ContainsKey("Url") && csBuilder.ContainsKey("Database");
            var isSqlCs = (csBuilder.ContainsKey("Data Source") || csBuilder.ContainsKey("Server")) && csBuilder.ContainsKey("Initial Catalog");

            if (isRavenCs && !isSqlCs)
            {
                return DbProviderType.RavenDb;
            }
            if (isSqlCs && !isRavenCs)
            {
                return DbProviderType.SqlServer;
            }

            throw new Exception("Db provider isn't recognizable by the specified connection string.");
        }

    }
}

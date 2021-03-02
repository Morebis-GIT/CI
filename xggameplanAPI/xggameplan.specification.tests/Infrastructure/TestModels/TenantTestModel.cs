using ImagineCommunications.GamePlan.Domain.Shared.System;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;

namespace xggameplan.specification.tests.Infrastructure.TestModels
{
    public class TenantTestModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DefaultTheme { get; set; }
        public DatabaseProviderConfiguration TenantDb { get; set; }

        public TenantTestModel()
        {
            TenantDb = DatabaseProviderConfiguration.CreateFromConfiguration(DbProviderType.SqlServer, "{\"connectionString\": \"Database=Test\"}");
        }
    }
}

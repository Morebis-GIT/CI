using ImagineCommunications.GamePlan.Domain.Generic.Types;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.MasterMigrations
{
    public partial class UpdateFeatureFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"UPDATE [dbo].[TenantProductFeatures] SET IsShared = 1 WHERE Name = '{nameof(ProductFeature.IntegrationSynchronization)}'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"UPDATE [dbo].[TenantProductFeatures] SET IsShared = 0 WHERE Name = '{nameof(ProductFeature.IntegrationSynchronization)}'");
        }
    }
}

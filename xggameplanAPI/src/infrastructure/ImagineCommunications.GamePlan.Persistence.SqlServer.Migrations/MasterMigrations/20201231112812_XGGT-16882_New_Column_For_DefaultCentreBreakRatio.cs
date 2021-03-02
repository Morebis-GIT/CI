using ImagineCommunications.GamePlan.Domain.Generic.Types;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.MasterMigrations
{
    public partial class XGGT16882_New_Column_For_DefaultCentreBreakRatio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql(@$"
                IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[TenantProductFeatures] WHERE [Name] = '{nameof(ProductFeature.DefaultCentreBreakRatio)}')
                INSERT INTO [dbo].[TenantProductFeatures] ([TenantId], [Name], [Enabled], [IsShared])
                SELECT [Id], '{nameof(ProductFeature.DefaultCentreBreakRatio)}', 0, 1 FROM [dbo].[Tenants]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql($"DELETE FROM [dbo].[TenantProductFeatures] WHERE [Name] = '{nameof(ProductFeature.DefaultCentreBreakRatio)}'");
        }
    }
}

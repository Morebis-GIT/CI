using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.MasterMigrations
{
    public partial class XGGT15915_Remove_MapBreakWithProgrammesByExternalRefFeatureFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql(
                "DELETE FROM [dbo].[TenantProductFeatures] WHERE [Name] = 'MapBreakWithProgrammesByExternalRef'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql(@$"
                    IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[TenantProductFeatures] WHERE [Name] = 'MapBreakWithProgrammesByExternalRef')
                        INSERT INTO [dbo].[TenantProductFeatures] ([TenantId], [Name], [Enabled], [IsShared])
                        SELECT [Id], 'MapBreakWithProgrammesByExternalRef', 0, 1 FROM [dbo].[Tenants]");
        }
    }
}

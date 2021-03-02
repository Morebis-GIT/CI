using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.MasterMigrations
{
    public partial class XGGT16881Add_AnalysisGroups_Feature_Flag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[TenantProductFeatures] WHERE [Name] = 'AnalysisGroups')\n" +
                "INSERT INTO [dbo].[TenantProductFeatures] ([TenantId], [Name], [Enabled], [IsShared])\n" +
                "SELECT [Id], 'AnalysisGroups', 0, 1 FROM [dbo].[Tenants]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [dbo].[TenantProductFeatures] WHERE [Name] = 'AnalysisGroups'");
        }
    }
}

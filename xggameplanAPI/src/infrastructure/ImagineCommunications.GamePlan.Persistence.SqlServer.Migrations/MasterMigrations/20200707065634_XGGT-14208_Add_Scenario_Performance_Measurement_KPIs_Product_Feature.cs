using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.MasterMigrations
{
    public partial class XGGT14208_Add_Scenario_Performance_Measurement_KPIs_Product_Feature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "IF EXISTS (SELECT TOP 1 1 FROM [dbo].[TenantProductFeatures])\n" +
                "INSERT INTO [dbo].[TenantProductFeatures] ([TenantId], [Name], [Enabled], [IsShared])\n" +
                "SELECT [Id], 'ScenarioPerformanceMeasurementKPIs', 1, 1 FROM [dbo].[Tenants]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [dbo].[TenantProductFeatures] WHERE [Name] = 'ScenarioPerformanceMeasurementKPIs'");
        }
    }
}

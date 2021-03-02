using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT18176_Add_CampaignLevelScenarioCampaignResults_Facility : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql(@$"
                    IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[Facilities] WHERE [Code] = 'XGSCRC')
                        INSERT INTO [dbo].[Facilities] ([Code], [Description], [Enabled])
                        VALUES ('XGSCRC', 'xG GamePlan - Scenario Campaign Results at Campaign Level', 1)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql($"DELETE FROM [dbo].[Facilities] WHERE [Code] = 'XGSCRC'");
        }
    }
}

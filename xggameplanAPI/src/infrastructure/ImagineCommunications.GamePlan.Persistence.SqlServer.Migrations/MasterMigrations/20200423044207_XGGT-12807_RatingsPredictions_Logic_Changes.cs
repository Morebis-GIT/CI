using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.MasterMigrations
{
    public partial class XGGT12807_RatingsPredictions_Logic_Changes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT TOP 1 1 FROM [dbo].[TenantProductFeatures])
                INSERT INTO [dbo].[TenantProductFeatures]([TenantId], [Name], [Enabled], [IsShared])
                    SELECT
                        [Id],
                        'ExactBreaksRatingsTimeMatching',
                        0,
                        0
                    FROM [dbo].[Tenants]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [dbo].[TenantProductFeatures] WHERE [Name] = 'ExactBreaksRatingsTimeMatching'");
        }
    }
}

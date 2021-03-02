using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT12197_Spots_Per_Day_KPIComparisonConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
            IF EXISTS (SELECT TOP 1 1 FROM [dbo].[KPIComparisonConfigs]) 
            BEGIN
                insert into KPIComparisonConfigs(
                    {nameof(KPIComparisonConfig.KPIName)},
                    {nameof(KPIComparisonConfig.DiscernibleDifference)},
                    {nameof(KPIComparisonConfig.HigherIsBest)},
                    {nameof(KPIComparisonConfig.Ranked)})
                values ('averageSpotsDeliveredPerDay', '{1.0f}', 1, 1)
            END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "KPIComparisonConfigs",
                keyColumn: "KPIName",
                keyValue: "averageSpotsDeliveredPerDay");
        }
    }
}

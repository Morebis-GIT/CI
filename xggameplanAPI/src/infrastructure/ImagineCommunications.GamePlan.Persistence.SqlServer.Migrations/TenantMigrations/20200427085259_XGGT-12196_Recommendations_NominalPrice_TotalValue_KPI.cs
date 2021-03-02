using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT12196_Recommendations_NominalPrice_TotalValue_KPI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Addition of new column to table and setting zero value 
            // for already existing data in one action 
            // (by adding this column with 0.0 as default value)
            migrationBuilder.AddColumn<double>(
                name: "NominalPrice",
                table: "Recommendations",
                nullable: false,
                defaultValue: 0.0);

            // Removal of default value from newly added column
            // (as it should not contain one)
            migrationBuilder.AlterColumn<double>(
                name: "NominalPrice",
                table: "Recommendations",
                nullable: false,
                defaultValue: null,
                oldDefaultValue: 0.0);

            migrationBuilder.Sql($@"
            IF EXISTS (SELECT TOP 1 1 FROM [dbo].[KPIComparisonConfigs]) 
            BEGIN
                insert into KPIComparisonConfigs(
                    {nameof(KPIComparisonConfig.KPIName)},
                    {nameof(KPIComparisonConfig.DiscernibleDifference)},
                    {nameof(KPIComparisonConfig.HigherIsBest)},
                    {nameof(KPIComparisonConfig.Ranked)})
                values ('totalValueDelivered', '{1.0f}', 1, 1)
            END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.DropColumn(
		        name: "NominalPrice",
		        table: "Recommendations");

            migrationBuilder.DeleteData(
                table: "KPIComparisonConfigs",
                keyColumn: "KPIName",
                keyValue: "totalValueDelivered");
        }
    }
}

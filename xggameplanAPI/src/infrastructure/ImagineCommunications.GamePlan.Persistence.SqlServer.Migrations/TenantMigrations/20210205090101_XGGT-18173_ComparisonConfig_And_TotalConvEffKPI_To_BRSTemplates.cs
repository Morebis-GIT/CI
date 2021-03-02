using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT18173_ComparisonConfig_And_TotalConvEffKPI_To_BRSTemplates : Migration
    {

        private const int ExcludedPriorityId = 1;
        private const string KpiName = "conversionEfficiencyTotal";
        private const float DefaultDiscernibleDifference = 1.0f;

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                DECLARE @template_id INT

                DECLARE templates_cursor CURSOR FOR
                SELECT Id FROM BRSConfigurationTemplates

                OPEN templates_cursor
                FETCH NEXT FROM templates_cursor INTO @template_id

                WHILE @@FETCH_STATUS = 0
                BEGIN
	                INSERT INTO BRSConfigurationForKPIs VALUES
	                (@template_id, '{KpiName}', {ExcludedPriorityId})

	                UPDATE BRSConfigurationTemplates
	                SET LastModified = GETUTCDATE()
	                WHERE Id = @template_id

                    FETCH NEXT FROM templates_cursor INTO @template_id
                END

                CLOSE templates_cursor
                DEALLOCATE templates_cursor");

            migrationBuilder.Sql($@"
                INSERT INTO KPIComparisonConfigs VALUES
                ('{KpiName}', {DefaultDiscernibleDifference}, 1, 1)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BRSConfigurationForKPIs",
                keyColumn: "KPIName",
                keyValue: KpiName);

            migrationBuilder.Sql(@"
                UPDATE BRSConfigurationTemplates
	            SET LastModified = GETUTCDATE()");

            migrationBuilder.DeleteData(
                table: "KPIComparisonConfigs",
                keyColumn: "KPIName",
                keyValue: KpiName);
        }
    }
}

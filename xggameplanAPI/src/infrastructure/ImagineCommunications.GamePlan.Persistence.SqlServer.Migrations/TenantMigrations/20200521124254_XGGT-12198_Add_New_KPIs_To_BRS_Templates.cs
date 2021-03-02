using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT12198_Add_New_KPIs_To_BRS_Templates : Migration
    {
        private const int ExcludedPriorityId = 1;
        private const string AverageSpotsDeliveredPerDayKpiName = "averageSpotsDeliveredPerDay";
        private const string TotalZeroRatedSpotsKpiName = "totalZeroRatedSpots";
        private const string TotalValueDeliveredKpiName = "totalValueDelivered";

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
	                (@template_id, '{AverageSpotsDeliveredPerDayKpiName}', {ExcludedPriorityId}),
	                (@template_id, '{TotalZeroRatedSpotsKpiName}', {ExcludedPriorityId}),
	                (@template_id, '{TotalValueDeliveredKpiName}', {ExcludedPriorityId})

	                UPDATE BRSConfigurationTemplates
	                SET LastModified = GETDATE()
	                WHERE Id = @template_id

                    FETCH NEXT FROM templates_cursor INTO @template_id
                END

                CLOSE templates_cursor
                DEALLOCATE templates_cursor");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BRSConfigurationForKPIs",
                keyColumn: "KPIName",
                keyValues: new object[]
                {
                    AverageSpotsDeliveredPerDayKpiName, TotalZeroRatedSpotsKpiName, TotalValueDeliveredKpiName
                });

            migrationBuilder.Sql(@"
                UPDATE BRSConfigurationTemplates
	            SET LastModified = GETDATE()");
        }
    }
}

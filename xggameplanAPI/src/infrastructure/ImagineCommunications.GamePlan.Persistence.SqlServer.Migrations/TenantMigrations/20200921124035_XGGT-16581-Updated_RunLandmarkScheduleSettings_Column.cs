using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT16581Updated_RunLandmarkScheduleSettings_Column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ScheduledTime",
                table: "RunLandmarkScheduleSettings",
                nullable: true,
                oldClrType: typeof(long));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"
                DELETE FROM RunLandmarkScheduleSettings
                WHERE {nameof(RunLandmarkScheduleSettings.ScheduledTime)} IS NULL");

            migrationBuilder.AlterColumn<long>(
                name: "ScheduledTime",
                table: "RunLandmarkScheduleSettings",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);
        }
    }
}

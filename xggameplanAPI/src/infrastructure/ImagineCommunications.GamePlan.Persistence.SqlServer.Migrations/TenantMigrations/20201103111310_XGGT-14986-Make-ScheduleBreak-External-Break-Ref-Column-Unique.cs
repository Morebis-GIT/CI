using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT14986MakeScheduleBreakExternalBreakRefColumnUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScheduleBreaks_ExternalBreakRef",
                table: "ScheduleBreaks");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleBreaks_ExternalBreakRef",
                table: "ScheduleBreaks",
                column: "ExternalBreakRef",
                unique: true,
                filter: "[ExternalBreakRef] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScheduleBreaks_ExternalBreakRef",
                table: "ScheduleBreaks");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleBreaks_ExternalBreakRef",
                table: "ScheduleBreaks",
                column: "ExternalBreakRef");
        }
    }
}

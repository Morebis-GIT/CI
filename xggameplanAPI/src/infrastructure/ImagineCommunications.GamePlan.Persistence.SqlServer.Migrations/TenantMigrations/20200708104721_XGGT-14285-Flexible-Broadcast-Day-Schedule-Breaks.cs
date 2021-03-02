using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT14285FlexibleBroadcastDayScheduleBreaks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BroadcastDate",
                table: "ScheduleBreaks",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClockHour",
                table: "ScheduleBreaks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BroadcastDate",
                table: "ScheduleBreaks");

            migrationBuilder.DropColumn(
                name: "ClockHour",
                table: "ScheduleBreaks");
        }
    }
}

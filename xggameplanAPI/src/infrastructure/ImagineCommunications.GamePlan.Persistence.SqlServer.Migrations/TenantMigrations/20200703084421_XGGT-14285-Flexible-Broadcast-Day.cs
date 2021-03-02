using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT14285FlexibleBroadcastDay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BroadcastDate",
                table: "Breaks",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClockHour",
                table: "Breaks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BroadcastDate",
                table: "Breaks");

            migrationBuilder.DropColumn(
                name: "ClockHour",
                table: "Breaks");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT13238_Extend_schedulebreak_model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowSplit",
                table: "ScheduleBreaks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BonusAllowed",
                table: "ScheduleBreaks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ExcludePackages",
                table: "ScheduleBreaks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LongForm",
                table: "ScheduleBreaks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NationalRegionalSplit",
                table: "ScheduleBreaks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PremiumCategory",
                table: "ScheduleBreaks",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Solus",
                table: "ScheduleBreaks",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowSplit",
                table: "ScheduleBreaks");

            migrationBuilder.DropColumn(
                name: "BonusAllowed",
                table: "ScheduleBreaks");

            migrationBuilder.DropColumn(
                name: "ExcludePackages",
                table: "ScheduleBreaks");

            migrationBuilder.DropColumn(
                name: "LongForm",
                table: "ScheduleBreaks");

            migrationBuilder.DropColumn(
                name: "NationalRegionalSplit",
                table: "ScheduleBreaks");

            migrationBuilder.DropColumn(
                name: "PremiumCategory",
                table: "ScheduleBreaks");

            migrationBuilder.DropColumn(
                name: "Solus",
                table: "ScheduleBreaks");
        }
    }
}

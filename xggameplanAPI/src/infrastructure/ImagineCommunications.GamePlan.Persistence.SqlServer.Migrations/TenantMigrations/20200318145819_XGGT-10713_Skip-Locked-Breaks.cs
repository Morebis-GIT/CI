using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT10713_SkipLockedBreaks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ExcludeBankHolidays",
                table: "Runs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ExcludeSchoolHolidays",
                table: "Runs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IgnorePremiumCategoryBreaks",
                table: "Runs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SkipLockedBreaks",
                table: "Runs",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExcludeBankHolidays",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "ExcludeSchoolHolidays",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "IgnorePremiumCategoryBreaks",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "SkipLockedBreaks",
                table: "Runs");
        }
    }
}

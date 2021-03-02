using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT13238_Extend_break_model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowSplit",
                table: "Breaks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BonusAllowed",
                table: "Breaks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ExcludePackages",
                table: "Breaks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LongForm",
                table: "Breaks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NationalRegionalSplit",
                table: "Breaks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PremiumCategory",
                table: "Breaks",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Solus",
                table: "Breaks",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowSplit",
                table: "Breaks");

            migrationBuilder.DropColumn(
                name: "BonusAllowed",
                table: "Breaks");

            migrationBuilder.DropColumn(
                name: "ExcludePackages",
                table: "Breaks");

            migrationBuilder.DropColumn(
                name: "LongForm",
                table: "Breaks");

            migrationBuilder.DropColumn(
                name: "NationalRegionalSplit",
                table: "Breaks");

            migrationBuilder.DropColumn(
                name: "PremiumCategory",
                table: "Breaks");

            migrationBuilder.DropColumn(
                name: "Solus",
                table: "Breaks");
        }
    }
}

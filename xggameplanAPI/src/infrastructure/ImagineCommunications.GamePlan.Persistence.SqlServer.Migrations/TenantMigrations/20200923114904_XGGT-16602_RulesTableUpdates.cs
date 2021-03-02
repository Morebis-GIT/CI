using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT16602_RulesTableUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForceOverUnder",
                table: "Rules");

            migrationBuilder.DropColumn(
                name: "Ignore",
                table: "Rules");

            migrationBuilder.DropColumn(
                name: "Over",
                table: "Rules");

            migrationBuilder.DropColumn(
                name: "PeakValue",
                table: "Rules");

            migrationBuilder.DropColumn(
                name: "Under",
                table: "Rules");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "Rules");

            migrationBuilder.AddColumn<int>(
                name: "CampaignType",
                table: "Rules",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CampaignType",
                table: "Rules");

            migrationBuilder.AddColumn<int>(
                name: "ForceOverUnder",
                table: "Rules",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Ignore",
                table: "Rules",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Over",
                table: "Rules",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PeakValue",
                table: "Rules",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Under",
                table: "Rules",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "Rules",
                maxLength: 64,
                nullable: true);
        }
    }
}

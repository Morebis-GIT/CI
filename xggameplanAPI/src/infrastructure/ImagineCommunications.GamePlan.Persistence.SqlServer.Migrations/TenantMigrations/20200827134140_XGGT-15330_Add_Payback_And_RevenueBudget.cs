using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT15330_Add_Payback_And_RevenueBudget : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "RevenueBudget",
                table: "CampaignTargetStrikeWeights",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Payback",
                table: "CampaignTargetStrikeWeightDayParts",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RevenueBudget",
                table: "CampaignTargetStrikeWeightDayParts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RevenueBudget",
                table: "CampaignTargetStrikeWeights");

            migrationBuilder.DropColumn(
                name: "Payback",
                table: "CampaignTargetStrikeWeightDayParts");

            migrationBuilder.DropColumn(
                name: "RevenueBudget",
                table: "CampaignTargetStrikeWeightDayParts");
        }
    }
}

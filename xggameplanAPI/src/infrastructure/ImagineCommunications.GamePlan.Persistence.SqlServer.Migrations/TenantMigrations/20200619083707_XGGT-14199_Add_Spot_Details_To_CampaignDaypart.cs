using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT14199_Add_Spot_Details_To_CampaignDaypart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "BaseDemographRatings",
                table: "CampaignTargetStrikeWeightDayParts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "NominalValue",
                table: "CampaignTargetStrikeWeightDayParts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Ratings",
                table: "CampaignTargetStrikeWeightDayParts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "TotalSpotCount",
                table: "CampaignTargetStrikeWeightDayParts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ZeroRatedSpotCount",
                table: "CampaignTargetStrikeWeightDayParts",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseDemographRatings",
                table: "CampaignTargetStrikeWeightDayParts");

            migrationBuilder.DropColumn(
                name: "NominalValue",
                table: "CampaignTargetStrikeWeightDayParts");

            migrationBuilder.DropColumn(
                name: "Ratings",
                table: "CampaignTargetStrikeWeightDayParts");

            migrationBuilder.DropColumn(
                name: "TotalSpotCount",
                table: "CampaignTargetStrikeWeightDayParts");

            migrationBuilder.DropColumn(
                name: "ZeroRatedSpotCount",
                table: "CampaignTargetStrikeWeightDayParts");
        }
    }
}

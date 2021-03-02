using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT13656_Campaign_Level_KPIs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AchievedPercentageRevenueBudget",
                table: "Campaigns",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AchievedPercentageTargetRatings",
                table: "Campaigns",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActiveLength",
                table: "Campaigns",
                type: "NVARCHAR(MAX)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RatingsDifferenceExcludingPayback",
                table: "Campaigns",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueDifference",
                table: "Campaigns",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueDifferenceExcludingPayback",
                table: "Campaigns",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AchievedPercentageRevenueBudget",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "AchievedPercentageTargetRatings",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "ActiveLength",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "RatingsDifferenceExcludingPayback",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "ValueDifference",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "ValueDifferenceExcludingPayback",
                table: "Campaigns");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT15118_NewScenarioCompactCampaignFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AchievedPercentageRevenueBudget",
                table: "ScenarioCompactCampaigns",
                type: "DECIMAL(28,18)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AchievedPercentageTargetRatings",
                table: "ScenarioCompactCampaigns",
                type: "DECIMAL(28,18)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActiveLength",
                table: "ScenarioCompactCampaigns",
                type: "NVARCHAR(MAX)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AgencyGroupCode",
                table: "ScenarioCompactCampaigns",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AgencyGroupShortName",
                table: "ScenarioCompactCampaigns",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AutomatedBooked",
                table: "ScenarioCompactCampaigns",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "ScenarioCompactCampaigns",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RatingsDifferenceExcludingPayback",
                table: "ScenarioCompactCampaigns",
                type: "DECIMAL(28,18)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReportingCategory",
                table: "ScenarioCompactCampaigns",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RevenueBooked",
                table: "ScenarioCompactCampaigns",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalesExecutiveName",
                table: "ScenarioCompactCampaigns",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Spots",
                table: "ScenarioCompactCampaigns",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "StopBooking",
                table: "ScenarioCompactCampaigns",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "TargetXP",
                table: "ScenarioCompactCampaigns",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TopTail",
                table: "ScenarioCompactCampaigns",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueDifference",
                table: "ScenarioCompactCampaigns",
                type: "DECIMAL(28,18)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueDifferenceExcludingPayback",
                table: "ScenarioCompactCampaigns",
                type: "DECIMAL(28,18)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ScenarioCompactCampaignPaybacks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ScenarioCompactCampaignId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    Amount = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioCompactCampaignPaybacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioCompactCampaignPaybacks_ScenarioCompactCampaigns_ScenarioCompactCampaignId",
                        column: x => x.ScenarioCompactCampaignId,
                        principalTable: "ScenarioCompactCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCompactCampaignPaybacks_ScenarioCompactCampaignId",
                table: "ScenarioCompactCampaignPaybacks",
                column: "ScenarioCompactCampaignId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScenarioCompactCampaignPaybacks");

            migrationBuilder.DropColumn(
                name: "AchievedPercentageRevenueBudget",
                table: "ScenarioCompactCampaigns");

            migrationBuilder.DropColumn(
                name: "AchievedPercentageTargetRatings",
                table: "ScenarioCompactCampaigns");

            migrationBuilder.DropColumn(
                name: "ActiveLength",
                table: "ScenarioCompactCampaigns");

            migrationBuilder.DropColumn(
                name: "AgencyGroupCode",
                table: "ScenarioCompactCampaigns");

            migrationBuilder.DropColumn(
                name: "AgencyGroupShortName",
                table: "ScenarioCompactCampaigns");

            migrationBuilder.DropColumn(
                name: "AutomatedBooked",
                table: "ScenarioCompactCampaigns");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "ScenarioCompactCampaigns");

            migrationBuilder.DropColumn(
                name: "RatingsDifferenceExcludingPayback",
                table: "ScenarioCompactCampaigns");

            migrationBuilder.DropColumn(
                name: "ReportingCategory",
                table: "ScenarioCompactCampaigns");

            migrationBuilder.DropColumn(
                name: "RevenueBooked",
                table: "ScenarioCompactCampaigns");

            migrationBuilder.DropColumn(
                name: "SalesExecutiveName",
                table: "ScenarioCompactCampaigns");

            migrationBuilder.DropColumn(
                name: "Spots",
                table: "ScenarioCompactCampaigns");

            migrationBuilder.DropColumn(
                name: "StopBooking",
                table: "ScenarioCompactCampaigns");

            migrationBuilder.DropColumn(
                name: "TargetXP",
                table: "ScenarioCompactCampaigns");

            migrationBuilder.DropColumn(
                name: "TopTail",
                table: "ScenarioCompactCampaigns");

            migrationBuilder.DropColumn(
                name: "ValueDifference",
                table: "ScenarioCompactCampaigns");

            migrationBuilder.DropColumn(
                name: "ValueDifferenceExcludingPayback",
                table: "ScenarioCompactCampaigns");
        }
    }
}

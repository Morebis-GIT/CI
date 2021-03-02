using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT11407_Add_ScenarioCampaignFailure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScenarioCampaignFailures",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ScenarioId = table.Column<Guid>(nullable: false),
                    ExternalCampaignId = table.Column<string>(maxLength: 64, nullable: true),
                    SalesArea = table.Column<string>(maxLength: 64, nullable: true),
                    Length = table.Column<long>(nullable: false),
                    MultipartNo = table.Column<int>(nullable: false),
                    StrikeWeightStartDate = table.Column<DateTime>(nullable: false),
                    StrikeWeightEndDate = table.Column<DateTime>(nullable: false),
                    DayPartStartTime = table.Column<long>(nullable: false),
                    DayPartEndTime = table.Column<long>(nullable: false),
                    DayPartDays = table.Column<string>(maxLength: 7, nullable: true),
                    FailureType = table.Column<int>(nullable: false),
                    FailureCount = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioCampaignFailures", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCampaignFailures_ExternalCampaignId",
                table: "ScenarioCampaignFailures",
                column: "ExternalCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCampaignFailures_SalesArea",
                table: "ScenarioCampaignFailures",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCampaignFailures_ScenarioId",
                table: "ScenarioCampaignFailures",
                column: "ScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCampaignFailures_StrikeWeightEndDate",
                table: "ScenarioCampaignFailures",
                column: "StrikeWeightEndDate");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCampaignFailures_StrikeWeightStartDate",
                table: "ScenarioCampaignFailures",
                column: "StrikeWeightStartDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScenarioCampaignFailures");
        }
    }
}

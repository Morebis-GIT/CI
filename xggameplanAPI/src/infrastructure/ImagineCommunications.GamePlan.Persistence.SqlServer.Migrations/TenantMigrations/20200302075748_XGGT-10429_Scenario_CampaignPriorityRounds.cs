using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT10429_Scenario_CampaignPriorityRounds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScenarioCampaignPriorityRoundCollections",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ScenarioId = table.Column<Guid>(nullable: false),
                    ContainsInclusionRound = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioCampaignPriorityRoundCollections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioCampaignPriorityRoundCollections_Scenarios_ScenarioId",
                        column: x => x.ScenarioId,
                        principalTable: "Scenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioCampaignPriorityRounds",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ScenarioCampaignPriorityRoundCollectionId = table.Column<int>(nullable: false),
                    Number = table.Column<int>(nullable: false),
                    PriorityFrom = table.Column<int>(nullable: false),
                    PriorityTo = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioCampaignPriorityRounds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioCampaignPriorityRounds_ScenarioCampaignPriorityRoundCollections_ScenarioCampaignPriorityRoundCollectionId",
                        column: x => x.ScenarioCampaignPriorityRoundCollectionId,
                        principalTable: "ScenarioCampaignPriorityRoundCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCampaignPriorityRoundCollections_ScenarioId",
                table: "ScenarioCampaignPriorityRoundCollections",
                column: "ScenarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCampaignPriorityRounds_ScenarioCampaignPriorityRoundCollectionId",
                table: "ScenarioCampaignPriorityRounds",
                column: "ScenarioCampaignPriorityRoundCollectionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScenarioCampaignPriorityRounds");

            migrationBuilder.DropTable(
                name: "ScenarioCampaignPriorityRoundCollections");
        }
    }
}

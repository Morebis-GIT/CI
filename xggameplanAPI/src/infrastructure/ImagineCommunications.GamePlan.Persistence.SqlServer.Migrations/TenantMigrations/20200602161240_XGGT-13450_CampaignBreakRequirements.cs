using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT13450_CampaignBreakRequirements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CampaignBreakRequirements",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CampaignId = table.Column<Guid>(nullable: false),
                    SalesArea = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignBreakRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignBreakRequirements_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignBreakRequirementItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CampaignBreakRequirementId = table.Column<int>(nullable: false),
                    CurrentPercentageSplit = table.Column<double>(nullable: false),
                    DesiredPercentageSplit = table.Column<double>(nullable: false),
                    Discriminator = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignBreakRequirementItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignBreakRequirementItems_CampaignBreakRequirements_CampaignBreakRequirementId",
                        column: x => x.CampaignBreakRequirementId,
                        principalTable: "CampaignBreakRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignBreakRequirementItems_CampaignBreakRequirementId",
                table: "CampaignBreakRequirementItems",
                column: "CampaignBreakRequirementId",
                unique: false);

            migrationBuilder.CreateIndex(
                name: "IX_CampaignBreakRequirements_CampaignId",
                table: "CampaignBreakRequirements",
                column: "CampaignId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CampaignBreakRequirementItems");

            migrationBuilder.DropTable(
                name: "CampaignBreakRequirements");
        }
    }
}

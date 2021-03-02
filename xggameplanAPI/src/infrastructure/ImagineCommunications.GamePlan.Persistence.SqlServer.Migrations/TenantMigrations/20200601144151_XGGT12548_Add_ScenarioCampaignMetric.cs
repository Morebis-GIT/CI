using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT12548_Add_ScenarioCampaignMetric : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScenarioCampaignMetrics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ScenarioId = table.Column<Guid>(nullable: false),
                    CampaignExternalId = table.Column<string>(maxLength: 64, nullable: true),
                    TotalSpots = table.Column<int>(nullable: false),
                    ZeroRatedSpots = table.Column<int>(nullable: false),
                    NominalValue = table.Column<double>(nullable: false),
                    TotalNominalValue = table.Column<double>(nullable: false),
                    DifferenceValueDelivered = table.Column<double>(nullable: false),
                    DifferenceValueDeliveredPercentage = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioCampaignMetrics", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCampaignMetrics_ScenarioId",
                table: "ScenarioCampaignMetrics",
                column: "ScenarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("IF OBJECT_ID('dbo.ScenarioCampaignMetrics', 'U') IS NOT NULL \n" +
                                 "DROP TABLE [dbo].[ScenarioCampaignMetrics]");
        }
    }
}

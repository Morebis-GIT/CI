using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT14180_AnalysisGroupTarget : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnalysisGroupTargetMetrics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ScenarioResultId = table.Column<int>(nullable: false),
                    AnalysisGroupTargetId = table.Column<Guid>(nullable: false),
                    Value = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisGroupTargetMetrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnalysisGroupTargetMetrics_ScenarioResults_ScenarioResultId",
                        column: x => x.ScenarioResultId,
                        principalTable: "ScenarioResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RunAnalysisGroupTargets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RunId = table.Column<Guid>(nullable: false),
                    AnalysisGroupTargetId = table.Column<Guid>(nullable: false),
                    AnalysisGroupId = table.Column<int>(nullable: false),
                    KPI = table.Column<string>(maxLength: 64, nullable: false),
                    Target = table.Column<double>(nullable: false),
                    SortIndex = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RunAnalysisGroupTargets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RunAnalysisGroupTargets_Runs_RunId",
                        column: x => x.RunId,
                        principalTable: "Runs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisGroupTargetMetrics_ScenarioResultId_AnalysisGroupTargetId",
                table: "AnalysisGroupTargetMetrics",
                columns: new[] { "ScenarioResultId", "AnalysisGroupTargetId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RunAnalysisGroupTargets_AnalysisGroupTargetId",
                table: "RunAnalysisGroupTargets",
                column: "AnalysisGroupTargetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RunAnalysisGroupTargets_RunId",
                table: "RunAnalysisGroupTargets",
                column: "RunId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalysisGroupTargetMetrics");

            migrationBuilder.DropTable(
                name: "RunAnalysisGroupTargets");
        }
    }
}

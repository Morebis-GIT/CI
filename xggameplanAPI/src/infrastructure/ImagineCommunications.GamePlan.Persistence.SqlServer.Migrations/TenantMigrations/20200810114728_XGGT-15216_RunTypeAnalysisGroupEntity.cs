using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT15216_RunTypeAnalysisGroupEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RunTypeAnalysisGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RunTypeId = table.Column<int>(nullable: false),
                    AnalysisGroupId = table.Column<int>(nullable: false),
                    KPI = table.Column<string>(nullable: true),
                    IsDefault = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RunTypeAnalysisGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RunTypeAnalysisGroups_AnalysisGroups_AnalysisGroupId",
                        column: x => x.AnalysisGroupId,
                        principalTable: "AnalysisGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RunTypeAnalysisGroups_RunTypes_RunTypeId",
                        column: x => x.RunTypeId,
                        principalTable: "RunTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RunTypeAnalysisGroups_AnalysisGroupId",
                table: "RunTypeAnalysisGroups",
                column: "AnalysisGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_RunTypeAnalysisGroups_RunTypeId",
                table: "RunTypeAnalysisGroups",
                column: "RunTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RunTypeAnalysisGroups_RunTypeId_AnalysisGroupId_KPI",
                table: "RunTypeAnalysisGroups",
                columns: new[] { "RunTypeId", "AnalysisGroupId", "KPI" },
                unique: true,
                filter: "[KPI] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RunTypeAnalysisGroups");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT10445pipelineauditandautobooktaskreport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AutoBookTaskReports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TimeCreated = table.Column<DateTime>(nullable: false),
                    RunId = table.Column<Guid>(nullable: false),
                    ScenarioId = table.Column<Guid>(nullable: false),
                    Url = table.Column<string>(maxLength: 256, nullable: false),
                    Region = table.Column<string>(maxLength: 64, nullable: false),
                    BinariesVersion = table.Column<string>(maxLength: 64, nullable: false),
                    Version = table.Column<string>(maxLength: 64, nullable: false),
                    FullName = table.Column<string>(maxLength: 64, nullable: false),
                    InstanceType = table.Column<string>(maxLength: 64, nullable: false),
                    StorageSizeGB = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoBookTaskReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PipelineAuditEvents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TimeCreated = table.Column<DateTime>(nullable: false),
                    EventTypeId = table.Column<int>(nullable: false),
                    EventId = table.Column<int>(nullable: false),
                    RunId = table.Column<Guid>(nullable: false),
                    ScenarioId = table.Column<Guid>(nullable: false),
                    Message = table.Column<string>(maxLength: 4096, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PipelineAuditEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AutoBookTaskReports_RunId",
                table: "AutoBookTaskReports",
                column: "RunId");

            migrationBuilder.CreateIndex(
                name: "IX_AutoBookTaskReports_ScenarioId",
                table: "AutoBookTaskReports",
                column: "ScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineAuditEvents_RunId",
                table: "PipelineAuditEvents",
                column: "RunId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineAuditEvents_ScenarioId",
                table: "PipelineAuditEvents",
                column: "ScenarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutoBookTaskReports");

            migrationBuilder.DropTable(
                name: "PipelineAuditEvents");
        }
    }
}

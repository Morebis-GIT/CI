using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT11441BRSImplementation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "BRSIndicator",
                table: "ScenarioResults",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BRSConfigurationTemplateId",
                table: "Runs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BRSConfigurationTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BRSConfigurationTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KPIPriorities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    WeightingFactor = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KPIPriorities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BRSConfigurationForKPIs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BRSConfigurationTemplateId = table.Column<int>(nullable: false),
                    KPIName = table.Column<string>(maxLength: 50, nullable: false),
                    PriorityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BRSConfigurationForKPIs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BRSConfigurationForKPIs_BRSConfigurationTemplates_BRSConfigurationTemplateId",
                        column: x => x.BRSConfigurationTemplateId,
                        principalTable: "BRSConfigurationTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BRSConfigurationForKPIs_BRSConfigurationTemplateId",
                table: "BRSConfigurationForKPIs",
                column: "BRSConfigurationTemplateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BRSConfigurationForKPIs");

            migrationBuilder.DropTable(
                name: "KPIPriorities");

            migrationBuilder.DropTable(
                name: "BRSConfigurationTemplates");

            migrationBuilder.DropColumn(
                name: "BRSIndicator",
                table: "ScenarioResults");

            migrationBuilder.DropColumn(
                name: "BRSConfigurationTemplateId",
                table: "Runs");
        }
    }
}

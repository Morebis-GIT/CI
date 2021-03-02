using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class AddLMKColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResultSource",
                table: "ScenarioResultMetrics",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ExternalRunId",
                table: "RunScenarios",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExternalStatus",
                table: "RunScenarios",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExternalStatusModifiedDate",
                table: "RunScenarios",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResultSource",
                table: "ScenarioResultMetrics");

            migrationBuilder.DropColumn(
                name: "ExternalRunId",
                table: "RunScenarios");

            migrationBuilder.DropColumn(
                name: "ExternalStatus",
                table: "RunScenarios");

            migrationBuilder.DropColumn(
                name: "ExternalStatusModifiedDate",
                table: "RunScenarios");
        }
    }
}

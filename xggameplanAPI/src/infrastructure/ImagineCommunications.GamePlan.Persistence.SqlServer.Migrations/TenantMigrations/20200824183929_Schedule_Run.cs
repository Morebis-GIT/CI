using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class Schedule_Run : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalRunId",
                table: "RunScenarios");

            migrationBuilder.DropColumn(
                name: "ExternalStatus",
                table: "RunScenarios");

            migrationBuilder.DropColumn(
                name: "ExternalStatusModifiedDate",
                table: "RunScenarios");

            migrationBuilder.CreateTable(
                name: "ExternalRunInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RunScenarioId = table.Column<int>(nullable: false),
                    ExternalRunId = table.Column<Guid>(nullable: false),
                    ExternalStatus = table.Column<int>(nullable: false),
                    ExternalStatusModifiedDate = table.Column<DateTime>(nullable: false),
                    QueueName = table.Column<string>(nullable: true),
                    Priority = table.Column<int>(nullable: true),
                    ScheduledDate = table.Column<DateTime>(nullable: true),
                    ScheduledTime = table.Column<TimeSpan>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalRunInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalRunInfo_RunScenarios_RunScenarioId",
                        column: x => x.RunScenarioId,
                        principalTable: "RunScenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalRunInfo_RunScenarioId",
                table: "ExternalRunInfo",
                column: "RunScenarioId",
                unique: true);

            migrationBuilder.DropColumn(
               name: "ScheduledTime",
               table: "ExternalRunInfo");

            migrationBuilder.RenameColumn(
                name: "ScheduledDate",
                table: "ExternalRunInfo",
                newName: "ScheduledDateTime");

            migrationBuilder.DropPrimaryKey(
               name: "PK_ExternalRunInfo",
               table: "ExternalRunInfo");

            migrationBuilder.DropIndex(
                name: "IX_ExternalRunInfo_RunScenarioId",
                table: "ExternalRunInfo");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ExternalRunInfo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExternalRunInfo",
                table: "ExternalRunInfo",
                column: "RunScenarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
               name: "PK_ExternalRunInfo",
               table: "ExternalRunInfo");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ExternalRunInfo",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExternalRunInfo",
                table: "ExternalRunInfo",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalRunInfo_RunScenarioId",
                table: "ExternalRunInfo",
                column: "RunScenarioId",
                unique: true);

            migrationBuilder.RenameColumn(
              name: "ScheduledDateTime",
              table: "ExternalRunInfo",
              newName: "ScheduledDate");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ScheduledTime",
                table: "ExternalRunInfo",
                nullable: true);

            migrationBuilder.DropTable(
                name: "ExternalRunInfo");

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
    }
}

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT14710AddRunScheduleSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RunScheduleSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RunId = table.Column<Guid>(nullable: false),
                    QueueName = table.Column<string>(maxLength: 64, nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RunScheduleSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RunScheduleSettings_Runs_RunId",
                        column: x => x.RunId,
                        principalTable: "Runs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RunScheduleSettings_RunId",
                table: "RunScheduleSettings",
                column: "RunId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RunScheduleSettings");
        }
    }
}

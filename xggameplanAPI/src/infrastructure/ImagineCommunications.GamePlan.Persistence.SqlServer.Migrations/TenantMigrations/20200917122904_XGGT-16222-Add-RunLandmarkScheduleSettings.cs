using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT16222AddRunLandmarkScheduleSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RunLandmarkScheduleSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RunTypeId = table.Column<int>(nullable: false),
                    SendToLandmarkAutomatically = table.Column<bool>(nullable: false),
                    QueueName = table.Column<string>(maxLength: 64, nullable: false),
                    DaysOfWeek = table.Column<string>(maxLength: 7, nullable: false),
                    ScheduledTime = table.Column<long>(nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RunLandmarkScheduleSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RunLandmarkScheduleSettings_RunTypes_RunTypeId",
                        column: x => x.RunTypeId,
                        principalTable: "RunTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RunLandmarkScheduleSettings_RunTypeId",
                table: "RunLandmarkScheduleSettings",
                column: "RunTypeId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RunLandmarkScheduleSettings");
        }
    }
}

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT12952_Consolidated_Landmark_Integration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "NominalPrice",
                table: "Spots",
                type: "DECIMAL(28,18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(18,2)");

            migrationBuilder.AddColumn<double>(
                name: "BreakPrice",
                table: "ScheduleBreaks",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "FloorRate",
                table: "ScheduleBreaks",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BreakPrice",
                table: "Breaks",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "FloorRate",
                table: "Breaks",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "LengthFactors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SalesArea = table.Column<string>(maxLength: 512, nullable: false),
                    Duration = table.Column<long>(nullable: false),
                    Factor = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LengthFactors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SpotBookingRules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SpotLength = table.Column<long>(nullable: false),
                    MinBreakLength = table.Column<long>(nullable: false),
                    MaxBreakLength = table.Column<long>(nullable: false),
                    MaxSpots = table.Column<int>(nullable: false),
                    BreakType = table.Column<string>(maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpotBookingRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SpotBookingRuleSalesAreas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SpotBookingRuleId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpotBookingRuleSalesAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpotBookingRuleSalesAreas_SpotBookingRules_SpotBookingRuleId",
                        column: x => x.SpotBookingRuleId,
                        principalTable: "SpotBookingRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpotBookingRuleSalesAreas_SpotBookingRuleId",
                table: "SpotBookingRuleSalesAreas",
                column: "SpotBookingRuleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LengthFactors");

            migrationBuilder.DropTable(
                name: "SpotBookingRuleSalesAreas");

            migrationBuilder.DropTable(
                name: "SpotBookingRules");

            migrationBuilder.DropColumn(
                name: "BreakPrice",
                table: "ScheduleBreaks");

            migrationBuilder.DropColumn(
                name: "FloorRate",
                table: "ScheduleBreaks");

            migrationBuilder.DropColumn(
                name: "BreakPrice",
                table: "Breaks");

            migrationBuilder.DropColumn(
                name: "FloorRate",
                table: "Breaks");

            migrationBuilder.AlterColumn<decimal>(
                name: "NominalPrice",
                table: "Spots",
                type: "DECIMAL(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(28,18)");
        }
    }
}

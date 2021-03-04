using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.Extensions;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantDbMigrations
{
    public partial class Fts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScheduleBreaks_Summary",
                table: "Breaks");

            migrationBuilder.AlterColumn<double>(
                name: "FloorRate",
                table: "ScheduleBreaks",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<string>(
                name: "TokenizedName",
                table: "Passes",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SearchTokens",
                table: "Campaigns",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "FloorRate",
                table: "Breaks",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double));

            _ = migrationBuilder.CreateFtsFields(TargetModel);

            _ = migrationBuilder.AddFulltextIndex<Run>(TargetModel, Run.SearchField);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SearchTokens",
                table: "Campaigns");

            migrationBuilder.AlterColumn<double>(
                name: "FloorRate",
                table: "ScheduleBreaks",
                nullable: false,
                oldClrType: typeof(double),
                oldDefaultValue: 0.0);

            migrationBuilder.AlterColumn<string>(
                name: "TokenizedName",
                table: "Passes",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "FloorRate",
                table: "Breaks",
                nullable: false,
                oldClrType: typeof(double),
                oldDefaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleBreaks_Summary",
                table: "Breaks",
                columns: new[] { "SalesAreaId", "ScheduledDate", "CustomId", "BreakType", "Duration", "Avail", "OptimizerAvail", "Optimize", "ExternalBreakRef", "Description", "ExternalProgRef", "PositionInProg" });
        }
    }
}

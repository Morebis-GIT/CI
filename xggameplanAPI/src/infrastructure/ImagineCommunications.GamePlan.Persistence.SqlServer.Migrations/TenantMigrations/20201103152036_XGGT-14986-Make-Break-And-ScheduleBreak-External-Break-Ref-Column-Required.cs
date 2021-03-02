using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT14986MakeBreakAndScheduleBreakExternalBreakRefColumnRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScheduleBreaks_ExternalBreakRef",
                table: "ScheduleBreaks");

            migrationBuilder.DropIndex(
                name: "IX_Breaks_ExternalBreakRef",
                table: "Breaks");

            migrationBuilder.AlterColumn<string>(
                name: "ExternalBreakRef",
                table: "ScheduleBreaks",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExternalBreakRef",
                table: "Breaks",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleBreaks_ExternalBreakRef",
                table: "ScheduleBreaks",
                column: "ExternalBreakRef",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Breaks_ExternalBreakRef",
                table: "Breaks",
                column: "ExternalBreakRef",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScheduleBreaks_ExternalBreakRef",
                table: "ScheduleBreaks");

            migrationBuilder.DropIndex(
                name: "IX_Breaks_ExternalBreakRef",
                table: "Breaks");

            migrationBuilder.AlterColumn<string>(
                name: "ExternalBreakRef",
                table: "ScheduleBreaks",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "ExternalBreakRef",
                table: "Breaks",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 64);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleBreaks_ExternalBreakRef",
                table: "ScheduleBreaks",
                column: "ExternalBreakRef",
                unique: true,
                filter: "[ExternalBreakRef] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Breaks_ExternalBreakRef",
                table: "Breaks",
                column: "ExternalBreakRef",
                unique: true,
                filter: "[ExternalBreakRef] IS NOT NULL");
        }
    }
}

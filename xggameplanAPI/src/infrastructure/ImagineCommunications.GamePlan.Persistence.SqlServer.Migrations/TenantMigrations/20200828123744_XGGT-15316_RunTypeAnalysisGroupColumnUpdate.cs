using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT15316_RunTypeAnalysisGroupColumnUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RunTypeAnalysisGroups_RunTypeId_AnalysisGroupId_KPI",
                table: "RunTypeAnalysisGroups");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "RunTypeAnalysisGroups");

            migrationBuilder.AddColumn<string>(
                name: "DefaultKPI",
                table: "RunTypes",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "KPI",
                table: "RunTypeAnalysisGroups",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RunTypeAnalysisGroups_RunTypeId_AnalysisGroupId_KPI",
                table: "RunTypeAnalysisGroups",
                columns: new[] { "RunTypeId", "AnalysisGroupId", "KPI" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RunTypeAnalysisGroups_RunTypeId_AnalysisGroupId_KPI",
                table: "RunTypeAnalysisGroups");

            migrationBuilder.DropColumn(
                name: "DefaultKPI",
                table: "RunTypes");

            migrationBuilder.AlterColumn<string>(
                name: "KPI",
                table: "RunTypeAnalysisGroups",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "RunTypeAnalysisGroups",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_RunTypeAnalysisGroups_RunTypeId_AnalysisGroupId_KPI",
                table: "RunTypeAnalysisGroups",
                columns: new[] { "RunTypeId", "AnalysisGroupId", "KPI" },
                unique: true,
                filter: "[KPI] IS NOT NULL");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT15742_AgencyGroupChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AgencyGroups_ShortName_Code",
                table: "AgencyGroups");

            migrationBuilder.AlterColumn<string>(
                name: "ShortName",
                table: "AgencyGroups",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "AgencyGroups",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.CreateIndex(
                name: "IX_AgencyGroups_ShortName_Code",
                table: "AgencyGroups",
                columns: new[] { "ShortName", "Code" },
                unique: true,
                filter: "[ShortName] IS NOT NULL AND [Code] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AgencyGroups_ShortName_Code",
                table: "AgencyGroups");

            migrationBuilder.AlterColumn<string>(
                name: "ShortName",
                table: "AgencyGroups",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "AgencyGroups",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AgencyGroups_ShortName_Code",
                table: "AgencyGroups",
                columns: new[] { "ShortName", "Code" },
                unique: true);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT14986MakeBreaksExternalBreakRefColumnUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Breaks_ExternalBreakRef",
                table: "Breaks");

            migrationBuilder.CreateIndex(
                name: "IX_Breaks_ExternalBreakRef",
                table: "Breaks",
                column: "ExternalBreakRef",
                unique: true,
                filter: "[ExternalBreakRef] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Breaks_ExternalBreakRef",
                table: "Breaks");

            migrationBuilder.CreateIndex(
                name: "IX_Breaks_ExternalBreakRef",
                table: "Breaks",
                column: "ExternalBreakRef");
        }
    }
}

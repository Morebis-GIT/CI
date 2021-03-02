using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT10718RenamingInventoryStatuses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RunInventoryStatus_Runs_RunId",
                table: "RunInventoryStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RunInventoryStatus",
                table: "RunInventoryStatus");

            migrationBuilder.RenameTable(
                name: "RunInventoryStatus",
                newName: "RunExcludedInventoryStatuses");

            migrationBuilder.RenameIndex(
                name: "IX_RunInventoryStatus_RunId",
                table: "RunExcludedInventoryStatuses",
                newName: "IX_RunExcludedInventoryStatuses_RunId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RunExcludedInventoryStatuses",
                table: "RunExcludedInventoryStatuses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RunExcludedInventoryStatuses_Runs_RunId",
                table: "RunExcludedInventoryStatuses",
                column: "RunId",
                principalTable: "Runs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RunExcludedInventoryStatuses_Runs_RunId",
                table: "RunExcludedInventoryStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RunExcludedInventoryStatuses",
                table: "RunExcludedInventoryStatuses");

            migrationBuilder.RenameTable(
                name: "RunExcludedInventoryStatuses",
                newName: "RunInventoryStatus");

            migrationBuilder.RenameIndex(
                name: "IX_RunExcludedInventoryStatuses_RunId",
                table: "RunInventoryStatus",
                newName: "IX_RunInventoryStatus_RunId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RunInventoryStatus",
                table: "RunInventoryStatus",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RunInventoryStatus_Runs_RunId",
                table: "RunInventoryStatus",
                column: "RunId",
                principalTable: "Runs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

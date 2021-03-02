using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.IntelligenceMigrations
{
    public partial class FK_GroupTransactionInfo_MessageInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MessageInfos_GroupTransactionId",
                table: "MessageInfos",
                column: "GroupTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageInfos_GroupTransactionInfos_GroupTransactionId",
                table: "MessageInfos",
                column: "GroupTransactionId",
                principalTable: "GroupTransactionInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageInfos_GroupTransactionInfos_GroupTransactionId",
                table: "MessageInfos");

            migrationBuilder.DropIndex(
                name: "IX_MessageInfos_GroupTransactionId",
                table: "MessageInfos");
        }
    }
}

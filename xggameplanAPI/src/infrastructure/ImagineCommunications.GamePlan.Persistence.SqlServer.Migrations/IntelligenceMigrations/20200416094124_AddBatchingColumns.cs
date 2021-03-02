using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.IntelligenceMigrations
{
    public partial class AddBatchingColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BatchSize",
                table: "MessageTypes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalBatchCount",
                table: "MessageInfos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProcessedBatchCount",
                table: "MessageInfos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BatchSize",
                table: "MessageTypes");

            migrationBuilder.DropColumn(
                name: "TotalBatchCount",
                table: "MessageInfos");

            migrationBuilder.DropColumn(
                name: "ProcessedBatchCount",
                table: "MessageInfos");
        }
    }
}

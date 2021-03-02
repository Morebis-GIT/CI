using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT12971_Add_SalesAreaGroupName_Field : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SalesAreaGroup",
                table: "ScenarioCampaignFailures",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioCampaignFailures_SalesAreaGroup",
                table: "ScenarioCampaignFailures",
                column: "SalesAreaGroup");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScenarioCampaignFailures_SalesAreaGroup",
                table: "ScenarioCampaignFailures");

            migrationBuilder.DropColumn(
                name: "SalesAreaGroup",
                table: "ScenarioCampaignFailures");
        }
    }
}

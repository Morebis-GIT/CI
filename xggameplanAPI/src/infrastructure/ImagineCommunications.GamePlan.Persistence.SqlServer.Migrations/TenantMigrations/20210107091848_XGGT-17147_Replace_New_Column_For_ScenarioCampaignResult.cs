using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT17147_Replace_New_Column_For_ScenarioCampaignResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Programmes_ProgrammeDictionaryId",
                table: "Programmes");

            migrationBuilder.DropColumn(
                name: "PassesThatDelivered100Percent",
                table: "ScenarioCampaignResults");

            migrationBuilder.AddColumn<int>(
                name: "PassThatDelivered100Percent",
                table: "ScenarioCampaignResults",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Programmes_ProgrammeDictionaryId",
                table: "Programmes",
                column: "ProgrammeDictionaryId")
                .Annotation("SqlServer:Include", new[] { "LiveBroadcast" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Programmes_ProgrammeDictionaryId",
                table: "Programmes");

            migrationBuilder.DropColumn(
                name: "PassThatDelivered100Percent",
                table: "ScenarioCampaignResults");

            migrationBuilder.AddColumn<string>(
                name: "PassesThatDelivered100Percent",
                table: "ScenarioCampaignResults",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Programmes_ProgrammeDictionaryId",
                table: "Programmes",
                column: "ProgrammeDictionaryId");
        }
    }
}

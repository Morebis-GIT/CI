using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT11646_AddIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RestrictionsSalesAreas_SalesArea",
                table: "RestrictionsSalesAreas",
                column: "SalesArea");

            migrationBuilder.CreateIndex(
                name: "IX_Restrictions_EndDate",
                table: "Restrictions",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Restrictions_RestrictionType",
                table: "Restrictions",
                column: "RestrictionType");

            migrationBuilder.CreateIndex(
                name: "IX_Restrictions_StartDate",
                table: "Restrictions",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProgrammesClassifications_Uid",
                table: "ProgrammesClassifications",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_CampaignGroup",
                table: "Campaigns",
                column: "CampaignGroup");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RestrictionsSalesAreas_SalesArea",
                table: "RestrictionsSalesAreas");

            migrationBuilder.DropIndex(
                name: "IX_Restrictions_EndDate",
                table: "Restrictions");

            migrationBuilder.DropIndex(
                name: "IX_Restrictions_RestrictionType",
                table: "Restrictions");

            migrationBuilder.DropIndex(
                name: "IX_Restrictions_StartDate",
                table: "Restrictions");

            migrationBuilder.DropIndex(
                name: "IX_ProgrammesClassifications_Uid",
                table: "ProgrammesClassifications");

            migrationBuilder.DropIndex(
                name: "IX_Campaigns_CampaignGroup",
                table: "Campaigns");
        }
    }
}

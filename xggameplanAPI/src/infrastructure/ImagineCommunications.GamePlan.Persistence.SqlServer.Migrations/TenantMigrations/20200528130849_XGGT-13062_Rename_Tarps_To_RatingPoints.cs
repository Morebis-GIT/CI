using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT13062_Rename_Tarps_To_RatingPoints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PassTarps_PassId",
                table: "PassTarps");

            migrationBuilder.RenameColumn(
                name: "Tarps",
                table: "Recommendations",
                newName: "RatingPoints");

            migrationBuilder.RenameTable(
                name: "PassTarps",
                newName: "PassRatingPoints");

            migrationBuilder.Sql(@"
                exec sp_rename @objname = '[dbo].[PK_PassTarps]', @newname = 'PK_PassRatingPoints'

                exec sp_rename @objname = '[dbo].[FK_PassTarps_Passes_PassId]', @newname = 'FK_PassRatingPoints_Passes_PassId'");

            migrationBuilder.CreateIndex(
                name: "IX_PassRatingPoints_PassId",
                table: "PassRatingPoints",
                column: "PassId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PassRatingPoints_PassId",
                table: "PassRatingPoints");

            migrationBuilder.RenameColumn(
                name: "RatingPoints",
                table: "Recommendations",
                newName: "Tarps");

            migrationBuilder.RenameTable(
                name: "PassRatingPoints",
                newName: "PassTarps");

            migrationBuilder.Sql(@"
                exec sp_rename @objname = '[dbo].[PK_PassRatingPoints]', @newname = 'PK_PassTarps'

                exec sp_rename @objname = '[dbo].[FK_PassRatingPoints_Passes_PassId]', @newname = 'FK_PassTarps_Passes_PassId'");

            migrationBuilder.CreateIndex(
                name: "IX_PassTarps_PassId",
                table: "PassTarps",
                column: "PassId");
        }
    }
}

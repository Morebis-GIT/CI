using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT11509_Recommendations_ProgrammeName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProgrammeName",
                table: "Recommendations",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProgrammeName",
                table: "ProgrammesDictionaries",
                maxLength: 128,
                nullable: true);

            // Seed ProgrammeName from Programmes into ProgrammesDictionaries
            // with same ExternalReference
            migrationBuilder.Sql(@"
                                    UPDATE pd
                                    SET    ProgrammeName = p.ProgrammeName

                                    FROM   (SELECT
                                                   ExternalReference,
                                                   ProgrammeName
                                            FROM   ProgrammesDictionaries
                                            WHERE ProgrammeName is NULL) pd

                                    INNER JOIN   (SELECT DISTINCT
                                                   ExternalReference,
                                                   ProgrammeName
                                            FROM   Programmes) p

                                    ON      pd.ExternalReference = p.ExternalReference
                                   ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProgrammeName",
                table: "Recommendations");

            migrationBuilder.DropColumn(
                name: "ProgrammeName",
                table: "ProgrammesDictionaries");
        }
    }
}

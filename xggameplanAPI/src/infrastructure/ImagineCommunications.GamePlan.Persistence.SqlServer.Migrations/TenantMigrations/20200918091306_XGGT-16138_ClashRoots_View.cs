using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT16138_ClashRoots_View : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql("DROP VIEW IF EXISTS [dbo].[ClashRoots]");

            _ = migrationBuilder.Sql(@"
                CREATE VIEW [dbo].[ClashRoots] AS
                    WITH clash_roots_cte (Externalref, ParentExternalidentifier, RootExternalRef)
                    AS
                    (
                        SELECT
                            c.Externalref, c.ParentExternalidentifier, c.Externalref AS RootExternalRef
                        FROM
                            Clashes c
                        WHERE
                            c.ParentExternalidentifier IS NULL OR TRIM(c.ParentExternalidentifier) = ''
                        UNION ALL
                        SELECT
                            c.Externalref, c.ParentExternalidentifier, cr.RootExternalRef
                        FROM
                            clash_roots_cte AS cr
                        INNER JOIN Clashes c
                            ON cr.Externalref = c.ParentExternalidentifier
                    )
                SELECT
                    cr.Externalref, cr.RootExternalRef
                FROM
                    clash_roots_cte cr");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql("DROP VIEW IF EXISTS [dbo].[ClashRoots]");
        }
    }
}

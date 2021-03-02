using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT14986DeleteDuplicateBreaksByExternalBreakRef : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // For any group of duplicates
            // this will keep the break with the latest Scheduled Date
            // where scheduled dates are equal
            // this will keep the break with the highest Custom ID
            // (the intention being to keep the last one that was added)
            _ = migrationBuilder.Sql(@"
                DECLARE @ExternalBreakRef VARCHAR(max);

                DECLARE EBR_CURSOR CURSOR
                    LOCAL STATIC READ_ONLY FORWARD_ONLY
                FOR
                SELECT ExternalBreakRef
                FROM Breaks
                GROUP BY ExternalBreakRef
                HAVING COUNT(ExternalBreakRef) > 1

                OPEN EBR_CURSOR
                FETCH NEXT FROM EBR_CURSOR INTO @ExternalBreakRef
                WHILE @@FETCH_STATUS = 0
                BEGIN
                    DELETE FROM Breaks WHERE Id IN (
                        SELECT B1.Id
                        FROM Breaks B1
                        WHERE B1.ExternalBreakRef = @ExternalBreakRef
                        EXCEPT
                        SELECT * FROM (
                            SELECT TOP 1 B2.Id
                            FROM Breaks B2
                            WHERE B2.ExternalBreakRef = @ExternalBreakRef
                            ORDER BY B2.ScheduledDate DESC,
                                     B2.CustomId DESC
                        ) BreakToKeep
                    )
                    FETCH NEXT FROM EBR_CURSOR INTO @ExternalBreakRef
                END
                CLOSE EBR_CURSOR
                DEALLOCATE EBR_CURSOR
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT14986DeleteDuplicateScheduleBreaksByExternalBreakRef : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql(@"
                DECLARE @ExternalBreakRef VARCHAR(max);

                DECLARE EBR_CURSOR CURSOR
                    LOCAL STATIC READ_ONLY FORWARD_ONLY
                FOR
                SELECT ExternalBreakRef
                FROM ScheduleBreaks
                GROUP BY ExternalBreakRef
                HAVING COUNT(ExternalBreakRef) > 1

                OPEN EBR_CURSOR
                FETCH NEXT FROM EBR_CURSOR INTO @ExternalBreakRef
                WHILE @@FETCH_STATUS = 0
                BEGIN
                    DELETE FROM ScheduleBreaks WHERE Id IN (
                        SELECT B1.Id
                        FROM ScheduleBreaks B1
                        WHERE B1.ExternalBreakRef = @ExternalBreakRef
                        EXCEPT
                        SELECT * FROM (
                            SELECT TOP 1 B2.Id
                            FROM ScheduleBreaks B2
                            WHERE B2.ExternalBreakRef = @ExternalBreakRef
                            ORDER BY B2.ScheduledDate DESC,
                                     B2.ScheduleId DESC
                        ) ScheduleBreakToKeep
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

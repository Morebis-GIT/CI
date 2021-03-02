using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.OutputFiles;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT12501_AddOutputFile : Migration
    {
        private const string FileId = "XG_CAMP_FAIL_REPT.out";
        private const string Description = "Scenario Campaign Failures";
        private const string AutoBookFileName = "XG_CAMP_FAIL_REPT.out";

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
            IF EXISTS (SELECT TOP 1 1 FROM [dbo].[OutputFiles]) 
            BEGIN
                insert into OutputFiles(
                    {nameof(OutputFile.FileId)},
                    {nameof(OutputFile.Description)},
                    {nameof(OutputFile.AutoBookFileName)})
                values ('{FileId}', '{Description}', '{AutoBookFileName}')
            END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                delete from OutputFiles
                where FileId = '{FileId}'
            ");
        }
    }
}

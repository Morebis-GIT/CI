using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.IntelligenceMigrations
{
    public partial class XGGT_15672_ProgrammeAndBreak_BatchSizes_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE dbo.MessageTypes SET BatchSize = 10 WHERE Id = 'IBulkProgrammeDeleted';
                UPDATE dbo.MessageTypes SET BatchSize = 1 WHERE Id = 'IBulkBreaksDeleted';
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE dbo.MessageTypes SET BatchSize = NULL WHERE Id = 'IBulkProgrammeDeleted';
                UPDATE dbo.MessageTypes SET BatchSize = NULL WHERE Id = 'IBulkBreaksDeleted';
            ");
        }
    }
}

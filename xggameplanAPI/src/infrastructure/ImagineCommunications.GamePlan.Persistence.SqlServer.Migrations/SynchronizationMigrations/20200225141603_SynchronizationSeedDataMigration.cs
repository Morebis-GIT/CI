using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.SynchronizationMigrations
{
    public partial class SynchronizationSeedDataMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO dbo.[SynchronizationObjects] ([Id], [ServiceId], [OwnerCount], [CreatedDate])
                VALUES
                (newid(), NULL, 0, getutcdate());
                GO");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM dbo.[SynchronizationObjects];");
        }
    }
}

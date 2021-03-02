using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.IntelligenceMigrations
{
    public partial class ClashTruncate_message_info : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO dbo.MessageTypes ([Id], [Priority], [Name], [Description], [MessageEntityTypeId])
                VALUES ('IClashTruncated', 5, 'IClashTruncated', null, 5)
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM dbo.MessageTypes WHERE [Id] = 'IClashTruncated'");
        }
    }
}

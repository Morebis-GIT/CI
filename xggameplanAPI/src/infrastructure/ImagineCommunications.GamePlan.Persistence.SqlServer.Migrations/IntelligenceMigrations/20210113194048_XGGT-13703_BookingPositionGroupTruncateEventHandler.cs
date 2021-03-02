using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.IntelligenceMigrations
{
    public partial class XGGT13703_BookingPositionGroupTruncateEventHandler : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql(@"
                INSERT INTO dbo.MessageTypes ([Id], [Priority], [Name], [Description], [MessageEntityTypeId])
                VALUES ('IBookingPositionGroupTruncated', 10, 'BookingPositionGroup Truncated', null, 1)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql("DELETE FROM dbo.MessageTypes WHERE [Id] = 'IBookingPositionGroupTruncated'");
        }
    }
}

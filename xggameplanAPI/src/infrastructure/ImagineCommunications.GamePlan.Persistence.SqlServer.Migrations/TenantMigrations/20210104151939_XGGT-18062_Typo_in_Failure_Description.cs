using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT18062_Typo_in_Failure_Description : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql($"UPDATE [dbo].[FaultTypeDescriptions] " +
                $"SET Description = 'Minimum Break Availability' " +
                $"WHERE Description = 'Miniumum Break Availability';");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql($"UPDATE [dbo].[FaultTypeDescriptions] " +
                $"SET Description = 'Miniumum Break Availability' " +
                $"WHERE Description = 'Minimum Break Availability';");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT16748RunLandmarkScheduleSettingNullableQueueName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql(@"
                    ALTER TABLE RunLandMarkScheduleSettings ALTER COLUMN QueueName nvarchar(64) NULL
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql(@"
                    UPDATE RunLandmarkScheduleSettings
                        SET QueueName = ''
                    WHERE QueueName is NULL

                    ALTER TABLE RunLandMarkScheduleSettings ALTER COLUMN QueueName nvarchar(64) NOT NULL
                ");
        }
    }
}

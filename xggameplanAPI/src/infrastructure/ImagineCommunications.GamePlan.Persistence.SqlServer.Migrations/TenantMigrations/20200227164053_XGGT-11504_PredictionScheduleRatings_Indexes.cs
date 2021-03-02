using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.TenantMigrations
{
    public partial class XGGT11504_PredictionScheduleRatings_Indexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "CREATE NONCLUSTERED INDEX [IX_PredictionScheduleRatings_Demographic] ON [dbo].[PredictionScheduleRatings] ([Demographic]) INCLUDE ([PredictionScheduleId]) WITH (DROP_EXISTING = ON)");
            migrationBuilder.Sql(
                "CREATE NONCLUSTERED INDEX [IX_PredictionScheduleRatings_PredictionScheduleId] ON [dbo].[PredictionScheduleRatings] ([PredictionScheduleId]) INCLUDE ([Time], [Demographic], [NoOfRatings]) WITH (DROP_EXISTING = ON)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_PredictionScheduleRatings_PredictionScheduleId] ON [dbo].[PredictionScheduleRatings] ([PredictionScheduleId]) WITH (DROP_EXISTING = ON)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_PredictionScheduleRatings_Demographic] ON [dbo].[PredictionScheduleRatings] ([Demographic]) WITH (DROP_EXISTING = ON)");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.IntelligenceMigrations
{
    public partial class IntelligenceSeedDataMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT [dbo].[MessagePriorities] ([Id], [Priority])
                VALUES
                (N'IBulkBookingPositionGroupCreated', 9),
                (N'IBulkBookingPositionGroupDeleted', 10),
                (N'IBulkBreakCreated', 5),
                (N'IBulkBreaksDeleted', 6),
                (N'IBulkBreakTypeCreated', 6),
                (N'IBulkBreakTypeDeleted', 7),
                (N'IBulkCampaignCreatedOrUpdated', 0),
                (N'IBulkCampaignDeleted', 10),
                (N'IBulkClashCreatedOrUpdated', 3),
                (N'IBulkClashDeleted', 5),
                (N'IBulkClashExceptionCreated', 2),
                (N'IBulkClashExceptionDeleted', 3),
                (N'IBulkDemographicCreatedOrUpdated', 9),
                (N'IBulkDemographicDeleted', 10),
                (N'IBulkHolidayCreated', 2),
                (N'IBulkHolidayDeleted', 5),
                (N'IBulkInventoryLockCreated', 3),
                (N'IBulkInventoryLockDeleted', 9),
                (N'IBulkInventoryTypeCreated', 2),
                (N'IBulkInventoryTypeDeleted', 7),
                (N'IBulkLockTypeCreated', 4),
                (N'IBulkLockTypeDeleted', 8),
                (N'IBulkProductCreatedOrUpdated', 9),
                (N'IBulkProductDeleted', 10),
                (N'IBulkProgrammeCategoryCreated', 6),
                (N'IBulkProgrammeCategoryDeleted', 7),
                (N'IBulkProgrammeClassificationCreated', 6),
                (N'IBulkProgrammeCreated', 5),
                (N'IBulkProgrammeDeleted', 6),
                (N'IBulkRatingsPredictionSchedulesCreated', 1),
                (N'IBulkRatingsPredictionSchedulesDeleted', 5),
                (N'IBulkRestrictionDeleted', 4),
                (N'IBulkSalesAreaCreatedOrUpdated', 8),
                (N'IBulkSalesAreaDeleted', 9),
                (N'IBulkSpotCreatedOrUpdated', 5),
                (N'IBulkSpotDeleted', 9),
                (N'IBulkTotalRatingCreated', 2),
                (N'IBulkTotalRatingDeleted', 4),
                (N'IBulkUniverseCreated', 1),
                (N'IBulkUniversesDeleted', 2),
                (N'IClashUpdated', 4),
                (N'IDeleteAllProgrammeClassification', 8),
                (N'IDemographicUpdated', 3),
                (N'IPayloadReference', 2),
                (N'IRestrictionCreatedOrUpdated', 3),
                (N'ISalesAreaUpdated', 3)
                GO");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("TRUNCATE TABLE dbo.[MessagePriorities];");
        }
    }
}

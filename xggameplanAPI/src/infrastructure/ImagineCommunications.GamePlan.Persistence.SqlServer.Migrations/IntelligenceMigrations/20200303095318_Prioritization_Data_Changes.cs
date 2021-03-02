using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.IntelligenceMigrations
{
    public partial class Prioritization_Data_Changes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM dbo.MessageEntityTypes;
SET IDENTITY_INSERT dbo.MessageEntityTypes ON

INSERT INTO dbo.MessageEntityTypes (Id, Name, Description) VALUES 
	(1, 'Booking Position Group', null), 
	(2, 'Break', null), 
	(3, 'Break Type', null), 
	(4, 'Campaign', null), 
	(5, 'Clash', null), 
	(6, 'Clash Exception', null), 
	(7, 'Day Part Group', null), 
	(8, 'Day Part', null), 
	(9, 'Demographics', null), 
	(10, 'Holidays', null), 
	(11, 'Inventory Status', null), 
	(12, 'Product', null), 
	(13, 'Programme', null), 
	(14, 'Programme Category', null), 
	(15, 'Programme Classification', null), 
	(16, 'Ratings Prediction Schedules', null), 
	(17, 'Restriction', null), 
	(18, 'Sales Area', null), 
	(19, 'Spot', null), 
	(20, 'Total Rating', null), 
	(21, 'Universe', null),
    (22, 'Spot Booking Rule', null),
    (23, 'Length Factor', null);

SET IDENTITY_INSERT dbo.MessageEntityTypes OFF
DBCC CHECKIDENT ('dbo.MessageEntityTypes', RESEED, 22)

TRUNCATE TABLE dbo.MessageTypes;
INSERT INTO dbo.MessageTypes ([Id], [Priority], [Name], [Description], [MessageEntityTypeId])
VALUES
    ('IBulkClashCreatedOrUpdated', 3, 'Bulk Clash Created Or Updated', null, 5),
    ('IClashUpdated', 4, 'Clash Updated', null, 5),
    ('IBulkClashDeleted', 5, 'Clash Deleted', null, 5),
    ('IBulkUniverseCreated', 0, 'Bulk Universe Created', null, 21),
    ('IBulkUniverseDeleted', 1, 'Universes Deleted', null, 21),
    ('IDemographicUpdated', 3, 'Demographic Updated', null, 9),
    ('IBulkDemographicCreatedOrUpdated', 9, 'Bulk Demographics Created', null, 9),
    ('IBulkDemographicDeleted', 10, 'Demographic Deleted', null, 9),
    ('IBulkRatingsPredictionSchedulesCreated', 1, 'Bulk Ratings Prediction Schedules Created', null, 16),
    ('IBulkRatingsPredictionSchedulesDeleted', 5, 'Bulk Ratings Prediction Schedules Deleted', null, 16),
    ('ISalesAreaUpdated', 3, 'Sales Area Updated', null, 18),
    ('IBulkSalesAreaCreatedOrUpdated', 8, 'Sales Area Created', null, 18),
    ('IBulkSalesAreaDeleted', 9, 'Sales Area Updated', null, 18),
    ('IBulkBreakCreated', 5, 'Bulk Break Created', null, 2),
    ('IBulkBreaksDeleted', 6, 'Bulk Breaks Deleted', null, 2),
    ('IBulkClashExceptionCreated', 2, 'Bulk Clash Exception Created', null, 6),
    ('IBulkClashExceptionDeleted', 3, 'Bulk Clash Exception Deleted', null, 6),
    ('IBulkProgrammeCreated', 5, 'Bulk Programme Created', null, 13),
    ('IBulkProgrammeDeleted', 6, 'Programmes Deleted', null, 13),
    ('IBulkHolidayCreated', 2, 'Holiday Created', null, 10),
    ('IBulkHolidayDeleted', 5, 'Holiday Deleted', null, 10),
    ('IBulkProgrammeClassificationCreated', 6, 'Bulk Programme Classification Created', null, 15),
    ('IDeleteAllProgrammeClassification', 8, 'Delete All Programme Classification', null, 15),
    ('IBulkProductCreatedOrUpdated', 9, 'Product Created Or Updated', null, 12),
    ('IBulkProductDeleted', 10, 'Product Deleted', null, 12),
    ('IBulkRestrictionCreatedOrUpdated', 3, 'Restriction Created Or Updated', null, 17),
    ('IBulkRestrictionDeleted', 4, 'Restriction Deleted', null, 17),
    ('IBulkSpotCreatedOrUpdated', 5, 'Spot Created Or Updated', null, 19),
    ('IBulkSpotDeleted', 9, 'Bulk Spot Deleted', null, 19),
    ('IBulkCampaignCreatedOrUpdated', 0, 'Campaign Created Or Updated', null, 4),
    ('IBulkCampaignDeleted', 10, 'Campaign Deleted', null, 4),
    ('IBulkBookingPositionGroupCreated', 9, 'IBulkBookingPositionGroupCreated', null, 1),
    ('IBulkBookingPositionGroupDeleted', 10, 'IBulkBookingPositionGroupDeleted', null, 1),
    ('IBulkProgrammeCategoryCreated', 6, 'IBulkProgrammeCategoryCreated', null, 14),
    ('IBulkProgrammeCategoryDeleted', 7, 'IBulkProgrammeCategoryDeleted', null, 14),
    ('IBulkLockTypeCreated', 4, 'IBulkLockTypeCreated', null, 11),
    ('IBulkLockTypeDeleted', 8, 'IBulkLockTypeDeleted', null, 11),
    ('IBulkInventoryTypeCreated', 2, 'IBulkInventoryTypeCreated', null, 11),
    ('IBulkInventoryTypeDeleted', 7, 'IBulkInventoryTypeDeleted', null, 11),
    ('IBulkInventoryLockCreated', 3, 'IBulkInventoryLockCreated', null, 11),
    ('IBulkInventoryLockDeleted', 9, 'IBulkInventoryLockDeleted', null, 11),
    ('IBulkTotalRatingCreated', 2, 'IBulkTotalRatingCreated', null, 20),
    ('IBulkTotalRatingDeleted', 4, 'IBulkTotalRatingDeleted', null, 20),
    ('IPayloadReference', 2, 'IPayloadReference', null, 11),
    ('IBulkBreakTypeCreated', 6, 'IBulkBreakTypeCreated', null, 3),
    ('IBulkBreakTypeDeleted', 7, 'IBulkBreakTypeDeleted', null, 3),
    ('IBulkLengthFactorCreated', 6, 'IBulkLengthFactorCreated', null, 22),
    ('IBulkLengthFactorDeleted', 7, 'IBulkLengthFactorDeleted', null, 22),
    ('IBulkSpotBookingRuleCreated', 6, 'Bulk Spot Booking Rule Created', null, 23),
    ('IBulkSpotBookingRuleDeleted', 7, 'Bulk Spot Booking Rule Deleted', null, 23),
    ('IBulkStandardDayPartCreated', 6, 'IBulkStandardDayPartCreated', null, 8),
    ('IBulkStandardDayPartDeleted', 7, 'IBulkStandardDayPartDeleted', null, 8),
    ('IBulkStandardDayPartGroupCreated', 6, 'IBulkStandardDayPartGroupCreated', null, 7),
    ('IBulkStandardDayPartGroupDeleted', 7, 'IBulkStandardDayPartGroupDeleted', null, 7)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("TRUNCATE TABLE dbo.MessageTypes;");
            migrationBuilder.Sql("DELETE FROM dbo.MessageEntityTypes;");
            migrationBuilder.Sql("DBCC CHECKIDENT ('dbo.MessageEntityTypes', RESEED, 1)");
        }
    }
}

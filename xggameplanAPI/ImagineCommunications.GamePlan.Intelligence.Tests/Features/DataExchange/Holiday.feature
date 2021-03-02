@Exchange @SqlServer
Feature: Holiday
	In order to synchronize data with xgGamePlan
	As a landmark user
	I want to send data to GamePlan

Background:
	Given GroupTransactionInfo for 1 event sent

Scenario: BulkHolidayCreated invalid model
	Given The data from file Holiday.SalesArea_Setup exists in database
	And I have BulkHolidayCreated message from file Holiday.BulkHolidayCreated_InvalidModel to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName				   |
		| Data[0].HolidayType          |
		| Data[0].HolidayDateRanges[1] |

Scenario: BulkHolidayCreated with non existing SalesArea
	Given The data from file Holiday.SalesArea_Setup exists in database
	And I have BulkHolidayCreated message from file Holiday.BulkHolidayCreated_WithNonExistingSalesArea to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: SalesAreaNotFound

Scenario: BulkHolidayCreated successfully updates SalesArea
	Given The data from file Holiday.SalesArea_Setup exists in database
	And I have BulkHolidayCreated message from file Holiday.BulkHolidayCreated_ValidData to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Holidays data in GamePlan database is updated as the data from the following file Holiday.SalesArea_Updated

Scenario: BulkHolidayDeleted invalid model
	Given The data from file Holiday.SalesArea_Setup exists in database
	And I have BulkHolidayDeleted message to publish
		| StartDate | EndDate |
		|           |         |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName      |
		| Data[0].StartDate |
		| Data[0].EndDate   |

Scenario: BulkHolidayDeleted successfully deletes data
	Given The data from file Holiday.SalesArea_Setup exists in database
	And I have BulkHolidayDeleted message to publish
		| StartDate                    | EndDate                      |
		| 2020-02-10T00:00:00.0000000Z | 2020-02-25T00:00:00.0000000Z |
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Holidays data in GamePlan database is updated as the data from the following file Holiday.SalesArea_Deleted

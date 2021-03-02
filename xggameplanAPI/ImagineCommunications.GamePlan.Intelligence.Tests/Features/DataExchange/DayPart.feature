@Exchange @SqlServer
Feature: DayPart
	In order to synchronize data with xgGamePlan
	As a landmark user
	I want to send data to GamePlan

Background:
	Given GroupTransactionInfo for 1 event sent

Scenario: BulkStandardDayPartCreated message should successfully save records
	Given The data from file DayParts.Setup exists in database
	Given I have BulkStandardDayPartCreated message from file DayParts.BulkStandardDayPartCreated.Success_Request to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the StandardDayParts data in GamePlan database is updated as the data from the following file DayParts.BulkStandardDayPartCreated.Result

Scenario: BulkStandardDayPartCreated message should throw exception when sales area is not found
	Given The data from file DayParts.Setup exists in database
	Given I have BulkStandardDayPartCreated message from file DayParts.BulkStandardDayPartCreated.Error_SalesAreaNotFound to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: SalesAreaNotFound

Scenario: BulkStandardDayPartCreated message should throw validation exception for empty properties
	Given I have BulkStandardDayPartCreated message from file DayParts.BulkStandardDayPartCreated.Error_InvalidData to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName       |
		| Data[0].DayPartId  |
		| Data[0].SalesArea  |
		| Data[0].Name       |
		| Data[0].Order      |
		| Data[0].Timeslices |

Scenario: BulkStandardDayPartDeleted message should successfully truncate table
	Given The data from file DayParts.Setup exists in database
	And I have BulkStandardDayPartDeleted message to publish
		||
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	Then the StandardDayParts data in GamePlan database is updated as following
		||	


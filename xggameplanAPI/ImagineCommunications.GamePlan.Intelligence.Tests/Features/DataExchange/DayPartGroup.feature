@Exchange @SqlServer
Feature: DayPartGroup
	In order to synchronize data with xgGamePlan
	As a landmark user
	I want to send data to GamePlan

Background:
	Given GroupTransactionInfo for 1 event sent


Scenario: BulkStandardDayPartCreated message should successfully save records
	Given The data from file DayPartGroups.Setup exists in database
	Given I have BulkStandardDayPartGroupCreated message from file DayPartGroups.BulkStandardDayPartGroupCreated.Success_Request to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the StandardDayPartGroups data in GamePlan database is updated as the data from the following file DayPartGroups.BulkStandardDayPartGroupCreated.Result


Scenario: BulkStandardDayPartGroupCreated message should throw exception when sales area is not found
	Given The data from file DayPartGroups.Setup exists in database
	Given I have BulkStandardDayPartGroupCreated message from file DayPartGroups.BulkStandardDayPartGroupCreated.Error_SalesAreaNotFound to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: SalesAreaNotFound


Scenario: BulkStandardDayPartGroupCreated message should throw exception when demographic is not found
	Given The data from file DayPartGroups.Setup exists in database
	Given I have BulkStandardDayPartGroupCreated message from file DayPartGroups.BulkStandardDayPartGroupCreated.Error_DemographicNotFound to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: DemographicNotFound


Scenario: BulkStandardDayPartGroupCreated message should throw exception when DayPart is not found
	Given The data from file DayPartGroups.Setup exists in database
	Given I have BulkStandardDayPartGroupCreated message from file DayPartGroups.BulkStandardDayPartGroupCreated.Error_DayPartNotFound to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: DayPartNotFound

Scenario: BulkStandardDayPartGroupCreated message should throw validation exception for empty properties
	Given I have BulkStandardDayPartGroupCreated message from file DayPartGroups.BulkStandardDayPartGroupCreated.Error_InvalidData to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName              |
		| Data[0].GroupId           |
		| Data[0].SalesArea         |


Scenario: BulkStandardDayPartGroupDeleted message should successfully truncate table
	Given The data from file DayPartGroups.Setup exists in database
	And I have BulkStandardDayPartGroupDeleted message to publish
		||
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	Then the StandardDayPartGroups data in GamePlan database is updated as following
		||	


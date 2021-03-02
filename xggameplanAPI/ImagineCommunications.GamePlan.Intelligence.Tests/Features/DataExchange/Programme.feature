@Exchange @SqlServer
Feature: Programme
	In order to synchronize data with xgGamePlan
	As a landmark user
	I want to send data to GamePlan

Background:
	Given GroupTransactionInfo for 1 event sent


Scenario: BulkProgrammeCreated message should successfully save records
	Given The data from file Programme.BulkProgrammeCreated.SalesAreas_Setup exists in database
	And The data from file Programme.BulkProgrammeCreated.Setup exists in database
	And I have BulkProgrammeCreated message from file Programme.BulkProgrammeCreated.SuccessPath to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Programmes data in GamePlan database is updated as the data from the following file Programme.BulkProgrammeCreated.Result
	And the Schedules data in GamePlan database is updated as the data from the following file Programme.BulkProgrammeCreated.Result

Scenario: BulkProgrammeCreated message should throw validation exception for empty properties
	Given I have BulkProgrammeCreated message from file Programme.BulkProgrammeCreated.ErrorPath to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName              |
		| Data[0].SalesArea         |
		| Data[0].ExternalReference |
		| Data[0].ProgrammeName     |
		| Data[0].StartDateTime     |
		| Data[0].Duration          |


Scenario: BulkProgrammeCreated message should throw invalid data exception for invalid sales area name
	Given The data from file Programme.BulkProgrammeCreated.SalesAreas_Setup exists in database
	And The data from file Programme.BulkProgrammeCreated.Setup exists in database
	And I have BulkProgrammeCreated message from file Programme.BulkProgrammeCreated.Error_InvalidSalesArea to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: SalesAreaNotFound


Scenario: BulkProgrammeCreated message should throw invalid data exception for invalid program categories
	Given The data from file Programme.BulkProgrammeCreated.SalesAreas_Setup exists in database
	And The data from file Programme.BulkProgrammeCreated.Setup exists in database
	And I have BulkProgrammeCreated message from file Programme.BulkProgrammeCreated.Error_InvalidProgramCategories to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: ProgrammeCategoryNotFound


Scenario: BulkProgrammeDeleted message should successfully delete records
	Given The data from file Programme.BulkProgrammeDeleted.SalesAreas_Setup exists in database
	And The data from file Programme.BulkProgrammeDeleted.Setup exists in database
	And I have BulkProgrammeDeleted message from file Programme.BulkProgrammeDeleted.SuccessPath to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Programmes data in GamePlan database is updated as the data from the following file Programme.BulkProgrammeDeleted.Result

Scenario: BulkProgrammeDeleted message should throw validation exception for empty properties
	Given I have BulkProgrammeDeleted message from file Programme.BulkProgrammeDeleted.ErrorPath to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName      |
		| Data[0].SalesArea |
		| Data[0].FromDate  |
		| Data[0].ToDate    |


Scenario: BulkProgrammeUpdated message should successfully update records
	Given The data from file Programme.BulkProgrammeUpdated.Setup exists in database
	And I have BulkProgrammeUpdated message from file Programme.BulkProgrammeUpdated.Success_Request to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Programmes data in GamePlan database is updated as the data from the following file Programme.BulkProgrammeUpdated.Success_Result

Scenario: BulkProgrammeUpdated message should throw validation exception for empty properties
	Given I have BulkProgrammeUpdated message from file Programme.BulkProgrammeUpdated.Error_InvalidData to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName              |
		| Data[0].ExternalReference |
		| Data[0].ProgrammeName     |

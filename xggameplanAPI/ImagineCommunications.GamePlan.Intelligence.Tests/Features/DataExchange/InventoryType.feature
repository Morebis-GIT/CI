@Exchange @SqlServer
Feature: InventoryType
	In order to synchronize data with xgGamePlan
	As a landmark user
	I want to send data to GamePlan

Background:
	Given GroupTransactionInfo for 1 event sent

Scenario: BulkInventoryTypeCreated message should successfully save records
	Given The data from file InventoryStatus.BulkInventoryTypeCreated.Setup exists in database
	And I have BulkInventoryTypeCreated message from file InventoryStatus.BulkInventoryTypeCreated.SuccessPath to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the InventoryTypes data in GamePlan database is updated as the data from the following file InventoryStatus.BulkInventoryTypeCreated.Result

Scenario: BulkInventoryTypeCreated message should throw invalid data exception for invalid lock types
	Given The data from file InventoryStatus.BulkInventoryTypeCreated.Setup exists in database
	And I have BulkInventoryTypeCreated message from file InventoryStatus.BulkInventoryTypeCreated.Error_InvalidLockTypes to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: LockTypeNotFound

Scenario: BulkInventoryDeleted message should successfully truncate table
	Given The data from file InventoryStatus.BulkInventoryTypeCreated.Setup exists in database
	And I have BulkInventoryTypeDeleted message to publish
		||
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	Then the InventoryTypes data in GamePlan database is updated as following
		||

Scenario: BulkInventoryTypeCreated invalid model
	Given I have BulkInventoryTypeCreated message to publish
		| Data |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName |
		| Data         |

Scenario: BulkInventoryTypeCreated model fields are invalid
	Given I have BulkInventoryTypeCreated message from file InventoryStatus.BulkInventoryTypeCreated.Error_InvalidData to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName          |
		| Data[0].Description   |
		| Data[0].InventoryCode |
		| Data[0].System        |

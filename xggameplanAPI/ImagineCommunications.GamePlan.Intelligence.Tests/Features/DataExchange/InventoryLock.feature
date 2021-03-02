@Exchange @SqlServer
Feature: InventoryLock
	In order to synchronize data with xgGamePlan
	As a landmark user
	I want to send data to GamePlan

Background:
	Given GroupTransactionInfo for 1 event sent


Scenario: BulkInventoryLockCreated message should successfully save records
	Given The data from file SalesArea.ExistingSalesAreas exists in database
	And The data from file InventoryStatus.BulkInventoryLockCreated.Setup exists in database
	And I have BulkInventoryLockCreated message from file InventoryStatus.BulkInventoryLockCreated.SuccessPath to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the InventoryLocks data in GamePlan database is updated as the data from the following file InventoryStatus.BulkInventoryLockCreated.Result


Scenario: BulkInventoryLockCreated message should throw exception when sales area is not found
	Given The data from file SalesArea.ExistingSalesAreas exists in database
	And The data from file InventoryStatus.BulkInventoryLockCreated.Setup exists in database
	Given I have BulkInventoryLockCreated message from file InventoryStatus.BulkInventoryLockCreated.Error_SalesAreaNotFound to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: SalesAreaNotFound

Scenario: BulkInventoryLockDeleted should successfully delete records
	Given The data from file SalesArea.ExistingSalesAreas exists in database
	And The data from file InventoryStatus.BulkInventoryLockCreated.Setup exists in database
	Given I have BulkInventoryLockDeleted message from file InventoryStatus.BulkInventoryLockDeleted.SuccessPath to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the InventoryLocks data in GamePlan database is updated as the data from the following file InventoryStatus.BulkInventoryLockDeleted.Result

Scenario: BulkInventoryLockCreated invalid model
	Given I have BulkInventoryLockCreated message to publish
		| Data |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName |
		| Data         |

Scenario: BulkInventoryLockDeleted invalid model
	Given I have BulkInventoryLockDeleted message to publish
		| Data |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName |
		| Data         |



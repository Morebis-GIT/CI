@Exchange @SqlServer
Feature: LockType
	In order to synchronize data with xgGamePlan
	As a landmark user
	I want to send data to GamePlan

Background:
	Given GroupTransactionInfo for 1 event sent

Scenario: BulkLockTypeCreated messages should successfully save records
	Given I have BulkLockTypeCreated message to publish
		| LockType | Name       |
		| 1        | BREAKS     |
		| 2        | PROGRAMMES | 
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the LockTypes data in GamePlan database is updated as following
		| LockType | Name       |
		| 1        | BREAKS     |
		| 2        | PROGRAMMES | 

Scenario: BulkLockTypeDeleted message should successfully truncate table
	Given The data from file InventoryStatus.LockType_Setup exists in database
	And I have BulkLockTypeDeleted message to publish
		||
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	Then the LockTypes data in GamePlan database is updated as following
		||	

Scenario: BulkLockTypeCreated invalid model
	Given I have BulkLockTypeCreated message to publish
		| Data |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName |
		| Data         |

Scenario: BulkLockTypeCreated model fileds are invalid
	Given I have BulkLockTypeCreated message to publish
		| LockType | Name  |
		|          | Break |
		| 1        |       |       
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName     |
		| Data[0].LockType |
		| Data[1].Name     |

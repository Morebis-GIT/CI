@Exchange @SqlServer
Feature: BreakType
	In order to synchronize data with xgGamePlan
	As a landmark user
	I want to send data to GamePlan

Background:
	Given GroupTransactionInfo for 1 event sent

Scenario: BulkBreakTypeCreated messages should successfully save records
	Given I have BulkBreakTypeCreated message from file BreakTypes.BulkBreakTypeCreated.Success_Request to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Metadatas data in GamePlan database is updated as the data from the following file BreakTypes.BulkBreakTypeCreated.Success_Result

Scenario: BulkBreakTypeCreated messages should successfully add new Break Types to existing
	Given The data from file BreakTypes.BulkBreakTypeCreated.WithExistingBreakTypes_Setup exists in database
	Given I have BulkBreakTypeCreated message from file BreakTypes.BulkBreakTypeCreated.Success_Request to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Metadatas data in GamePlan database is updated as the data from the following file BreakTypes.BulkBreakTypeCreated.WithExistingBreakTypes_Success_Result

Scenario: BulkBreakTypeDeleted message should successfully remove Break Types from Metadata
	Given The data from file BreakTypes.BulkBreakTypeDeleted.Setup exists in database
	And I have BulkBreakTypeDeleted message to publish
		||
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	Then the Metadatas data in GamePlan database is updated as the data from the following file BreakTypes.BulkBreakTypeDeleted.Success_Result

Scenario: BulkBreakTypeCreated invalid model
	Given I have BulkBreakTypeCreated message to publish
		| Data |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName |
		| Data         |

Scenario: BulkBreakTypeCreated model fileds are invalid
	Given I have BulkBreakTypeCreated message to publish
		| Name |
		|      |       
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName |
		| Data[0].Name |

@Exchange @SqlServer
Feature: LengthFactor
	In order to synchronize data with xgGamePlan
	As a landmark user
	I want to send data to GamePlan

Background:
	Given GroupTransactionInfo for 1 event sent

Scenario: BulkLengthFactorCreated invalid model
	Given I have BulkLengthFactorCreated message to publish
		| Data |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName |
		| Data         |

Scenario: BulkLengthFactorCreated success case
	Given The data from file LengthFactor.LengthFactor_Setup exists in database
	And I have BulkLengthFactorCreated message from file LengthFactor.BulkLengthFactorCreated_SuccessRequest to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the LengthFactors data in GamePlan database is updated as the data from the following file LengthFactor_Data

Scenario: BulkLengthFactorDeleted message should successfully truncate table
	Given The data from file LengthFactor.LengthFactor_Setup exists in database
	And I have BulkLengthFactorDeleted message to publish
		||
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	Then the LengthFactors data in GamePlan database is updated as following
		||

@Exchange @SqlServer
Feature: SpotBookingRule
	In order to synchronize data with xgGamePlan
	As a landmark user
	I want to send data to GamePlan

Background:
	Given GroupTransactionInfo for 1 event sent


Scenario: BulkSpotBookingRuleCreated message should successfully save records
	Given The data from file SpotBookingRules.Setup exists in database
	Given I have BulkSpotBookingRuleCreated message from file SpotBookingRules.BulkSpotBookingRuleCreated.Success_Request to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the SpotBookingRules data in GamePlan database is updated as the data from the following file SpotBookingRules.BulkSpotBookingRuleCreated.Success_Result


Scenario: BulkSpotBookingRuleCreated message should throw exception when sales area is not found
	Given The data from file SpotBookingRules.Setup exists in database
	Given I have BulkSpotBookingRuleCreated message from file SpotBookingRules.BulkSpotBookingRuleCreated.Error_SalesAreaNotFound to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: SalesAreaNotFound


Scenario: BulkSpotBookingRuleCreated message should throw exception when break type is not found
	Given The data from file SpotBookingRules.Setup exists in database
	Given I have BulkSpotBookingRuleCreated message from file SpotBookingRules.BulkSpotBookingRuleCreated.Error_BreakTypeNotFound to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: InvalidBreakType

Scenario: BulkSpotBookingRuleCreated invalid model
	Given I have BulkSpotBookingRuleCreated message to publish
		| Data |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName |
		| Data         |


Scenario: BulkSpotBookingRuleDeleted message should successfully truncate table
	Given The data from file SpotBookingRules.Setup exists in database
	And I have BulkSpotBookingRuleDeleted message to publish
		||
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	Then the SpotBookingRules data in GamePlan database is updated as following
		||	
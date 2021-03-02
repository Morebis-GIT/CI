@Exchange @SqlServer
Feature: BookingPositionGroup
	In order to synchronize data with xgGamePlan
	As a landmark user
	I want to send data to GamePlan

Background:
	Given GroupTransactionInfo for 1 event sent

Scenario: BulkBookingPositionGroupCreated invalid model
	Given I have BulkBookingPositionGroupCreated message to publish
		| Data |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName |
		| Data         |

Scenario: BulkBookingPositionGroupCreated success case
	Given I have BulkBookingPositionGroupCreated message from file BookingPositionGroup.BulkBookingPositionGroupCreated.Success_Request to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the BookingPositionGroups data in GamePlan database is updated as the data from the following file BookingPositionGroup.BulkBookingPositionGroupCreated.Success_Result

Scenario: BulkBookingPositionGroupDeleted invalid model
	Given I have BulkBookingPositionGroupDeleted message to publish
		| Data |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName |
		| Data         |

Scenario: BulkBookingPositionGroupDeleted success case
	Given The data from file BookingPositionGroup.BulkBookingPositionGroupDeleted.Setup exists in database
	Given I have BulkBookingPositionGroupDeleted message from file BookingPositionGroup.BulkBookingPositionGroupDeleted.Success_Request to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the BookingPositionGroups data in GamePlan database is updated as the data from the following file BookingPositionGroup.BulkBookingPositionGroupDeleted.Success_Result

Scenario: BookingPositionGroupTruncated success case
	Given The data from file BookingPositionGroup.BulkBookingPositionGroupDeleted.Setup exists in database
	And I have BookingPositionGroupTruncated message to publish
		||
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the BookingPositionGroups data in GamePlan database is updated as following
		||

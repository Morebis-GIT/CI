@Exchange @SqlServer
Feature: Product
	In order to synchronize data with xgGamePlan
	As a landmark user
	I want to send data to GamePlan

Background:
	Given GroupTransactionInfo for 1 event sent

Scenario: BulkProductCreatedOrUpdated invalid model
	Given I have BulkProductCreatedOrUpdated message from file Product.BulkProductCreatedOrUpdated.InvalidData to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName                 |
		| Data[0].Externalidentifier   |
		| Data[0].Name                 |
		| Data[0].EffectiveStartDate   |
		| Data[0].ClashCode            |

Scenario: BulkProductCreatedOrUpdated invalid date range
	Given I have BulkProductCreatedOrUpdated message from file Product.BulkProductCreatedOrUpdated.InvalidEffectiveDateRange to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName                                     |
		| Data[0].ProductCreatedOrUpdated_InvalidDateRange |

Scenario: BulkProductCreatedOrUpdated successfully creates Product
	Given  I have BulkProductCreatedOrUpdated message from file Product.BulkProductCreatedOrUpdated.CreateSuccess_Request to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Products data in GamePlan database is updated as the data from the following file Product.BulkProductCreatedOrUpdated.CreateSuccess_Result

Scenario: BulkProductCreatedOrUpdated successfully updates Product
	Given The data from file Product.Setup exists in database
	And I have BulkProductCreatedOrUpdated message from file Product.BulkProductCreatedOrUpdated.UpdateSuccess_Request to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Products data in GamePlan database is updated as the data from the following file Product.BulkProductCreatedOrUpdated.UpdateSuccess_Result

Scenario: BulkProductDeleted invalid model
	Given  I have BulkProductDeleted message to publish
		| Externalidentifier |
		|                    |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName       |
		| Data[0].Externalidentifier |

Scenario: BulkProductDeleted successfully deletes Product
	Given The data from file Product.Setup exists in database
	And I have BulkProductDeleted message to publish
		| Externalidentifier |
		| ext1               |
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Products data in GamePlan database is updated as the data from the following file Product.BulkProductDeleted.DeletedSuccess_Result

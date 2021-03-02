@Exchange @SqlServer
Feature: ClashException
	In order to synchronize data with xgGamePlan
	As a landmark user
	I want to send data to GamePlan

Background:
	Given GroupTransactionInfo for 1 event sent

Scenario: BulkClashExceptionCreated message should throw model validation exception for empty data
	And I have BulkClashExceptionCreated message to publish
		||
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName |
		| Data         |

Scenario: BulkClashexceptionCreated message should throw model validation exception for properties
	Given I have BulkClashExceptionCreated message from file ClashException.BulkClashExceptionCreated.Error_InvalidData to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName                 |
		| Data[0].EndDate              |
		| Data[0].FromValue            |
		| Data[0].ToValue              |
		| Data[0].FromToTypeAdvertiser |
		| Data[0].TimeAndDows          |

Scenario: BulkClashexceptionCreated message should throw model validation exception for time and dow properties
	Given I have BulkClashExceptionCreated message from file ClashException.BulkClashExceptionCreated.Error_InvalidData_TimeAndDow to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName                      |
		| Data[0].TimeAndDows[0].DaysOfWeek |

Scenario: BulkClashexceptionCreated message should throw exception when clash code is not found
	Given The data from file ClashException.BulkClashExceptionCreated.Setup exists in database
	And I have BulkClashExceptionCreated message from file ClashException.BulkClashExceptionCreated.Error_InvalidClashCode to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: ClashCodeNotFound

Scenario: BulkClashexceptionCreated message should throw exception when product is not found
	Given The data from file ClashException.BulkClashExceptionCreated.Setup exists in database
	And I have BulkClashExceptionCreated message from file ClashException.BulkClashExceptionCreated.Error_InvalidProduct to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: ProductNotFound

Scenario: BulkClashexceptionCreated message should throw exception when product advertiser is not found
	Given The data from file ClashException.BulkClashExceptionCreated.Setup exists in database
	And I have BulkClashExceptionCreated message from file ClashException.BulkClashExceptionCreated.Error_InvalidProductAdvertiser to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: ProductAdvertiserNotFound

Scenario: BulkClashexceptionCreated message should throw exception when clashes' structure rules violate
	Given The data from file ClashException.BulkClashExceptionCreated.Setup exists in database
	And I have BulkClashExceptionCreated message from file ClashException.BulkClashExceptionCreated.Error_StructureRulesViolation to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: ClashException_StructureRulesViolation

Scenario: BulkClashexceptionCreated message should throw exception when date ranges overlap
	Given The data from file ClashException.BulkClashExceptionCreated.Setup exists in database
	And I have BulkClashExceptionCreated message from file ClashException.BulkClashExceptionCreated.Error_TimeRangesOverlap to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: ClashException_TimeRangesOverlap

Scenario: BulkClashexceptionCreated message should successfully save record to the database
	Given The data from file ClashException.BulkClashExceptionCreated.Setup exists in database
	And I have BulkClashExceptionCreated message from file ClashException.BulkClashExceptionCreated.Success to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the ClashExceptions data in GamePlan database is updated as the data from the following file ClashException.BulkClashExceptionCreated.Success_Result

Scenario: BulkClashExceptionDeleted message should throw model validation exception for properties
	Given I have BulkClashExceptionDeleted message to publish
		| Data |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName |
		| Data         |

Scenario: BulkClashExceptionDeleted message should not find records in the database to delete
	Given The data from file ClashException.BulkClashExceptionDeleted.Setup exists in database
	Given I have BulkClashExceptionDeleted message from file ClashException.BulkClashExceptionDeleted.NotFound_Request to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the ClashExceptions data in GamePlan database is updated as the data from the following file ClashException.BulkClashExceptionDeleted.Success_NotFound_Result

Scenario: BulkClashExceptionDeleted message should successfully delete records from the database
	Given The data from file ClashException.BulkClashExceptionDeleted.Setup exists in database
	Given I have BulkClashExceptionDeleted message from file ClashException.BulkClashExceptionDeleted.Success_Request to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the ClashExceptions data in GamePlan database is updated as the data from the following file ClashException.BulkClashExceptionDeleted.Success_Result

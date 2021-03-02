@Exchange @SqlServer
Feature: Spot
    In order to synchronize data with xgGamePlan
    As a landmark user
    I want to send data to GamePlan

Background:
	Given GroupTransactionInfo for 1 event sent

Scenario: BulkSpotCreatedOrUpdated message should throw exception for invalid end date
	Given I have BulkSpotCreatedOrUpdated message to publish
		| ExternalSpotRef | StartDateTime | EndDateTime |
		| test            |               |             |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName        |
		| Data[0].EndDateTime |

Scenario: BulkSpotCreatedOrUpdated message should throw exception for invalid external spot reference
	Given I have BulkSpotCreatedOrUpdated message to publish
		| ExternalSpotRef | StartDateTime | EndDateTime               |
		|                 |               | 2019-09-17T00:00:00+04:00 |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName            |
		| Data[0].ExternalSpotRef |

Scenario: BulkSpotCreatedOrUpdated message should throw exception for invalid end date when passed EndDateTime is less than StartDateTime
	Given I have BulkSpotCreatedOrUpdated message to publish
		| ExternalSpotRef | StartDateTime             | EndDateTime               |
		| test            | 2019-09-17T00:00:00+04:00 | 2019-09-15T00:00:00+04:00 |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName        |
		| Data[0].EndDateTime |

Scenario: BulkSpotCreatedOrUpdated message should create new record without break reference
	Given The data from file Spot.BulkSpotCreatedOrUpdated.SalesAreas_Setup exists in database
	And The data from file Spot.BulkSpotCreatedOrUpdated.Setup exists in database
	And I have BulkSpotCreatedOrUpdated message from file Spot.BulkSpotCreatedOrUpdated.Success_Create_WithoutBreak to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Spots data in GamePlan database is updated as the data from the following file Spot.BulkSpotCreatedOrUpdated.Success_Create_WithoutBreak_Result

Scenario: BulkSpotCreatedOrUpdated message should create new record with break reference
	Given The data from file Spot.BulkSpotCreatedOrUpdated.SalesAreas_Setup exists in database
	And The data from file Spot.BulkSpotCreatedOrUpdated.Setup exists in database
	And I have BulkSpotCreatedOrUpdated message from file Spot.BulkSpotCreatedOrUpdated.Success_Create_WithBreak to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Spots data in GamePlan database is updated as the data from the following file Spot.BulkSpotCreatedOrUpdated.Success_Create_WithBreak_Result

Scenario: BulkSpotCreatedOrUpdated message should update record
	Given The data from file Spot.BulkSpotCreatedOrUpdated.SalesAreas_Setup exists in database
	And The data from file Spot.BulkSpotCreatedOrUpdated.Setup exists in database
	And I have BulkSpotCreatedOrUpdated message from file Spot.BulkSpotCreatedOrUpdated.Success_Update to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Spots data in GamePlan database is updated as the data from the following file Spot.BulkSpotCreatedOrUpdated.Success_Update_Result

Scenario: BulkSpotDeleted message should throw validation exception for empty propeties
	And I have BulkSpotDeleted message from file Spot.BulkSpotDeleted.Error_Validation_Data to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName |
		| Data         |

Scenario: BulkSpotDeleted message should throw validation exception for empty data
	And I have BulkSpotDeleted message from file Spot.BulkSpotDeleted.Error_Validation_Properties to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName            |
		| Data[1].ExternalSpotRef |

Scenario: BulkSpotDeleted message should delete single non multipart record from database
	Given The data from file Spot.BulkSpotDeleted.Setup exists in database
	And I have BulkSpotDeleted message from file Spot.BulkSpotDeleted.Success_NonMultipart to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Spots data in GamePlan database is updated as the data from the following file Spot.BulkSpotDeleted.Success_NonMultipart_Result

Scenario: BulkSpotDeleted message should delete toptail multipart records from database
	Given The data from file Spot.BulkSpotDeleted.Setup exists in database
	And I have BulkSpotDeleted message from file Spot.BulkSpotDeleted.Success_TopTailDelete to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Spots data in GamePlan database is updated as the data from the following file Spot.BulkSpotDeleted.Success_TopTailDelete_Result

Scenario: BulkSpotDeleted message should delete samebreak multipart records from database
	Given The data from file Spot.BulkSpotDeleted.Setup exists in database
	And I have BulkSpotDeleted message from file Spot.BulkSpotDeleted.Success_SameBreakDelete to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Spots data in GamePlan database is updated as the data from the following file Spot.BulkSpotDeleted.Success_SameBreakDelete_Result

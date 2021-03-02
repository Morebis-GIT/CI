@Exchange @SqlServer
Feature: SalesArea
    In order to synchronize data with xgGamePlan
    As a landmark user
    I want to send data to GamePlan

Background:
	Given GroupTransactionInfo for 1 event sent

#CREATE
Scenario: BulkSalesAreaCreatedOrUpdated message should throw validation exception for empty properties
	Given I have BulkSalesAreaCreatedOrUpdated message to publish
		| Name | ShortName | CurrencyCode | BaseDemographic1 | BaseDemographic2 | ChannelGroup | StartOffset      | DayDuration      | Demographics |
		|      |           |              |                  |                  |              | 00:00:00.0000015 | 00:00:00.0000000 |              |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName            |
		| Data[0].Name            |
		| Data[0].ShortName       |
		| Data[0].CurrencyCode    |
		| Data[0].BaseDemographic1 |
		| Data[0].BaseDemographic2 |
		| Data[0].ChannelGroup     |
		| Data[0].DayDuration      |
		| Data[0].Demographics     |

Scenario: BulkSalesAreaCreatedOrUpdated message should successfully save record in the database
	Given The data from file SalesArea.Setup exists in database
	And I have BulkSalesAreaCreatedOrUpdated message from file SalesArea.BulkSalesAreaCreatedOrUpdated.SuccessCreate_Request to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the SalesAreas data in GamePlan database is updated as the data from the following file SalesArea.BulkSalesAreaCreatedOrUpdated.SuccessCreate_Result

#UPDATE
Scenario: BulkSalesAreaCreatedOrUpdated message should throw exception when demographic is not found
	Given The data from file SalesArea.Setup exists in database
	And I have BulkSalesAreaCreatedOrUpdated message from file SalesArea.BulkSalesAreaCreatedOrUpdated.InvalidDemographic_Request to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: DemographicNotFound

Scenario: BulkSalesAreaCreatedOrUpdated message should successfully update record in the database
	Given The data from file SalesArea.Setup exists in database
	And I have BulkSalesAreaCreatedOrUpdated message from file SalesArea.BulkSalesAreaCreatedOrUpdated.SuccessUpdate_Request to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the SalesAreas data in GamePlan database is updated as the data from the following file SalesArea.BulkSalesAreaCreatedOrUpdated.SuccessUpdate_Result

Scenario: SalesAreaUpdated message should throw validation exception for empty properties
	Given I have SalesAreaUpdated message to publish
		| Name | ShortName | CurrencyCode | BaseDemographic1 | BaseDemographic2 | ChannelGroup | StartOffset      | DayDuration      | Demographics |
		|      |           |              |                  |                  |              | 00:00:00.0000015 | 00:00:00.0000000 |              |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName     |
		| Name             |
		| ShortName        |
		| CurrencyCode     |
		| BaseDemographic1 |
		| BaseDemographic2 |
		| ChannelGroup     |
		| DayDuration      |
		| Demographics     |

Scenario: SalesAreaUpdated message should throw exception when sales area is not found
	Given I have SalesAreaUpdated message from file SalesArea.SalesAreaUpdated.Success_Request to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: SalesAreaNotFound

Scenario: SalesAreaUpdated message should throw exception when demographic is not found
	Given The data from file SalesArea.Setup exists in database
	And I have SalesAreaUpdated message from file SalesArea.SalesAreaUpdated.InvalidDemographic_Request to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: DemographicNotFound

Scenario: SalesAreaUpdated message should successfully update record in the database
	Given The data from file SalesArea.Setup exists in database
	And I have SalesAreaUpdated message from file SalesArea.SalesAreaUpdated.Success_Request to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the SalesAreas data in GamePlan database is updated as the data from the following file SalesArea.SalesAreaUpdated.Success_Result
#DELETE
Scenario: BulkSalesAreaDeleted message should throw validation exception for empty properties
	Given I have BulkSalesAreaDeleted message to publish
		| ShortName |
		|           |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName      |
		| Data[0].ShortName |

Scenario: BulkSalesAreaDeleted message should delete record with specified short name from the database
	Given The data from file SalesArea.Setup exists in database
	And I have BulkSalesAreaDeleted message to publish
		| ShortName |
		| GTV 9     |
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the SalesAreas data in GamePlan database is updated as the data from the following file SalesArea.BulkSalesAreaDeleted.Success_Result

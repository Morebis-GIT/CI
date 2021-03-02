@Exchange @SqlServer
Feature: TotalRating
	In order to synchronize data with xgGamePlan
	As a landmark user
	I want to send data to GamePlan

Background:
	Given GroupTransactionInfo for 1 event sent


Scenario: BulkTotalRatingCreated message should successfully save records
	Given The data from file TotalRatings.Setup exists in database
	Given I have BulkTotalRatingCreated message from file TotalRatings.BulkTotalRatingCreated.Success_Request to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the TotalRatings data in GamePlan database is updated as the data from the following file TotalRatings.BulkTotalRatingCreated.Success_Result


Scenario: BulkTotalRatingCreated message should throw exception when sales area is not found
	Given The data from file TotalRatings.Setup exists in database
	Given I have BulkTotalRatingCreated message from file TotalRatings.BulkTotalRatingCreated.Error_SalesAreaNotFound to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: SalesAreaNotFound


Scenario: BulkTotalRatingCreated message should throw exception when demographic is not found
	Given The data from file TotalRatings.Setup exists in database
	Given I have BulkTotalRatingCreated message from file TotalRatings.BulkTotalRatingCreated.Error_DemographicNotFound to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: DemographicNotFound


Scenario: BulkTotalRatingDeleted message should successfully delete records
	Given The data from file TotalRatings.Setup exists in database
	Given I have BulkTotalRatingDeleted message from file TotalRatings.BulkTotalRatingDeleted.Success_Request to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the TotalRatings data in GamePlan database is updated as the data from the following file TotalRatings.BulkTotalRatingDeleted.Success_Result

Scenario: BulkTotalRatingCreated invalid model
	Given I have BulkTotalRatingCreated message to publish
		| Data |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName |
		| Data         |

Scenario: BulkTotalRatingDeleted invalid model
	Given I have BulkTotalRatingDeleted message to publish
		| DateTimeFrom             | DateTimeTo               | SalesArea |
		|                          | 2019-11-10T12:28:42.478Z | 11        |
		| 2019-11-10T12:28:42.478Z |                          | 11        |
		| 2019-11-10T12:28:42.478Z | 2019-11-20T12:28:42.478Z |           |
		| 2019-09-25T09:52:23.203Z | 2019-09-23T09:52:23.203Z | 11        |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName                   |
		| Data[0].DateTimeFrom           |
		| Data[1].DateTimeTo             |
		| Data[2].SalesArea              |
		| Data[3].DateToLessThanDateFrom |

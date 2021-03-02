@Exchange @SqlServer
Feature: RatingsPredictionSchedule
	In order to synchronize data with xgGamePlan
	As a landmark user
	I want to send data to GamePlan

Background:
	Given GroupTransactionInfo for 1 event sent

Scenario: BulkRatingsPredictionSchedulesCreated message should throw validation exception for empty data property
	Given I have BulkRatingsPredictionScheduleCreated message to publish
		||
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName |
		| Data         |

Scenario: BulkRatingsPredictionSchedulesCreated message should throw validation exception for empty properties
	Given I have BulkRatingsPredictionScheduleCreated message to publish
		| SalesArea | ScheduleDay |
		|           |             |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName        |
		| Data[0].SalesArea   |
		| Data[0].ScheduleDay |

Scenario: BulkRatingsPredictionSchedulesCreated message should be successfully saved to the database
	Given I have BulkRatingsPredictionScheduleCreated message to publish
		| SalesArea | ScheduleDay              | Ratings |
		| SAT1      | 2019-09-19T09:52:23.192Z |         |
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And  the RatingsPredictionSchedules data in GamePlan database is updated as following
		| SalesArea | ScheduleDay              | Ratings |
		| SAT1      | 2019-09-19T09:52:23.192Z |         |

Scenario:BulkRatingsPredictionSchedulesDeleted message should throw validation exception for empty properties
	Given I have BulkRatingsPredictionSchedulesDeleted message to publish
		| DateTimeFrom             | DateTimeTo               | SalesArea |
		| 2019-09-19T09:52:23.203Z | 2019-09-21T09:52:23.203Z |           |
		|                          | 2019-09-23T09:52:23.203Z | SA1       |
		| 2019-09-22T09:52:23.203Z |                          | SA2       |
		| 2019-09-25T09:52:23.203Z | 2019-09-23T09:52:23.203Z | SA3       |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName                          |
		| Data[0].SalesArea                     |
		| Data[1].DateTimeFrom                  |
		| Data[2].DateTimeTo                    |
		| Data[2].DateFromLessOrEqualThanDateTo |
		| Data[3].DateFromLessOrEqualThanDateTo |

Scenario: Published BulkRatingsPredictionSchedulesDeleted message should be successfully sent to xgGamePlan and delete the existing records
	Given The data from file RatingsPredictionSchedule.BulkRatingsPredictionSchedulesDeleted.Setup exists in database
	And I have BulkRatingsPredictionSchedulesDeleted message to publish
		| DateTimeFrom                 | DateTimeTo                   | SalesArea |
		| 2019-05-07T00:00:00.0000000Z | 2019-05-09T00:00:00.0000000Z | GTV91     |
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the RatingsPredictionSchedules data in GamePlan database is updated as the data from the following file RatingsPredictionSchedule.BulkRatingsPredictionSchedulesDeleted.Result

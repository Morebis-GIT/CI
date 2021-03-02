@Exchange @SqlServer
Feature: Breaks
	In order to synchronize data with xgGamePlan
	As a landmark user
	I want to send data to GamePlan

Background:
	Given GroupTransactionInfo for 1 event sent

Scenario: BulkBreakCreated invalid model
	Given I have BulkBreakCreated message to publish
		| ScheduledDate            | SalesArea | BreakType | Duration | Optimize | ExternalBreakRef | Description | ExternalProgRef |
		|                          | test      | test      | 2:00:00  |          | test             |             |                 |
		| 2019-09-16T12:28:42.478Z |           | test2     | 3:00:00  |          | test2            |             |                 |
		| 2019-09-17T12:28:42.478Z | test3     |           | 4:00:00  |          | test3            |             |                 |
		| 2019-09-19T12:28:42.478Z | test4     | test4     | 00:00:00 |          |                  |             |                 |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName             |
		| Data[0].ScheduledDate    |
		| Data[1].SalesArea        |
		| Data[2].BreakType        |
		| Data[3].ExternalBreakRef |
		| Data[3].Duration         |
		

Scenario: BulkBreakCreated invalid BreakType
	Given The data from file Break.SalesAreas_Setup exists in database
	And The data from file Break.BulkBreakCreated_Setup exists in database
	And I have BulkBreakCreated message to publish
		| ScheduledDate                | SalesArea | BreakType | Duration | Optimize | ExternalBreakRef | Description | ExternalProgRef |
		| 2019-09-20T10:46:52.5760000Z | TST1      | Break_01  | 2:00:00  | true     | ref_1            | ref_1 desc  |                 |
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: InvalidBreakType

Scenario: BulkBreakCreated invalid SalesArea names
	Given The data from file Break.SalesAreas_Setup exists in database
	And The data from file Break.BulkBreakCreated_Setup exists in database
	And I have BulkBreakCreated message to publish
		| ScheduledDate                | SalesArea | BreakType | Duration | Optimize | ExternalBreakRef | Description | ExternalProgRef |
		| 2019-09-20T10:46:52.5760000Z | Sla_01    | BASE      | 2:00:00  | true     | ref_1            | ref_1 desc  |                 |
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: SalesAreaNotFound

@ignore
Scenario: BulkBreakCreated successfully creates single Breaks, updates existing Schedule
	Given The data from file Break.SalesAreas_Setup exists in database
	And The data from file Break.BulkBreakCreated_Setup exists in database
	And I have BulkBreakCreated message to publish
		| ScheduledDate                | SalesArea | BreakType | Duration | Optimize | ExternalBreakRef | Description | ExternalProgRef |
		| 2019-09-20T02:00:00.0000000Z | TST1      | BASE      | 3:00:00  | true     | ref_11           | ref_11 desc |                 |
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And  the Breaks data in GamePlan database is updated as the data from the following file Break.BulkBreakCreated_Setup_SingleBreak_Result

Scenario: BulkBreakCreated successfully creates Breaks, updates existing Schedules and creates new ones
	Given The data from file Break.SalesAreas_Setup exists in database
	And The data from file Break.BulkBreakCreated_Setup exists in database
	And I have BulkBreakCreated message to publish
		| ScheduledDate                | SalesArea | BreakType | Duration | Optimize | ExternalBreakRef | Description | ExternalProgRef |
		| 2019-09-20T10:46:52.5760000Z | TST1      | BASE      | 2:00:00  | true     | ref_11           | ref_11 desc |                 |
		| 2019-09-21T10:46:52.5760000Z | TST2      | BASE      | 3:00:00  | true     | ref_12           | ref_12 desc |                 |
		| 2019-09-22T10:46:52.5760000Z | TST3      | PREMIUM   | 3:00:00  | true     | ref_13           | ref_13 desc |                 |
		| 2019-09-22T10:46:52.5760000Z | TST3      | PREMIUM   | 3:00:00  | true     | ref_14           | ref_14 desc |                 |
		| 2019-09-27T10:46:52.5760000Z | TST2      | BASE      | 3:00:00  | true     | ref_15           | ref_15 desc |                 |
		| 2019-09-27T10:46:52.5760000Z | TST4      | BASE      | 3:00:00  | true     | ref_16           | ref_16 desc |                 |
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And  the Breaks data in GamePlan database is updated as the data from the following file Break.BulkBreakCreated_Setup_Result

Scenario: BulkBreaksDeleted invalid model with SalesAreaNames empty
	Given I have BulkBreaksDeleted message to publish
		| DateRangeStart          | DateRangeEnd            | SalesAreaNames |
		| 2019-09-03T09:58:44.138 | 2020-09-04T09:58:44.138 |                |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName           |
		| Data[0].SalesAreaNames |

Scenario: BulkBreaksDeleted invalid model with SalesAreaNames empty and invalid date range
	Given I have BulkBreaksDeleted message to publish
		| DateRangeStart           | DateRangeEnd             | SalesAreaNames |
		| 2020-09-07T09:58:44.138Z | 2020-09-04T09:58:44.138Z |                |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName                        |
		| Data[0].SalesAreaNames              |
		| Data[0].StartDateGreaterThanEndDate |

Scenario: BulkBreaksDeleted successfully deletes break and updates schedule
	Given The data from file Break.SalesAreas_Setup exists in database
	And The data from file Break.BulkBreakDeleted_Setup exists in database
	And I have BulkBreaksDeleted message to publish
		| DateRangeStart              | DateRangeEnd                | SalesAreaNames |
		| 2019-09-21T00:00:00.0000000 | 2019-09-21T00:00:00.0000000 | TST2           |
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And  the Breaks data in GamePlan database is updated as the data from the following file Break.BulkBreakDeleted_Setup_NoSpots_Result

Scenario: BulkBreaksDeleted successfully deletes break, updates schedule and removes Spots
	Given The data from file Break.SalesAreas_Setup exists in database
	And The data from file Break.BulkBreakDeleted_WithSpots_Setup exists in database
	And I have BulkBreaksDeleted message to publish
		| DateRangeStart              | DateRangeEnd                | SalesAreaNames |
		| 2019-09-21T00:00:00.0000000 | 2019-09-21T00:00:00.0000000 | TST2           |
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And  the Breaks data in GamePlan database is updated as the data from the following file Break.BulkBreakDeleted_WithSpots_Result

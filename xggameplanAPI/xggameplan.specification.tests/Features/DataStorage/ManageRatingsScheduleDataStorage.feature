@ManageDataStorage

Feature: Manage RatingsSchedule data storage
    In order to manage ratings schedule
	As a user
	I want to store ratings schedule via RatingsSchedule repository

Background:
    Given there is a RatingsSchedule repository	

Scenario: Add new RatingsSchedule
	Given predefined RatingsPredictionSchedules data
    When I create a document
    And I get all documents
    Then there should be 1 documents returned

Scenario: Add new RatingsSchedules
	Given predefined RatingsPredictionSchedules data
    When I create 3 documents
    And I get all documents
    Then there should be 3 documents returned

Scenario: Get all RatingsSchedule
	Given predefined RatingsPredictionSchedules data
    And 3 documents created
    When I get all documents
    Then there should be 3 documents returned

Scenario: Counting all RatingsSchedule
	Given predefined RatingsPredictionSchedules data
    And 3 documents created
    When I count the number of documents
    Then there should be 3 documents counted

Scenario: Truncating RatingsSchedule documents
    Given 5 documents created
    When I truncate documents
    And I get all documents
    Then no documents should be returned

Scenario: Get RatingsSchedule by date and sales area
	Given predefined Campaigns.SalesAreas.json data 
	And predefined data imported
	And the following documents created:
		| SalesArea | ScheduleDay |
		| QTQ91     | 2019-01-01  |
		| GTV93     | 2019-01-01  |
		| STW92     | 2019-02-15  |
		| STW99     | 2019-03-01  |
		| STW99     | 2019-04-01  |
		| QTQ91     | 2019-04-01  |
	When I call GetSchedule method with parameters:
        | Parameter    | Value      |
        | fromDateTime | 2019-01-01 |
        | salesarea    | QTQ91      |
	Then the received document should contain the following values:
		| Property    | Value      |
		| SalesArea   | QTQ91      |
		| ScheduleDay | 2019-01-01 |

Scenario: Delete RatingsSchedule
	Given predefined Campaigns.SalesAreas.json data 
	And predefined data imported
	And the following documents created:
		| SalesArea | ScheduleDay |
		| QTQ91     | 2019-01-01  |
		| GTV93     | 2019-01-01  |
		| STW92     | 2019-02-15  |
	When I call RemoveRatingsSchedule method with parameters:
        | Parameter   | Value      |
        | scheduleDay | 2019-01-01 |
        | salesarea   | QTQ91      |
	And I get all documents
	Then there should be 2 documents returned

Scenario Outline: Get RatingsSchedule by date range and sales area
	Given predefined Campaigns.SalesAreas.json data 
	And predefined data imported
	And the following documents created:
		| SalesArea | ScheduleDay |
		| QTQ91     | 2019-01-01  |
		| GTV93     | 2019-01-01  |
		| STW92     | 2019-02-15  |
		| STW92     | 2019-02-16  |
		| STW92     | 2019-02-17  |
		| STW92     | 2019-02-18  |
		| STW99     | 2019-03-01  |
		| STW99     | 2019-04-01  |
		| QTQ91     | 2019-04-01  |
	When I call GetSchedules method with parameters:
		| Parameter    | Value          |
		| fromDateTime | <FromDateTime> |
		| toDateTime   | <ToDateTime>   |
		| salesarea    | <SalesArea>    |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| FromDateTime | ToDateTime | SalesArea | ExpectedReturnCount |
	| 2019-02-15   | 2019-02-20 | STW92     | 4                   |
	| 2019-02-15   | 2019-02-20 | STW12     | 0                   |
	| 2019-02-15   | 2019-02-15 | STW92     | 1                   |
	| 2019-02-15   | 2019-02-15 | STXXX     | 0                   |
	| 2019-02-15   | 2019-02-15 | ''        | 0                   |
	| 0001-01-01   | 2019-02-15 | ''        | 0                   |
	| 2019-02-15   | 0001-01-01 | ''        | 0                   |
	| 0001-01-01   | 0001-01-01 | ''        | 0                   |
	| 2020-01-01   | 2021-01-01 | STW92     | 0                   |

Scenario Outline: Validate RatingsSchedule by date range, sales areas and demographics
	Given predefined Campaigns.SalesAreas.json data 
	And predefined RatingsPredictionSchedules data
	And predefined data imported
	When I call Validate_RatingsPredictionSchedules method with parameters:
		| Parameter                                   | Value                   |
		| fromDateTime                                | <FromDateTime>          |
		| toDateTime                                  | <ToDateTime>            |
		| salesAreaNames                              | <SalesAreaNames>        |
		| demographics                                | <DemographicsExtRefs>   |
		| noOfRatingPredictionsPerScheduleDayAreaDemo | <NoOfRatingPredictions> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| FromDateTime | ToDateTime | SalesAreaNames | DemographicsExtRefs | NoOfRatingPredictions | ExpectedReturnCount |
	| 2019-10-23   | 2019-10-23 | GTV93          | 2, 3, 4             | 8                     | 0                   |
	| 2019-10-23   | 2019-10-25 | TCN93          | 2, 3, 4             | 8                     | 1                   |
	| 2019-10-23   | 2019-10-24 | GTV93          | 2, 3                | 128                   | 3                   |
	| 2019-10-23   | 2019-10-24 | TCN91          | 4, 5                | 18                    | 1                   |
	| 2019-10-23   | 2019-10-24 | TCN91          | 2, 3                | 18                    | 1                   |
	| 2020-10-23   | 2021-10-24 | TCN93          | 2, 3                | 18                    | 1                   |

@ManageDataStorage

Feature: Manage TotalRatings data storage
	In order to manage TotalRatings
	As an Airtime manager
	I want to store TotalRatings in a data store

Background:
	Given there is a TotalRatings repository
	And predefined TotalRatings.SalesAreas.json data
	And predefined data imported

Scenario: Get TotalRatings by id
	Given the following documents created:
		| Id | SalesArea | Demograph | Date                | DaypartGroup | Daypart | TotalRatings |
		| 1  | 11        | DEMO1     | 2019-11-15 06:00:00 | 45555        | 122     | 3.25         |
		| 2  | 22        | DEMO2     | 2019-11-17 06:00:00 | 33334        | 0123    | 4.758        |
	When I get document with id 1
	Then there should be 1 documents returned

Scenario: Add new TotalRatings
	When I create the following documents:
		| Id | SalesArea | Demograph | Date                | DaypartGroup | Daypart | TotalRatings |
		| 1  | 11        | DEMO1     | 2019-11-15 06:00:00 | 45555        | 122     | 3.25         |
		| 2  | 22        | DEMO2     | 2019-11-17 06:00:00 | 33334        | 0123    | 4.758        |
	And I get all documents
	Then there should be 2 documents returned

Scenario: Get all TotalRatings
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Truncate TotalRatings
	Given 5 documents created
	When I truncate documents
	And I get all documents
	Then no documents should be returned

Scenario Outline: Search TotalRatings by date range and sales areas
	Given the following documents created:
		| Id | SalesArea | Demograph | Date                | DaypartGroup | Daypart | TotalRatings |
		| 1  | 11        | DEMO1     | 2019-11-15 06:00:00 | 45555        | 122     | 3.25         |
		| 2  | 22        | DEMO2     | 2019-11-17 06:00:00 | 33334        | 0123    | 4.758        |
		| 3  | 11        | DEMO3     | 2019-11-14 06:00:00 | 45555        | 122     | 3.75         |
		| 4  | 33        | DEMO2 4   | 2019-11-07 06:00:00 | 33334        | 0123    | 10.536       |
	When I call Search method with parameters:
		| Parameter | Value       |
		| salesArea | <SalesArea> |
		| startDate | <StartDate> |
		| endDate   | <EndDate>   |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| SalesArea | StartDate  | EndDate    | ExpectedReturnCount |
	| 11        | 2019-11-12 | 2019-11-16 | 2                   |
	| 22        | 2019-11-12 | 2019-11-19 | 1                   |
	| 44        | 2019-11-12 | 2019-11-16 | 0                   |
	| 11        | 2019-10-22 | 2019-10-12 | 0                   |

Scenario Outline: Delete TotalRatings by ids
	Given the following documents created:
		| Id | SalesArea | Demograph | Date                | DaypartGroup | Daypart | TotalRatings |
		| 1  | 11        | DEMO1     | 2019-11-15 06:00:00 | 45555        | 122     | 3.25         |
		| 2  | 22        | DEMO2     | 2019-11-17 06:00:00 | 33334        | 0123    | 4.758        |
		| 3  | 11        | DEMO3     | 2019-11-14 06:00:00 | 45555        | 122     | 3.75         |
		| 4  | 33        | DEMO2 4   | 2019-11-07 06:00:00 | 33334        | 0123    | 10.536       |
	When I call DeleteRange method with parameters:
		| Parameter | Value |
		| ids       | <Ids> |
	And I get all documents
	Then there should be <ExpectedReturnCount> documents returned
	
	Examples:
	| Ids | ExpectedReturnCount |
	| 1   | 3                   |
	| 1,2 | 2                   |

Scenario Outline: Search TotalRatings by months
	Given the following documents created:
		| Id | SalesArea | Demograph | Date                | DaypartGroup | Daypart | TotalRatings |
		| 1  | 11        | DEMO1     | 2019-12-01 06:00:00 | 45555        | 122     | 3.25         |
		| 2  | 22        | DEMO2     | 2019-11-01 06:00:00 | 33334        | 0123    | 4.758        |
		| 3  | 11        | DEMO3     | 2019-10-01 06:00:00 | 45555        | 122     | 3.75         |
		| 4  | 33        | DEMO2 4   | 2019-09-01 06:00:00 | 33334        | 0123    | 10.536       |
	When I call SearchByMonths method with parameters:
		| Parameter | Value       |
		| startDate | <StartDate> |
		| endDate   | <EndDate>   |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| StartDate  | EndDate    | ExpectedReturnCount |
	| 2019-11-29 | 2019-12-02 | 2                   |
	| 2019-10-12 | 2019-10-13 | 1                   |
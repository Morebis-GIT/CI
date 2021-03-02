@ManageDataStorage

Feature: Manage StandardDayPart data storage
	In order to manage StandardDayPart
	As an Airtime manager
	I want to store StandardDayPart in a data store

Background:
	Given there is a StandardDayParts repository
	And predefined StandardDayParts.SalesAreas.json data
	And predefined data imported

Scenario: Add new standardDayPart
	When I create the following documents:
		| Id | DayPartId | SalesArea | Name | Order |
		| 1  | 1         | SA1       | DP1  | 1     |
		| 2  | 2         | SA2       | DP2  | 2     |
		| 3  | 3         | SA3       | DP3  | 3     |
	And I get all documents
	Then there should be 3 documents returned

Scenario: Get all standardDayPart
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Get standardDayPart by id
	Given the following documents created:
		| Id | DayPartId | SalesArea | Name | Order |
		| 1  | 1         | SA1       | DP1  | 1     |
		| 2  | 2         | SA2       | DP2  | 2     |
		| 3  | 3         | SA3       | DP3  | 3     |     
	When I get document with id 2
	Then there should be 1 documents returned

Scenario: Truncate standardDayPart
	Given 5 documents created
	When I truncate documents
	And I get all documents
	Then no documents should be returned

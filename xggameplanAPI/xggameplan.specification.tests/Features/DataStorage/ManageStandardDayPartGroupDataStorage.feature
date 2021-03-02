@ManageDataStorage

Feature: Manage StandardDayPartGroups data storage
	In order to manage StandardDayParts
	As an Airtime manager
	I want to store StandardDayParts in a data store

Background:
	Given there is a StandardDayPartGroups repository

Scenario: Add new standardDayPartGroups
	When I create the following documents:
		| Id | GroupId | SalesArea | Demographic | Optimizer | Policy | RatingReplacement |
		| 1  | 1       | SA1       | D1          | 1         | 1      | 0                 |
		| 2  | 2       | SA2       | D2          | 0         | 0      | 1                 |
		| 3  | 3       | SA3       | D3          | 1         | 1      | 0                 |
	And I get all documents
	Then there should be 3 documents returned

Scenario: Get all standardDayPartGroups
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Get standardDayPartGroup by id
	Given the following documents created:
		| Id | GroupId | SalesArea | Demographic | Optimizer | Policy | RatingReplacement |
		| 1  | 1       | SA1       | D1          | 1         | 1      | 0                 |
		| 2  | 2       | SA2       | D2          | 0         | 0      | 1                 |
		| 3  | 3       | SA3       | D3          | 1         | 1      | 0                 |    
	When I get document with id 2
	Then there should be 1 documents returned

Scenario: Truncate standardDayPartGroups
	Given 5 documents created
	When I truncate documents
	And I get all documents
	Then no documents should be returned

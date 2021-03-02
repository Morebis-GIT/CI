@ManageDataStorage

Feature: Manage InventoryLocks data storage
	In order to manage InventoryLocks
	As an Airtime manager
	I want to store InventoryLocks in a data store

Background:
	Given there is a InventoryLocks repository
	And predefined SalesAreas.json data

Scenario: Get InventoryLocks by id
	Given the following documents created:
		| Id | SalesArea | InventoryCode | StartDate           | EndDate             | StartTime | EndTime  |
		| 1  | NWS91     | CA            | 2019-11-15 06:00:00 | 2019-11-16 06:00:00 | 03:00:00  | 03:00:00 |
		| 2  | QTQ93     | CA            | 2019-11-15 06:00:00 | 2019-11-16 06:00:00 | 02:00:00  | 02:00:00 |
	When I get document with id 1
	Then there should be 1 documents returned

Scenario: Add new InventoryLocks
	When I create the following documents:
		| Id | SalesArea | InventoryCode | StartDate           | EndDate             | StartTime | EndTime  |
		| 1  | NWS91     | CA            | 2019-11-15 06:00:00 | 2019-11-16 06:00:00 | 03:00:00  | 03:00:00 |
		| 2  | QTQ93     | CA            | 2019-11-15 06:00:00 | 2019-11-16 06:00:00 | 02:00:00  | 02:00:00 |
	And I get all documents
	Then there should be 2 documents returned

Scenario: Get all InventoryLocks
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Truncate InventoryLocks
	Given 5 documents created
	When I truncate documents
	And I get all documents
	Then no documents should be returned

Scenario Outline: Remove InventoryLocks by salesAreas
	Given predefined SalesAreas.json data imported
	And the following documents created:
		| Id | SalesArea | InventoryCode | StartDate           | EndDate             | StartTime | EndTime  |
		| 1  | NWS91     | CA1           | 2019-11-15 06:00:00 | 2019-11-16 06:00:00 | 03:00:00  | 03:00:00 |
		| 2  | QTQ93     | CA2           | 2019-11-15 06:00:00 | 2019-11-16 06:00:00 | 02:00:00  | 02:00:00 |
		| 3  | QTQ91     | CA3           | 2019-11-15 06:00:00 | 2019-11-16 06:00:00 | 02:00:00  | 02:00:00 |
		| 4  | STW92     | CA4           | 2019-11-15 06:00:00 | 2019-11-16 06:00:00 | 02:00:00  | 02:00:00 |
	When I call DeleteRange method with parameters:
		| Parameter  | Value        |
		| salesAreas | <SalesAreas> |
	And I get all documents
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| SalesAreas          | ExpectedReturnCount |
	| NWS91               | 3                   |
	| NWS91, QTQ91        | 2                   |
	| NWS91, QTQ91, STW92 | 1                   |

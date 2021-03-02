@ManageDataStorage

Feature: ManageDemographicDataStorage
	In order to manage demographics
	As a user
	I want to store demographics via Demographics repository

Background:
	Given there is a Demographics repository
	And predefined Demographics data

Scenario: Add one new demographic
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Add new demographics
	Given the following documents created:
		| Id | ExternalRef | Name     | ShortName | DisplayOrder | Gameplan |
		| 1  | 815         | GS 32-16 | S32-16    | 13           | True     |
		| 2  | 816         | GS 41-19 | S41-19    | 15           | False    |
		| 3  | 817         | GS 41-20 | S41-20    | 16           | True     |
	When I get all documents
	Then there should be 3 documents returned

Scenario: Get all demographics
	Given 5 documents created
	When I get all documents
	Then there should be 5 documents returned

Scenario: Get demographic by id
	Given predefined data imported
	When I get document with id 2
	Then there should be 1 documents returned

Scenario: Get a non-existing demographic by id
	Given the following documents created:
		| Id | ExternalRef | Name     | ShortName | DisplayOrder | Gameplan |
		| 1  | 815         | GS 32-16 | S32-16    | 13           | True     |
		| 2  | 816         | GS 41-19 | S41-19    | 15           | False    |
		| 3  | 817         | GS 41-20 | S41-20    | 16           | True     |
	When I get document with id '99'
	Then no documents should be returned

Scenario: Delete demographics by id
	Given predefined data imported
	When I delete document with id 2
	And I get all documents
	Then there should be 3 documents returned

Scenario: Truncate demographics
	Given 5 documents created
	When I truncate documents
	And I get all documents
	Then no documents should be returned

Scenario: Counting all Demographics
	Given 3 documents created
	When I count the number of documents
	Then there should be 3 documents counted

Scenario: Update demographic
	Given the following documents created:
		| Id | ExternalRef | Name     | ShortName | DisplayOrder | Gameplan |
		| 1  | 815         | GS 32-16 | S32-16    | 13           | True     |
		| 2  | 816         | GS 41-19 | S41-19    | 15           | False    |
		| 3  | 817         | GS 41-20 | S41-20    | 16           | True     |
	When I get document with id 2
	Then there should be 1 documents returned
	When I update received document by values:
		| Property     | Value |
		| ExternalRef  | 1020  |
		| DisplayOrder | 300   |
	And I get document with id 2
	Then the received document should contain the following values:
		| Property     | Value    |
		| Name         | GS 41-19 |
		| ShortName    | S41-19   |
		| ExternalRef  | 1020     |
		| DisplayOrder | 300      |
		| Gameplan     | False    |

Scenario: Get demographics by external reference parameter
	Given predefined data imported
	When I call GetByExternalRef method with parameters:
		| Parameter   | Value |
		| externalRef | 15    |
	Then there should be 0 documents returned
	When I call GetByExternalRef method with parameters:
		| Parameter   | Value |
		| externalRef | 657   |
	Then there should be 1 documents returned

Scenario: Get all gameplan demographics
	Given the following documents created:
		| Id | ExternalRef | Name     | ShortName | DisplayOrder | Gameplan |
		| 1  | 815         | GS 32-16 | S32-16    | 13           | True     |
		| 2  | 816         | GS 41-19 | S41-19    | 15           | False    |
		| 3  | 817         | GS 41-20 | S41-20    | 16           | True     |
	When I call GetAllGameplanDemographics method
	Then there should be 2 documents returned

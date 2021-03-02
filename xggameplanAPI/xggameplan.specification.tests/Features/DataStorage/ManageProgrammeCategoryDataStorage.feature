@ManageDataStorage

Feature: Manage ProgrammeCategories data storage
	In order to manage ProgrammeCategories
	As an Airtime manager
	I want to store ProgrammeCategories in a data store

Background:
	Given there is a ProgrammeCategories repository

Scenario: Get ProgrammeCategories by id
	Given the following documents created:
		| Id | Name     | ExternalRef | ParentExternalRef |
		| 1  | ALL      |             |                   |
		| 2  | CHILDREN | CHD         | PARENT            |   
	When I get document with id 1
	Then there should be 1 documents returned

Scenario: Add new ProgrammeCategories
	When I create the following documents:
		| Id | Name     | ExternalRef | ParentExternalRef |
		| 1  | ALL      |             |                   |
		| 2  | CHILDREN | CHD         | PARENT            |
	And I get all documents
	Then there should be 2 documents returned

Scenario: Get all ProgrammeCategories
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Truncate ProgrammeCategories
	Given 5 documents created
	When I truncate documents
	And I get all documents
	Then no documents should be returned

Scenario Outline: Search ProgrammeCategories by names
	Given the following documents created:
		| Id | Name     | ExternalRef | ParentExternalRef |
		| 1  | ALL      |             |                   |
		| 2  | CHILDREN | CHD         | PARENT            |   
	When I call Search method with parameters:
		| Parameter              | Value                    |
		| programmeCategoryNames | <ProgrammeCategoryNames> |
	Then there should be <ExpectedReturnCount> documents returned
	Examples:
	| ProgrammeCategoryNames | ExpectedReturnCount |
	| ALL                    | 1                   |

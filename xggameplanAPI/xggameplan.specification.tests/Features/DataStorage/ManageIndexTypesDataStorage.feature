@ManageDataStorage

Feature: ManageIndexTypesDataStorage
	In order to manage IndexTypes
	As an Airtime manager
	I want to store IndexTypes via IndexTypes repository

Background:
	Given there is a IndexTypes repository
	And predefined SalesAreas.json data

Scenario: Add new index type
	Given 1 documents created
	When I get all documents
	Then there should be 1 documents returned

Scenario: Add new index types
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Get an existing index type by id
	Given predefined SalesAreas.json data imported
	And the following documents created:
		| Id | Description | SalesArea |
		| 1  | Initial     | NWS91     |
		| 2  | Word        | QTQ91     |
		| 3  | Word        | STW92     |
	When I get document with id 1
	Then there should be 1 documents returned

Scenario: Get a non-existing index type by id
	Given predefined SalesAreas.json data imported
	And the following documents created:
		| Id | Description | SalesArea |
		| 1  | Initial     | NWS91     |
		| 2  | Word        | QTQ91     |
		| 3  | Word        | STW92     |
	When I get document with id 0
	Then no documents should be returned

Scenario: Update index type
	Given predefined SalesAreas.json data imported
	And the following documents created:
		| Id | Description | SalesArea |
		| 1  | Initial     | NWS91     |
		| 2  | Word        | QTQ91     |
		| 3  | Word        | STW92     |
	When I get document with id 1
	Then there should be 1 documents returned
	When I update received document by values:
        | Parameter   | Value   |
        | Description | Updated |
        | SalesArea   | GTV93   |
	And I get document with id 1
	Then the received document should contain the following values:
        | Parameter   | Value   |
        | Description | Updated |
        | SalesArea   | GTV93   |

Scenario: Delete an existing index type by id
	And the following documents created:
		| Id | Description | SalesArea |
		| 1  | Initial     | NWS91     |
		| 2  | Word        | QTQ91     |
	When I delete document with id 1
	And I get all documents
	Then the received document should contain the following values:
        | Parameter | Value |
        | Id        | 2     |
	When I get document with id 1
	Then no documents should be returned

Scenario: Delete a non-existing index type by id
	Given the following documents created:
		| Id | Description | SalesArea |
		| 1  | Initial     | NWS91     |
		| 2  | Word        | QTQ91     |
	When I delete document with id 0
	And I get all documents
	Then there should be 2 documents returned

Scenario: Count all index types
	Given 3 documents created
	When I count the number of documents
	Then there should be 3 documents counted

Scenario: Truncate index types
	Given 3 documents created
	When I truncate documents
	And I get all documents
	Then no documents should be returned

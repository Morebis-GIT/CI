@ManageDataStorage

Feature: Manage InventoryTypes data storage
	In order to manage InventoryTypes
	As an Airtime manager
	I want to store InventoryTypes in a data store

Background:
	Given there is a InventoryTypes repository

Scenario: Get InventoryType by id
	Given the following documents created:
		| Id | InventoryCode | Description | System |
		| 1  | CA            | Main        | S      |
		| 2  | CB            | Second      | S      |
		| 3  | CC            | Third       | S      |      
	When I get document with id 1
	Then there should be 1 documents returned

Scenario: Add new InventoryTypes
	When I create the following documents:
		| Id | InventoryCode | Description | System |
		| 1  | CA            | Main        | S      |
		| 2  | CB            | Second      | S      |
		| 3  | CC            | Third       | S      |
	And I get all documents
	Then there should be 3 documents returned

Scenario: Get all InventoryTypes
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Truncate InventoryTypes
	Given 5 documents created
	When I truncate documents
	And I get all documents
	Then no documents should be returned

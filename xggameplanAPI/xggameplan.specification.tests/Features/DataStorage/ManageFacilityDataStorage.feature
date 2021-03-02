@ManageDataStorage

Feature: Manage Facilities data storage
	In order to manage Facilities
	As an Airtime manager
	I want to store Facilities in a data store
	
Background: 
	Given there is a Facilities repository
	
Scenario: Get all Facilities
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned
	
Scenario: Get an existing Facility by id
	Given the following documents created:
		| Id | Code   |
		| 1  | ABAREA |
		| 2  | ABBONS |
		| 3  | ABBTRQ |
	When I get document with id 1
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property  | Value |
		| Id        | 1     |
	
Scenario: Get a nonexistent Facility by id
	Given the following documents created:
		| Id | Code   |
		| 1  | ABAREA |
		| 2  | ABBONS |
		| 3  | ABBTRQ |
	When I get document with id 5
	Then no documents should be returned
	
Scenario: Add new Facility
	When I create a document
	And I get all documents
	Then there should be 1 documents returned
	
Scenario: Update exiting Facility
	Given the following documents created:
		| Id | Code   |
		| 1  | ABAREA |
		| 2  | ABBONS |
		| 3  | ABBTRQ |
	When I get document with id 1
	And I update received document by values:
		| Property   | Value  |
		| Code       | DKLHET |
	And I get document with id 1
	Then the received document should contain the following values:
		| Property   | Value  |
		| Code       | DKLHET |
	
Scenario: Delete Facility by id
	Given the following documents created:
		| Id | Code   |
		| 1  | ABAREA |
		| 2  | ABBONS |
		| 3  | ABBTRQ |
	When I delete document with id 1
	And I get all documents
	Then there should be 2 documents returned

Scenario: Delete a nonexistent Facility by id
	Given the following documents created:
		| Id | Code   |
		| 1  | ABAREA |
		| 2  | ABBONS |
		| 3  | ABBTRQ |
	When I delete document with id 5
	And I get all documents
	Then there should be 3 documents returned

@ManageDataStorage

Feature: Manage FlexibilityLevel data storage
	In order to manage FlexibilityLevels
	As an Airtime manager
	I want to store FlexibilityLevels in a data store
	
Background: 
	Given there is a FlexibilityLevels repository
	
Scenario: Get all FlexibilityLevels
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned
	
Scenario: Get an existing FlexibilityLevel by id
	Given the following documents created:
		| Id | Name     |
		| 1  | Low      |
		| 2  | Medium   |
		| 3  | High     |
	When I get document with id 1
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property  | Value |
		| Id        | 1     |
	
Scenario: Get a nonexistent FlexibilityLevel by id
	Given the following documents created:
		| Id | Name     |
		| 1  | Low      |
		| 2  | Medium   |
		| 3  | High     |
	When I get document with id 4
	Then no documents should be returned
	
Scenario: Add new FlexibilityLevel
	When I create a document
	And I get all documents
	Then there should be 1 documents returned
	
Scenario: Update exiting FlexibilityLevel
	Given the following documents created:
		| Id | Name     |
		| 1  | Low      |
		| 2  | Medium   |
		| 3  | High     |
	When I get document with id 1
	And I update received document by values:
		| Property  | Value     |
		| Name      | Extreme   |
	And I get document with id 1
	Then the received document should contain the following values:
		| Property  | Value     |
		| Name      | Extreme   |
	
Scenario: Delete FlexibilityLevel by id
	Given the following documents created:
		| Id | Name     |
		| 1  | Low      |
		| 2  | Medium   |
		| 3  | High     |
	When I delete document with id 1
	And I get all documents
	Then there should be 2 documents returned

Scenario: Delete a nonexistent FlexibilityLevel by id
	Given the following documents created:
		| Id | Name     |
		| 1  | Low      |
		| 2  | Medium   |
		| 3  | High     |
	When I delete document with id 4
	And I get all documents
	Then there should be 3 documents returned

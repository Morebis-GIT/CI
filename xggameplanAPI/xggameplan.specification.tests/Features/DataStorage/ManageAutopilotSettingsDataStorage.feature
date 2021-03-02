@ManageDataStorage

Feature: Manage AutopilotSettings data storage
	In order to manage AutopilotSettings
	As an Airtime manager
	I want to store AutopilotSettings in a data store
	
Background: 
	Given there is a AutopilotSettings repository
	
Scenario: Get all AutopilotSettings
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Get default AutopilotSettings
	Given 1 documents created
	When I call GetDefault method
	Then there should be 1 documents returned
	
Scenario: Get an existing AutopilotSettings by id
	Given the following documents created:
		| Id | DefaultFlexibilityLevelId |
		| 1  | 1                         |
		| 2  | 2                         |
		| 3  | 3                         |
	When I get document with id 1
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property  | Value |
		| Id        | 1     |
	
Scenario: Get a nonexistent AutopilotSettings by id
	Given the following documents created:
		| Id | DefaultFlexibilityLevelId |
		| 1  | 1                         |
		| 2  | 2                         |
		| 3  | 3                         |
	When I get document with id 4
	Then no documents should be returned
	
Scenario: Add new AutopilotSettings
	When I create a document
	And I get all documents
	Then there should be 1 documents returned
	
Scenario: Update exiting AutopilotSettings
	Given the following documents created:
		| Id | DefaultFlexibilityLevelId |
		| 1  | 1                         |
		| 2  | 2                         |
		| 3  | 3                         |
	When I get document with id 1
	And I update received document by values:
		| Property                  | Value |
		| DefaultFlexibilityLevelId | 3     |
	And I get document with id 1
	Then the received document should contain the following values:
		| Property                  | Value |
		| DefaultFlexibilityLevelId | 3     |
	
Scenario: Delete AutopilotSettings by id
	Given the following documents created:
		| Id | DefaultFlexibilityLevelId |
		| 1  | 1                         |
		| 2  | 2                         |
		| 3  | 3                         |
	When I delete document with id 1
	And I get all documents
	Then there should be 2 documents returned

Scenario: Delete a nonexistent AutopilotSettings by id
	Given the following documents created:
		| Id | DefaultFlexibilityLevelId |
		| 1  | 1                         |
		| 2  | 2                         |
		| 3  | 3                         |
	When I delete document with id 4
	And I get all documents
	Then there should be 3 documents returned

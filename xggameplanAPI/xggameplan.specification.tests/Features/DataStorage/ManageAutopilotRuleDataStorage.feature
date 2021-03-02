@ManageDataStorage

Feature: Manage AutopilotRule data storage
	In order to manage AutopilotRules
	As an Airtime manager
	I want to store AutopilotRules in a data store
	
Background: 
	Given there is a AutopilotRules repository
	
Scenario: Get all AutopilotRules
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned
	
Scenario: Get an existing AutopilotRule by id
	Given the following documents created:
		| Id | RuleId | RuleTypeId | FlexibilityLevelId  | Enabled |
		| 1  | 4      | 7          | 1                   | true    |
		| 2  | 5      | 8          | 2                   | true    |
		| 3  | 6      | 9          | 3                   | true    |
	When I get document with id 1
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property  | Value |
		| Id        | 1     |
	
Scenario: Get a nonexistent AutopilotRule by id
	Given the following documents created:
		| Id | RuleId | RuleTypeId | FlexibilityLevelId  | Enabled |
		| 1  | 4      | 7          | 1                   | true    |
		| 2  | 5      | 8          | 2                   | true    |
		| 3  | 6      | 9          | 3                   | true    |
	When I get document with id 4
	Then no documents should be returned
	
Scenario: Add new AutopilotRule
	When I create a document
	And I get all documents
	Then there should be 1 documents returned
	
Scenario: Update exiting AutopilotRule
	Given the following documents created:
		| Id | RuleId | RuleTypeId | FlexibilityLevelId  | Enabled |
		| 1  | 4      | 7          | 1                   | true    |
		| 2  | 5      | 8          | 2                   | true    |
		| 3  | 6      | 9          | 3                   | true    |
	When I get document with id 1
	And I update received document by values:
		| Property              | Value |
		| FlexibilityLevelId    | 1     |
		| Enabled               | false |
	And I get document with id 1
	Then the received document should contain the following values:
		| Property              | Value |
		| FlexibilityLevelId    | 1     |
		| Enabled               | false |
	
Scenario: Delete AutopilotRule by id
	Given the following documents created:
		| Id | RuleId | RuleTypeId | FlexibilityLevelId  | Enabled |
		| 1  | 4      | 7          | 1                   | true    |
		| 2  | 5      | 8          | 2                   | true    |
		| 3  | 6      | 9          | 3                   | true    |
	When I delete document with id 1
	And I get all documents
	Then there should be 2 documents returned

Scenario: Delete a nonexistent AutopilotRule by id
	Given the following documents created:
		| Id | RuleId | RuleTypeId | FlexibilityLevelId  | Enabled |
		| 1  | 4      | 7          | 1                   | true    |
		| 2  | 5      | 8          | 2                   | true    |
		| 3  | 6      | 9          | 3                   | true    |
	When I delete document with id 4
	And I get all documents
	Then there should be 3 documents returned
	
Scenario Outline: Get AutopilotRules by flexibilityLevelId
	Given the following documents created:
		| Id | RuleId | RuleTypeId | FlexibilityLevelId  | Enabled |
		| 1  | 4      | 7          | 1                   | true    |
		| 2  | 5      | 8          | 2                   | true    |
		| 3  | 6      | 9          | 3                   | true    |
	When I call GetByFlexibilityLevelId method with parameters:
		| Parameter     | Value |
		| id            | <Id>  |
	Then there should be <ExpectedReturnCount> documents returned
	
	Examples:
	| Id    | ExpectedReturnCount |
	| 1     | 1                   |
	| 3     | 1                   |
	| 5     | 0                   |
	
Scenario Outline: Delete AutopilotRule by ids
	Given the following documents created:
		| Id | RuleId | RuleTypeId | FlexibilityLevelId  | Enabled |
		| 1  | 4      | 7          | 1                   | true    |
		| 2  | 5      | 8          | 2                   | true    |
		| 3  | 6      | 9          | 3                   | true    |
	When I call Delete method with parameters:
		| Parameter     | Value |
		| ids           | <Ids> |
	And I get all documents
	Then there should be <ExpectedReturnCount> documents returned
	
	Examples:
	| Ids       | ExpectedReturnCount |
	| 1         | 2                   |
	| 1, 2      | 1                   |
	| 1, 2, 3   | 0                   |
	| 1, 2, 5   | 1                   |
	| 5, 7, 1   | 2                   |
	| 5, 7, 9   | 3                   |

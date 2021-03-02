@ManageDataStorage

Feature: ManageRuleTypeDataStorage
	In order to manage rule types
	As a user
	I want to store rule types via RuleTypes repository

Background:
	Given there is a RuleTypes repository
	And predefined RuleTypes data

Scenario: Add one new rule
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Add new ruletypes
	Given the following documents created:
		| Id  | Name   | IsCustom | AllowedForAutopilot |
		| 1   | RT 1-1 | True     | True                |
		| 2   | RT 2-1 | False    | False               |
		| 3   | RT 3-1 | True     | True                |
	When I get all documents
	Then there should be 3 documents returned

Scenario: Get all RuleTypes
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Get RuleTypes by id
	Given predefined data imported
	When I get document with id 2
	Then there should be 1 documents returned

Scenario: Get a non-existing RuleTypes by id
	Given the following documents created:
		| Id  | Name   | IsCustom | AllowedAutopilot |
		| 1   | RT 1-1 | True     | True             |
		| 2   | RT 2-1 | False    | False            |
		| 3   | RT 3-1 | True     | True             |
	When I get document with id '99'
	Then no documents should be returned

Scenario: Delete RuleTypes by id
	Given predefined data imported
	When I delete document with id 2
	And I get all documents
	Then there should be 2 documents returned

Scenario: Update RuleTypes
	Given the following documents created:
		| Id  | Name   | IsCustom | AllowedAutopilot |
		| 1   | RT 1-1 | True     | True             |
		| 2   | RT 2-1 | False    | False            |
		| 3   | RT 3-1 | True     | True             |
	When I get document with id 2
	Then there should be 1 documents returned
	When I update received document by values:
		| Property      | Value  |
		| Name          | RT 2-2 |
		| IsCustom      | True   |
	And I get document with id 2
	Then the received document should contain the following values:
		| Property            | Value  |
		| Name                | RT 2-2 |
		| IsCustom            | True   |
		| AllowedForAutopilot | False  |

@ManageDataStorage

Feature: ManageRuleDataStorage
	In order to manage rule
	As a user
	I want to store rules via Rules repository

Background:
	Given there is a Rules repository
	And predefined Rules data

Scenario: Add one new Rule
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Add new Rules
	Given the following documents created:
		| Id    | RuleTypeId  | InternalType    | Description | Type  |
		| 1     | 1           | internal        | R 1-1       | rules |
		| 2     | 1           | internal        | R 2-1       | rules |
		| 3     | 1           | internal        | R 3-1       | rules |
	When I get all documents
	Then there should be 3 documents returned

Scenario: Get all Rules
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Get Rules by id
	Given predefined data imported
	When I get document with id 2
	Then there should be 1 documents returned

Scenario: Get a non-existing Rules by id
	Given the following documents created:
		| Id    | RuleTypeId  | InternalType    | Description | Type  |
		| 1     | 1           | internal        | R 1-1       | rules |
		| 2     | 1           | internal        | R 2-1       | rules |
		| 3     | 1           | internal        | R 3-1       | rules |
	When I get document with id 99
	Then no documents should be returned

Scenario: Delete Rules by id
	Given the following documents created:
		| Id    | RuleTypeId  | InternalType    | Description | Type  |
		| 1     | 1           | internal        | R 1-1       | rules |
		| 2     | 1           | internal        | R 2-1       | rules |
		| 3     | 1           | internal        | R 3-1       | rules |
	When I delete document with id 2
	And I get all documents
	Then there should be 2 documents returned

Scenario: Update Rules
	Given the following documents created:
		| Id    | RuleTypeId  | InternalType    | Description | Type  |
		| 1     | 1           | internal        | R 1-1       | rules |
		| 2     | 1           | internal        | R 2-1       | rules |
		| 3     | 1           | internal        | R 3-1       | rules |
	When I get document with id 2
	Then there should be 1 documents returned
	When I update received document by values:
		| Property      | Value     |
		| Description   | RT 2-2    |
		| CampaignType  | Spot      |
	When I get document with id 2
	Then the received document should contain the following values:
		| Property      | Value     |
		| Description   | RT 2-2    |
		| CampaignType  | Spot      |
		| RuleTypeId    | 1         |
		| Type          | rules     |

Scenario Outline: Find Rule by ruleTypeId
	Given the following documents created:
		| Id    | RuleTypeId  | InternalType    | Description | Type  |
		| 1     | 1           | internal        | R 1-1       | rules |
		| 2     | 2           | internal        | R 2-1       | rules |
		| 3     | 3           | internal        | R 3-1       | rules |
	When I call FindByRuleTypeId method with parameters:
		| Parameter     | Value |
		| ruleTypeId    | <Id>  |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| Id        | ExpectedReturnCount |
	| 1         | 1                   |
	| 5         | 0                   |

Scenario Outline: Find Rules by ruleTypeIds
	Given the following documents created:
		| Id    | RuleTypeId  | InternalType    | Description  | Type  |
		| 1     | 1           | internal        | R 1-1        | rules |
		| 2     | 2           | internal        | R 2-1        | rules |
		| 3     | 3           | internal        | R 3-1        | rules |
	When I call FindByRuleTypeIds method with parameters:
		| Parameter     | Value |
		| ruleTypeIds   | <Ids> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| Ids       | ExpectedReturnCount |
	| 1         | 1                   |
	| 1, 2      | 2                   |
	| 1, 2, 3   | 3                   |
	| 1, 2, 5   | 2                   |
	| 5, 9, 7   | 0                   |

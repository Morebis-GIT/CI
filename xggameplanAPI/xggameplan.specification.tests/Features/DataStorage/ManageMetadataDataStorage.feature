@ManageDataStorage

Feature: ManageMetadataDataStorage
	In order to manage metadata
	As a user
	I want to store metadata via Metadata repository

Background: 
	Given there is a Metadata repository
	And predefined Metadata data

Scenario: Add new Metadata
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Get all Metadata
	Given predefined data imported
	When I get all documents
	Then there should be 1 documents returned

Scenario Outline: Get Metadata by key
	Given predefined data imported
	When I call GetByKey method with parameters:
		| Parameter | Value |
		| key       | <Key> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| Key               | ExpectedReturnCount |
	| BreakTypes        | 2                   |

Scenario Outline: Get Metadata by keys
	Given predefined data imported
	When I call GetByKeys method with parameters:
		| Parameter | Value |
		| keys       | <Keys> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| Keys                          | ExpectedReturnCount |
	| BreakTypes                    | 2                   |

Scenario Outline: Delete Metadata by key
	Given predefined data imported
	When I call DeleteByKey method with parameters:
		| Parameter | Value |
		| key       | <Key> |
	And I call GetByKey method with parameters:
		| Parameter | Value |
		| key       | <Key> |
	Then there should be <ExpectedReturnCount> documents returned
	
	Examples:
	| Key               | ExpectedReturnCount |
	| BreakTypes        | 0                   |

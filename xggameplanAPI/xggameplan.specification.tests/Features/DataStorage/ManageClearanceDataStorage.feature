@ManageDataStorage

Feature: ManageClearanceDataStorage
	In order to manage clearance codes
	As a user
	I want to store clearance codes via Clearance repository

Background:
	Given there is a Clearance repository

Scenario: Add new clearance code
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Add new clearance codes
	When I create the following documents:
		| Id | Code | Description |
		| 1  | G    | Code G      |
		| 2  | S    | Code S      |
		| 3  | W    | Code W      |
	And I get all documents
	Then there should be 3 documents returned

Scenario: Get all clearance codes
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Get clearance code by id
	Given the following documents created:
		| Id | Code | Description |
		| 1  | G    | Code G      |
		| 2  | S    | Code S      |
		| 3  | W    | Code W      |
	When I get document with id 2
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property | Value |
		| Id       | 2     |

Scenario: Get a non-existing clearance codes by id
	Given the following documents created:
		| Id | Code | Description |
		| 1  | G    | Code G      |
		| 2  | S    | Code S      |
		| 3  | W    | Code W      |
	When I get document with id '99'
	Then no documents should be returned

Scenario: Delete clearance code by id
	Given the following documents created:
		| Id | Code | Description |
		| 1  | G    | Code G      |
		| 2  | S    | Code S      |
		| 3  | W    | Code W      |
	When I delete document with id 2
	And I get all documents
	Then there should be 2 documents returned

Scenario: Truncate clearance codes
	Given 5 documents created
	When I truncate documents
	And I get all documents
	Then no documents should be returned

Scenario: Counting all clearance codes
	Given 3 documents created
	When I count the number of documents
	Then there should be 3 documents counted

Scenario Outline: Get clearance code by external reference
	Given the following documents created:
		| Id | Code | Description |
		| 1  | G    | Code G      |
		| 2  | S    | Code S      |
		| 3  | W    | Code W      |
	When I call FindByExternal method with parameters:
		| Parameter   | Value  |
		| externalRef | <Code> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
		| Code | ExpectedReturnCount |
		| S    | 1                   |
		| s    | 1                   |
		| G    | 1                   |
		| K    | 0                   |

Scenario Outline: Get clearance codes by external references
	Given the following documents created:
		| Id | Code | Description |
		| 1  | G    | Code G      |
		| 2  | S    | Code S      |
		| 3  | W    | Code W      |
	When I call FindByExternal method with parameters:
		| Parameter    | Value   |
		| externalRefs | <Codes> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
		| Codes   | ExpectedReturnCount |
		| S, G    | 2                   |
		| g, w    | 2                   |
		| W, t    | 1                   |
		| K, m, t | 0                   |

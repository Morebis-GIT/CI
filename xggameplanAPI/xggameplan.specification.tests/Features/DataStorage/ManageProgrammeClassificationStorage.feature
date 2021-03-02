@ManageDataStorage

Feature: ManageProgrammeClassificationDataStorage
	In order to manage programme classifications
	As a user
	I want to store programme classifications via ProgrammeClassifications repository

Background: 
	Given there is a ProgrammeClassifications repository

Scenario: Add new ProgrammeClassifications
	When I create the following documents:
		| Uid | Code | Description    |
		| 1   | G    | General        |
		| 2   | X    | Restricted 18+ |
		| 3   | C    | Childrens      |
	And I get all documents
	Then there should be 3 documents returned

Scenario: Get all ProgrammeClassifications
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Counting all ProgrammeClassifications
	Given 3 documents created
	When I count the number of documents
	Then there should be 3 documents counted

Scenario: Truncating ProgrammeClassifications documents
	Given 5 documents created
	When I truncate documents
	And I get all documents
	Then no documents should be returned

Scenario: Remove an existing ProgrammeClassification
	Given the following documents created:
		| Uid | Code | Description    |
		| 1   | G    | General        |
		| 2   | X    | Restricted 18+ |
		| 3   | C    | Childrens      |
	When I delete document with id '2'
	And I get all documents
	Then there should be 2 documents returned

Scenario: Removing a non-existing ProgrammeClassification
	Given the following documents created:
		| Uid | Code | Description    |
		| 1   | G    | General        |
		| 2   | X    | Restricted 18+ |
		| 3   | C    | Childrens      |
	When I try to delete document with id '123'
	And I get all documents
	Then there should be 3 documents returned

Scenario: Get a non-existing ProgrammeClassification by id
	Given the following documents created:
		| Uid | Code | Description    |
		| 1   | G    | General        |
		| 2   | X    | Restricted 18+ |
		| 3   | C    | Childrens      |
	When I get document with id '312'
	Then no documents should be returned

Scenario: Get an existing ProgrammeClassification by id
	Given the following documents created:
		| Uid | Code | Description    |
		| 1   | G    | General        |
		| 2   | X    | Restricted 18+ |
		| 3   | C    | Childrens      |
	When I get document with id '2'
	Then there should be 1 documents returned
	And the received document should contain the following values:
         | Property    | Value          |
         | Uid         | 2              |
         | Code        | X              |
         | Description | Restricted 18+ |

Scenario: Update ProgrammeClassification
	Given the following documents created:
		| Uid | Code | Description    |
		| 1   | G    | General        |
		| 2   | X    | Restricted 18+ |
		| 3   | C    | Childrens      |
	When I get document with id '2'
	And I update received document by values:
         | Property    | Value             |
         | Uid         | 2                 |
         | Code        | MA                |
         | Description | Mature Adults 15+ |
	And I get document with id '2'
	Then the received document should contain the following values:
         | Property    | Value             |
         | Uid         | 2                 |
         | Code        | MA                |
         | Description | Mature Adults 15+ |

Scenario Outline: Get ProgrammeClassification by code
	Given the following documents created:
		| Uid | Code | Description    |
		| 1   | G    | General        |
		| 2   | X    | Restricted 18+ |
		| 3   | C    | Childrens      |
	When I call GetByCode method with parameters:
		| Parameter | Value  |
		| code      | <Code> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| Code | ExpectedReturnCount |
	| ""   | 0                   |
	| X    | 1                   |
	| XX   | 0                   |

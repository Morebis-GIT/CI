@ManageDataStorage

Feature: ManageRunTypeDataStorage
	In order to manage run types
	As a user
	I want to store run types via RunTypes repository

Background:
	Given there is a RunTypes repository
	And predefined RunTypes data

Scenario: Add one new RunType
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Get all RunTypes
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Get RunTypes by id
	Given the following documents created:
        | Id |	  Name	   |
        | 1  | NewRuntype1 |
        | 2  | NewRuntype2 |
        | 3  | NewRuntype3 |
	When I get document with id '2'
	Then there should be 1 documents returned

Scenario: Delete RunTypes by id
	Given the following documents created:
        | Id |	  Name	   |
        | 1  | NewRuntype1 |
        | 2  | NewRuntype2 |
        | 3  | NewRuntype3 |
	When I delete document with id 1
	And I get document with id 1
	Then no documents should be returned

Scenario: Update RunTypes
	Given the following documents created:
		| Id  |		Name    | Description | Hidden |
		| 1   | NewRuntype1 | NewRuntype1 | True   |
		| 2   | NewRuntype2 | OldRuntype2 | False  |
		| 3   | NewRuntype3 | NewRuntype3 | True   |
	When I get document with id 2
	Then there should be 1 documents returned
	When I update received document by values:
		| Property      | Value        |
		| Hidden        | True         |
		| Description   | NewRuntype2  |
	And I get document with id 2
	Then the received document should contain the following values:
		| Property            | Value  |
		| Hidden        | True         |
		| Description   | NewRuntype2  |
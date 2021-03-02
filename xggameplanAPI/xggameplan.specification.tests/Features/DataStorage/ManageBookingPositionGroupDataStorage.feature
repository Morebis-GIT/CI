@ManageDataStorage

Feature: Manage BookingPositionGroups data storage
	In order to manage BookingPositionGroups
	As an Airtime manager
	I want to store BookingPositionGroups in a data store

Background: 
	Given there is a BookingPositionGroups repository

Scenario: Get all BookingPositionGroups
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Get an existing BookingPositionGroup by id
	Given the following documents created:
		| Id | GroupId | Code |
		| 1  | 4       | FST  |
		| 2  | 5       | SND  |
		| 3  | 6       | THD  |
	When I get document with id 1
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property  | Value |
		| Id        | 1     |

Scenario: Get a nonexistent BookingPositionGroup by id
	Given the following documents created:
		| Id | GroupId | Code |
		| 1  | 4       | FST  |
		| 2  | 5       | SND  |
		| 3  | 6       | THD  |
	When I get document with id 5
	Then no documents should be returned

Scenario: Add new BookingPositionGroup
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Update exiting BookingPositionGroup
	Given the following documents created:
		| Id | GroupId | Code |
		| 1  | 4       | FST  |
		| 2  | 5       | SND  |
		| 3  | 6       | THD  |
	When I get document with id 1
	And I update received document by values:
		| Property   | Value  |
		| Code       | LST    |
	And I get document with id 1
	Then the received document should contain the following values:
		| Property   | Value  |
		| Code       | LST    |

Scenario: Delete BookingPositionGroup by id
	Given the following documents created:
		| Id | GroupId | Code |
		| 1  | 4       | FST  |
		| 2  | 5       | SND  |
		| 3  | 6       | THD  |
	When I delete document with id 1
	And I get all documents
	Then there should be 2 documents returned

Scenario: Delete a nonexistent BookingPositionGroup by id
	Given the following documents created:
		| Id | GroupId | Code |
		| 1  | 4       | FST  |
		| 2  | 5       | SND  |
		| 3  | 6       | THD  |
	When I delete document with id 5
	And I get all documents
	Then there should be 3 documents returned

Scenario Outline: Get BookingPositionGroups by GroupIds
	Given the following documents created:
		| Id | GroupId |
		| 1  | 4       |
		| 2  | 5       |
		| 3  | 6       |
	When I call GetByGroupIds method with parameters:
		| Parameter | Value      |
		| groupIds  | <GroupIds> |
	Then there should be <ExpectedReturnCount> documents returned
	Examples:
	| GroupIds | ExpectedReturnCount |
	| 4        | 1                   |
	| 4, 5, 6  | 3                   |
	| 9        | 0                   |

@ManageDataStorage

Feature: Manage BookingPositions data storage
	In order to manage BookingPositions
	As an Airtime manager
	I want to store BookingPositions in a data store

Background: 
	Given there is a BookingPositions repository

Scenario: Get all BookingPositions
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Get an existing BookingPosition by id
	Given the following documents created:
		| Id | Position | Abbreviation |
		| 1  | 1        | FST          |
		| 2  | 2        | SND          |
		| 3  | 3        | THD          |
	When I get document with id 1
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property  | Value |
		| Id        | 1     |

Scenario: Get a nonexistent BookingPosition by id
	Given the following documents created:
		| Id | Position | Abbreviation |
		| 1  | 1        | FST          |
		| 2  | 2        | SND          |
		| 3  | 3        | THD          |
	When I get document with id 5
	Then no documents should be returned

Scenario Outline: Get BookingPosition by position
	Given the following documents created:
		| Id | Position |
		| 1  | 97       |
		| 2  | 98       |
		| 3  | 99       |
	When I call GetByPosition method with parameters:
		| Parameter | Value      |
		| position  | <Position> |
	Then there should be <ExpectedReturnCount> documents returned
	Examples:
	| Position  | ExpectedReturnCount |
	| 97        | 1                   |
	| 100       | 0                   |

Scenario Outline: Get BookingPosition by positions
	Given the following documents created:
		| Id | Position |
		| 1  | 97       |
		| 2  | 98       |
		| 3  | 99       |
	When I call GetByPositions method with parameters:
		| Parameter | Value       |
		| positions | <Positions> |
	Then there should be <ExpectedReturnCount> documents returned
	Examples:
	| Positions | ExpectedReturnCount |
	| 98        | 1                   |
	| 97, 98    | 2                   |
	| 100       | 0                   |

@ManageDataStorage

Feature: ManageChannelsDataStorage
	In order to manage channels
	As a user
	I want to store channels via Channels repository

Background:
	Given there is a Channels repository

Scenario: Add new channels
	Given 1 documents created
	When I create 2 documents
	And I get all documents
	Then there should be 3 documents returned

Scenario: Get all channels
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Get channel by id
	Given the following documents created:
		| Id | Name | ShortName |
		| 1  | Nine | Nine      |
		| 2  | Sky  | Sky       |
		| 3  | Fox  | Fox       |
	When I get document with id '2'
	Then the received document should contain the following values:
		| Property  | Value |
		| Id        | 2     |
		| Name      | Sky   |
		| ShortName | Sky   |

Scenario: Get a non-existing channels by id
	Given the following documents created:
		| Id | Name | ShortName |
		| 1  | Nine | Nine      |
		| 2  | Sky  | Sky       |
		| 3  | Fox  | Fox       |
	When I get document with id '99'
	Then no documents should be returned

Scenario: Delete channel by id
	Given the following documents created:
		| Id | Name | ShortName |
		| 1  | Nine | Nine      |
		| 2  | Sky  | Sky       |
		| 3  | Fox  | Fox       |
	When I delete document with id '3'
	And I get all documents
	Then there should be 2 documents returned

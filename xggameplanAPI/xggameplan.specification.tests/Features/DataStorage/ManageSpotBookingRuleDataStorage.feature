@ManageDataStorage
@SqlServer
Feature: Manage SpotBookingRules data storage
	In order to manage SpotBookingRules
	As an Airtime manager
	I want to store SpotBookingRules in a data store

Background:
	Given there is a SpotBookingRules repository
	And predefined SalesAreas.json data 
	And predefined data imported

Scenario: Add new SpotBookingRules
	When I create 3 documents
	And I get all documents
	Then there should be 3 documents returned

Scenario: Get all SpotBookingRules
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Get SpotBookingRule by id
	Given the following documents created:
		| Id | SpotLength | MinBreakLength | MaxBreakLength | MaxSpots | BreakType |
		| 1  | 00:30:00   | 00:30:00       | 00:30:00       | 1        | BT1       |
		| 2  | 00:45:00   | 00:55:00       | 00:10:00       | 5        | BT3       |
	When I get document with id 1
	Then there should be 1 documents returned

Scenario: Truncate SpotBookingRules
	Given 5 documents created
	When I truncate documents
	And I get all documents
	Then no documents should be returned
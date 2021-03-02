@ManageDataStorage

Feature: Manage LockTypes data storage
	In order to manage LockTypes
	As an Airtime manager
	I want to store LockTypes in a data store

Background:
	Given there is a LockTypes repository

Scenario: Add new LockTypes
	When I create the following documents:
		| Id | LockType | Name      |
		| 1  | 1        | Break     |
		| 2  | 2        | SalesArea |
		| 3  | 3        | Programme |  
	And I get all documents
	Then there should be 3 documents returned

Scenario: Get all lockTypes
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Get lockType by id
	Given the following documents created:
		| Id | LockType | Name      |
		| 1  | 1        | Break     |
		| 2  | 2        | SalesArea |
		| 3  | 3        | Programme |       
	When I get document with id 1
	Then there should be 1 documents returned

Scenario: Truncate lockTypes
	Given 5 documents created
	When I truncate documents
	And I get all documents
	Then no documents should be returned

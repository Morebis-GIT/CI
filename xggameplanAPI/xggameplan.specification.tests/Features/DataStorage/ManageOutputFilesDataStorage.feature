@ManageDataStorage

Feature: Manage OutputFiles data storage
	In order to manage OutputFiles
	As an administrator
	I want to store OutputFiles in a data store

Background: 
	Given there is a OutputFiles repository

Scenario: Add new Output file
	Given 1 documents created
	When I get all documents
	Then there should be 1 documents returned

Scenario Outline: Get Output file by id
	Given the following documents created:
		| FileId   |
		| SDIS.out |
		| EFFE.out |
	When I get document with id '<FileId>'
	Then there should be <ExpectedCount> documents returned
	Examples:
		| FileId   | ExpectedCount |
		| EFFE.out | 1             |
		| AAA.out  | 0             |

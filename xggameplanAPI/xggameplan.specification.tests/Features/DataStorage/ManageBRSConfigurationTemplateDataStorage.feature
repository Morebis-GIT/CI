@ManageDataStorage
@SqlServer
Feature: Manage BRS Configuration Template data storage
	In order to manage BRS configuration templates
	As an Airtime manager
	I want to store BRS configuration templates in a data store
	
Background: 
	Given there is a BRSConfigurationTemplate repository
	
Scenario: Get all BRS configuration templates
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned
	
Scenario: Get an existing BRS configuration templates by id
	Given the following documents created:
		| Id | Name  | LastModified | IsDefault |
		| 1  | Conf1 | 01-01-2020   | true      |
		| 2  | Conf2 | 03-03-2020   | false     |
		| 3  | Conf3 | 02-03-2020   | false     |
	When I get document with id 1
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property     | Value      |
		| Id           | 1          |
		| Name         | Conf1      |
		| LastModified | 01-01-2020 |
		| IsDefault    | true       |
	
Scenario: Get a nonexistent BRS configuration template by id
	Given the following documents created:
		| Id | Name  | LastModified | IsDefault |
		| 1  | Conf1 | 01-01-2020   | true      |
		| 2  | Conf2 | 03-03-2020   | false     |
		| 3  | Conf3 | 02-03-2020   | false     |
	When I get document with id 5
	Then no documents should be returned
	
Scenario: Add new BRS configuration template
	When I create a document
	And I get all documents
	Then there should be 1 documents returned
	
Scenario: Update exiting BRS configuration template
	Given the following documents created:
		| Id | Name  | LastModified | IsDefault |
		| 1  | Conf1 | 01-01-2020   | true      |
		| 2  | Conf2 | 03-03-2020   | false     |
		| 3  | Conf3 | 02-03-2020   | false     |
	When I get document with id 2
	And I update received document by values:
		| Property | Value |
		| Name     | qqq   |
	And I get document with id 2
	Then the received document should contain the following values:
		| Property | Value |
		| Name     | qqq   |
	
Scenario: Delete BRS configuration template by id
	Given the following documents created:
		| Id | Name  | LastModified | IsDefault |
		| 1  | Conf1 | 01-01-2020   | true      |
		| 2  | Conf2 | 03-03-2020   | false     |
		| 3  | Conf3 | 02-03-2020   | false     |
	When I delete document with id 2
	And I get all documents
	Then there should be 2 documents returned

Scenario: Delete a nonexistent BRS configuration template by id
	Given the following documents created:
		| Id | Name  | LastModified | IsDefault |
		| 1  | Conf1 | 01-01-2020   | true      |
		| 2  | Conf2 | 03-03-2020   | false     |
		| 3  | Conf3 | 02-03-2020   | false     |
	When I delete document with id 5
	And I get all documents
	Then there should be 3 documents returned

Scenario: Counting all BRS configuration template
	Given 3 documents created
	When I count the number of documents
	Then there should be 3 documents counted
	
Scenario: Get default configuration templaate
	Given the following documents created:
		| Id | Name  | LastModified | IsDefault |
		| 1  | Conf1 | 01-01-2020   | false     |
		| 2  | Conf2 | 03-03-2020   | true      |
		| 3  | Conf3 | 02-03-2020   | false     |
	When I call GetDefault method
	Then the received document should contain the following values:
		| Property | Value |
		| Id       | 2     |

Scenario: Change default configuration
	Given the following documents created:
		| Id | Name  | LastModified | IsDefault |
		| 1  | Conf1 | 01-01-2020   | true      |
		| 2  | Conf2 | 03-03-2020   | false     |
		| 3  | Conf3 | 02-03-2020   | false     |
	When I call ChangeDefaultConfiguration method with parameters:
		| Parameter | Value |
		| id        | 2     |
	And I get document with id 2
	Then the received document should contain the following values:
		| Property  | Value |
		| IsDefault | true  |

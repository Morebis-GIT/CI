@ManageDataStorage

Feature: Manage AutoBook instance configurations data storage
	In order to manage AutoBook instance configurations
	As a user
	I want to store AutoBook instance configurations in a data store

Background:
	Given there is a AutoBookInstanceConfiguration repository

Scenario: Add new AutoBookInstanceConfigurations
    When I create 3 documents
    And I get all documents
    Then there should be 3 documents returned

Scenario: Get all AutoBookInstanceConfigurations
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Get an AutoBookInstanceConfiguration by id
	Given the following documents created:
		| Id | Description       | CloudProvider | InstanceType | StorageSizeGb | Cost | 
		| 1  | t3.medium (50GB)  | AWS           | t3.medium    | 50            | 24   | 
		| 2  | c5.xlarge (50GB)  | AWS           | c5.xlarge    | 50            | 36   | 
		| 3  | c5.2xlarge (50GB) | AWS           | c5.2xlarge   | 50            | 54   | 
	When I get document with id 2
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property      | Value            |
		| Id            | 2                |
		| Description   | c5.xlarge (50GB) |
		| CloudProvider | AWS              |
		| InstanceType  | c5.xlarge        |
		| StorageSizeGb | 50               |
		| Cost          | 36               |

Scenario: Get a non-existing AutoBookInstanceConfiguration by id
	Given the following documents created:
		| Id | Description       | CloudProvider | InstanceType | StorageSizeGb | Cost | 
		| 1  | t3.medium (50GB)  | AWS           | t3.medium    | 50            | 24   | 
		| 2  | c5.xlarge (50GB)  | AWS           | c5.xlarge    | 50            | 36   | 
		| 3  | c5.2xlarge (50GB) | AWS           | c5.2xlarge   | 50            | 54   | 
	When I get document with id 22
	Then no documents should be returned

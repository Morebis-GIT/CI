@ManageDataStorage

Feature: Manage MS Teams audit event settings data storage
	In order to manage MS Teams audit event settings
	As a system developer
	I want to store MS teams audit event settings in a data store

Background:
	Given there is a MSTeamsAuditEventSettings repository

Scenario: Add one new MS Teams Audit Event Setting
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Add several new MS Teams Audit Event Settings
	When I create the following documents:
		| EventTypeId | MessageCreatorId                     |
		| 25          | 1771D896-912C-42D3-8131-3740805D7772 |
		| 26          | C499E915-3AFB-46B4-B8ED-35A68D551ED4 |
		| 27          | 4AB41BB6-5CED-421F-93E1-F8C355728877 |
	And I get all documents
	Then there should be 3 documents returned

Scenario: Get MS Teams Audit Event Setting by id
	Given the following documents created:
		| EventTypeId | MessageCreatorId                     |
		| 25          | 1771D896-912C-42D3-8131-3740805D7772 |
		| 26          | C499E915-3AFB-46B4-B8ED-35A68D551ED4 |
		| 27          | 4AB41BB6-5CED-421F-93E1-F8C355728877 |
	When I get document with id '26'
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property         | Value                                |
		| MessageCreatorId | C499E915-3AFB-46B4-B8ED-35A68D551ED4 |

Scenario: Get all MS Teams Audit Event Settings
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Counting all MS Teams Audit Event Settings
	Given 3 documents created
	When I count the number of documents
	Then there should be 3 documents counted

Scenario: Delete an existing MS Teams Audit Event Setting by id
	Given the following documents created:
		| EventTypeId | MessageCreatorId                     |
		| 25          | 1771D896-912C-42D3-8131-3740805D7772 |
		| 26          | C499E915-3AFB-46B4-B8ED-35A68D551ED4 |
		| 27          | 4AB41BB6-5CED-421F-93E1-F8C355728877 |
	When I delete document with id 26
	And I get all documents
	Then there should be 2 documents returned

Scenario: Truncate MS Teams Audit Event Settings documents
	Given 3 documents created
	When I truncate documents
	And I get all documents
	Then no documents should be returned

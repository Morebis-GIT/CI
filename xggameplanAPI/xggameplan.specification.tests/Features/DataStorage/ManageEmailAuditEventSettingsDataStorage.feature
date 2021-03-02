@ManageDataStorage

Feature: Manage email audit event settings data storage
	In order to alert people of audit events via email
	As an alert monitor
	I want to store email audit event settings in a data store

Background: 
	Given there is a EmailAuditEventSettings repository

Scenario: Add an email audit event setting to the data store
    When I create the following documents:
         | EventTypeId | EmailCreatorId |
         | 1           | Default 1      |
         | 2           | Default 2      |
         | 3           | Default 3      |
	And I get all documents
	Then there should be 3 documents returned

Scenario: Remove an email audit event setting from the data store
	Given the following documents created:
         | EventTypeId | EmailCreatorId |
         | 1           | Default 1      |
         | 2           | Default 2      |
         | 3           | Default 3      |
	When I delete document with id 2
	And I get all documents
	Then there should be 2 documents returned

Scenario: Removing an email audit event setting not in the data store
	Given the following documents created:
         | EventTypeId | EmailCreatorId |
         | 1           | Default 1      |
         | 2           | Default 2      |
         | 3           | Default 3      |
	When I delete document with id 7
	And I get all documents
	Then there should be 3 documents returned

Scenario: Get all email audit event settings
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Truncating email audit event setting
	Given 5 documents created
	When I truncate documents
	And I get all documents
	Then no documents should be returned

Scenario: Updating email audit event setting
	Given the following documents created:
         | EventTypeId | EmailCreatorId |
         | 1           | Default 1      |
    When I get all documents
    And I update received document by values:
         | Parameter      | Value   |
         | EmailCreatorId | Default |
    And I get all documents
    Then the received document should contain the following values:
         | Parameter      | Value   |
         | EventTypeId    | 1       |
         | EmailCreatorId | Default |

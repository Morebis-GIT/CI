@ManageDataStorage

Feature: Manage AutoBook default parameters data storage
	In order to manage AutoBook default parameters
	As a user
	I want to store AutoBook default parameters in a data store

Background:
	Given there is a AutoBookDefaultParameters repository
	And predefined AutoBookDefaultParameters data

Scenario: Add new AutoBookDefaultParameters
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Get existing AutoBookDefaultParameters
	Given predefined data imported
	When I call Get method
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property | Value                                |
		| Id       | 67e0bffb-a45e-455a-a410-fa78977f0aba |

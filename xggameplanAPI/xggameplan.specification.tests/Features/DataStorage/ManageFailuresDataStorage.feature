@ManageDataStorage

Feature: ManageFailuresDataStorage
	In order to manage failures
	As a user
	I want to store failures via Failures repository

Background:
	Given there is a Failures repository
	And predefined Failures data

Scenario: Add new Failures
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Remove Failures
	Given predefined data imported
	When I delete document with id '2259294C-24E6-45B7-B1EF-2BAA53CF4278'
	And I get document with id '2259294C-24E6-45B7-B1EF-2BAA53CF4278'
	Then no documents should be returned

Scenario: Get a non-existing Failures by id
	Given predefined data imported
	When I get document with id '00000000-0000-0000-0000-000000000000'
	Then no documents should be returned

Scenario: Get an existing Failures by id
	Given predefined data imported
	When I get document with id '6D5304D7-5848-4577-8F31-65305E8DA0BD'
	Then there should be 1 documents returned
	And the received document should contain the following values:
         | Property | Value                                |
         | Id       | 6D5304D7-5848-4577-8F31-65305E8DA0BD |

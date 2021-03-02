@ManageDataStorage

Feature: Manage CampaignSettings data storage
	In order to advertise products
	As an Airtime manager
	I want to store campaign settings in a data store

	Background:
	Given there is a CampaignSettings repository
	And predefined CampaignSettings data

	Scenario: Add new CampaignSettings
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

	Scenario: Add many CampaignSettings
	When I create 3 documents
	And I get all documents
	Then there should be 3 documents returned

	Scenario: Get all CampaignSettings
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

	Scenario: Counting all CampaignSettings
	Given 3 documents created
	When I count the number of documents
	Then there should be 3 documents counted

	Scenario: Truncating CampaignSettings documents
	Given 5 documents created
	When I truncate documents
	And I get all documents
	Then no documents should be returned

	Scenario: Remove an existing CampaignSettings
	Given predefined data imported
	When I delete document with id '1'
	And I get all documents
	Then there should be 2 documents returned

	Scenario: Removing a non-existing CampaignSettings
	Given predefined data imported
	When I delete document with id '5'
	And I get all documents
	Then there should be 3 documents returned

	Scenario: Get a non-existing CampaignSettings by id
	Given predefined data imported
	When I get document with id '5'
	Then no documents should be returned

	Scenario: Get an existing CampaignSettings by id
	Given predefined data imported
	When I get document with id '1'
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property | Value |
		| Id       | 1     |

	Scenario: Get CampaignSettings by external reference
	Given predefined data imported
	When I call GetByExternalId method with parameters:
		| Parameter  | Value     |
		| externalId | Campaign2 |
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property           | Value     |
		| Id                 | 2         |
		| CampaignExternalId | Campaign2 |

	Scenario: Update CampaignSettings by external reference
	Given predefined data imported
	When I call GetByExternalId method with parameters:
		| Parameter  | Value     |
		| externalId | Campaign3 |
	And I update received document by values:
		| Property          | Value |
		| IncludeRightSizer | No    |
	And I call GetByExternalId method with parameters:
		| Parameter  | Value     |
		| externalId | Campaign3 |
	Then the received document should contain the following values:
		| Property           | Value     |
		| Id                 | 3         |
		| CampaignExternalId | Campaign3 |
		| IncludeRightSizer  | No        |
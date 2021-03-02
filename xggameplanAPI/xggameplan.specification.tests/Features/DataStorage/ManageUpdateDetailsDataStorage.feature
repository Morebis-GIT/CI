@ManageDataStorage

Feature: Manage UpdateDetails data storage
	In order to manage UpdateDetails
	As a user
	I want to store update details in a data store

Background: 
	Given there is a UpdateDetails repository

Scenario: Add new UpdateDetail
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Get all UpdateDetails
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Remove an existing UpdateDetail
	Given the following documents created:
		| Id                                   | Name                  | TimeApplied         |
		| 5A9C3130-A885-4F7B-AA27-E4DD6EDC0AA9 | Migrate sales areas   | 2019-01-16 06:11:24 |
		| 9F6A7794-30B1-4325-879F-B774B61EA8FA | Remove expired tokens | 2019-03-04 06:26:18 |
		| 5D9ACDF3-CDCC-4F23-AB1D-63D7227B8683 | Clean spots           | 2019-04-21 06:40:42 |
	When I delete document with id '9F6A7794-30B1-4325-879F-B774B61EA8FA'
	And I get all documents
	Then there should be 2 documents returned

Scenario: Removing a non-existing UpdateDetail
	Given the following documents created:
		| Id                                   | Name                  | TimeApplied         |
		| 5A9C3130-A885-4F7B-AA27-E4DD6EDC0AA9 | Migrate sales areas   | 2019-01-16 06:11:24 |
		| 9F6A7794-30B1-4325-879F-B774B61EA8FA | Remove expired tokens | 2019-03-04 06:26:18 |
		| 5D9ACDF3-CDCC-4F23-AB1D-63D7227B8683 | Clean spots           | 2019-04-21 06:40:42 |
	When I delete document with id '999654FE-42B3-4C3D-8654-74D0B28F074F'
	And I get all documents
	Then there should be 3 documents returned

Scenario: Get a non-existing UpdateDetail by id
	Given the following documents created:
		| Id                                   | Name                  | TimeApplied         |
		| 5A9C3130-A885-4F7B-AA27-E4DD6EDC0AA9 | Migrate sales areas   | 2019-01-16 06:11:24 |
		| 9F6A7794-30B1-4325-879F-B774B61EA8FA | Remove expired tokens | 2019-03-04 06:26:18 |
		| 5D9ACDF3-CDCC-4F23-AB1D-63D7227B8683 | Clean spots           | 2019-04-21 06:40:42 |
	When I get document with id '999654FE-42B3-4C3D-8654-74D0B28F074F'
	Then no documents should be returned

Scenario: Get an existing UpdateDetail by id
	Given the following documents created:
		| Id                                   | Name                  | TimeApplied         |
		| 5A9C3130-A885-4F7B-AA27-E4DD6EDC0AA9 | Migrate sales areas   | 2019-01-16 06:11:24 |
		| 9F6A7794-30B1-4325-879F-B774B61EA8FA | Remove expired tokens | 2019-03-04 06:26:18 |
		| 5D9ACDF3-CDCC-4F23-AB1D-63D7227B8683 | Clean spots           | 2019-04-21 06:40:42 |
	When I get document with id '5A9C3130-A885-4F7B-AA27-E4DD6EDC0AA9'
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property    | Value                                |
		| Id          | 5A9C3130-A885-4F7B-AA27-E4DD6EDC0AA9 |
		| Name        | Migrate sales areas                  |
		| TimeApplied | 2019-01-16 06:11:24                  |

Scenario: Update the UpdateDetails
	Given the following documents created:
		| Id                                   | Name                  | TimeApplied         |
		| 5A9C3130-A885-4F7B-AA27-E4DD6EDC0AA9 | Migrate sales areas   | 2019-01-16 06:11:24 |
		| 9F6A7794-30B1-4325-879F-B774B61EA8FA | Remove expired tokens | 2019-03-04 06:26:18 |
		| 5D9ACDF3-CDCC-4F23-AB1D-63D7227B8683 | Clean spots           | 2019-04-21 06:40:42 |
	When I get document with id '5A9C3130-A885-4F7B-AA27-E4DD6EDC0AA9'
	And I update received document by values:
		| Property    | Value                |
		| Name        | Migrate restrictions |
		| TimeApplied | 2019-02-15 03:45:21  |
	And I get document with id '5A9C3130-A885-4F7B-AA27-E4DD6EDC0AA9'
	Then the received document should contain the following values:
		| Property    | Value                |
		| Name        | Migrate restrictions |
		| TimeApplied | 2019-02-15 03:45:21  |

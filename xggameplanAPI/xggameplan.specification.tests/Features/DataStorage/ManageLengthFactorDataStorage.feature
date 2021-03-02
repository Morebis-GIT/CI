@ManageDataStorage
@SqlServer
Feature: Manage Length Factor data storage
	In order to manage length factors
	As an Airtime manager
	I want to store length factor in a data store

Background: 
	Given there is a LengthFactor repository
	And predefined LengthFactor.SalesAreas.json data
	And predefined data imported

Scenario: Get all length factors
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Get an existing length factor by id
	Given the following documents created:
		| Id | SalesArea | Duration | Factor |
		| 1  | TCN94     | 1:00:00  | 0.1    |
		| 2  | QTQ93     | 1:50:00  | 0.35   |
		| 3  | STW93     | 0:05:00  | 0.55   |
	When I get document with id 1
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property  | Value   |
		| Id        | 1       |

Scenario: Get a nonexistent length factors by id
	Given the following documents created:
		| Id | SalesArea | Duration | Factor |
		| 1  | TCN94     | 1:00:00  | 0.1    |
		| 2  | QTQ93     | 1:50:00  | 0.35   |
		| 3  | STW93     | 0:05:00  | 0.55   |
	When I get document with id 5
	Then no documents should be returned
	
Scenario: Add new length factor
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Update exiting length factor
	Given the following documents created:
		| Id | SalesArea | Duration | Factor |
		| 1  | TCN94     | 1:00:00  | 0.1    |
		| 2  | QTQ93     | 1:50:00  | 0.35   |
		| 3  | STW93     | 0:05:00  | 0.55   |
	When I get document with id 2
	And I update received document by values:
		| Property | Value |
		| Factor   | 0.123  |
	And I get document with id 2
	Then the received document should contain the following values:
		| Property  | Value   |
		| Factor    | 0.123   |

Scenario: Delete length factor by id
	Given the following documents created:
		| Id | SalesArea | Duration | Factor |
		| 1  | TCN94     | 1:00:00  | 0.1    |
		| 2  | QTQ93     | 1:50:00  | 0.35   |
		| 3  | STW93     | 0:05:00  | 0.55   |
	When I delete document with id 2
	And I get all documents
	Then there should be 2 documents returned

Scenario: Delete a nonexistent length factor by id
	Given the following documents created:
		| Id | SalesArea | Duration | Factor |
		| 1  | TCN94     | 1:00:00  | 0.1    |
		| 2  | QTQ93     | 1:50:00  | 0.35   |
		| 3  | STW93     | 0:05:00  | 0.55   |
	When I delete document with id 5
	And I get all documents
	Then there should be 3 documents returned

Scenario: Truncating length factors
	Given 3 documents created
	When I truncate documents
	And I get all documents
	Then no documents should be returned

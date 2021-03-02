@ManageDataStorage

Feature: Manage Delivery Capping Group data storage
	In order to manage delivery capping groups
	As an Airtime manager
	I want to store delivery capping groups in a data store
	
Background: 
	Given there is a DeliveryCappingGroup repository
	
Scenario: Get all delivery capping groups
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned
	
Scenario: Get an existing delivery capping group by id
	Given the following documents created:
		| Id | Description | Percentage | ApplyToPrice |
		| 1  | Group1      | 100        | true         |
		| 2  | Group2      | 150        | true         |
		| 3  | Group3      | 95         | false        |
	When I get document with id 1
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property  | Value |
		| Id        | 1     |
	
Scenario: Get a nonexistent delivery capping group by id
	Given the following documents created:
		| Id | Description | Percentage | ApplyToPrice |
		| 1  | Group1      | 100        | true         |
		| 2  | Group2      | 150        | true         |
		| 3  | Group3      | 95         | false        |
	When I get document with id 5
	Then no documents should be returned

Scenario: Get an existing delivery capping groups by ids
	Given the following documents created:
		| Id | Description | Percentage | ApplyToPrice |
		| 1  | Group1      | 100        | true         |
		| 2  | Group2      | 150        | true         |
		| 3  | Group3      | 95         | false        |
	When I call Get method with parameters:
		| Parameter   | Value  |
		| ids         | 2, 3   |
	Then there should be 2 documents returned

	
Scenario: Add new delivery capping group
	When I create a document
	And I get all documents
	Then there should be 1 documents returned
	
Scenario: Update exiting delivery capping group
	Given the following documents created:
		| Id | Description | Percentage | ApplyToPrice |
		| 1  | Group1      | 100        | true         |
		| 2  | Group2      | 150        | true         |
		| 3  | Group3      | 95         | false        |
	When I get document with id 2
	And I update received document by values:
		| Property    | Value  |
		| Percentage | 123    |
	And I get document with id 2
	Then the received document should contain the following values:
		| Property   | Value  |
		| Percentage | 123    |
	
Scenario: Delete delivery capping group by id
	Given the following documents created:
		| Id | Description | Percentage | ApplyToPrice |
		| 1  | Group1      | 100        | true         |
		| 2  | Group2      | 150        | true         |
		| 3  | Group3      | 95         | false        |
	When I delete document with id 2
	And I get all documents
	Then there should be 2 documents returned

Scenario: Delete a nonexistent delivery capping group by id
	Given the following documents created:
		| Id | Description | Percentage | ApplyToPrice |
		| 1  | Group1      | 100        | true         |
		| 2  | Group2      | 150        | true         |
		| 3  | Group3      | 95         | false        |
	When I delete document with id 5
	And I get all documents
	Then there should be 3 documents returned

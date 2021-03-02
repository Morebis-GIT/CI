@ManageDataStorage

Feature: Manage Sales Area Demographics data storage
	In order to manage sales area demographic
	As an Airtime manager
	I want to store sales area demographic in a data store
	
Background: 
	Given there is a SalesAreaDemographic repository
	And predefined SalesAreaDemographic.SalesAreas.json data
	And predefined data imported
	
Scenario: Get all sales area demographics
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned
	
Scenario: Add new sales area demographic
	When I create 3 documents
	And I get all documents
	Then there should be 3 documents returned
	
Scenario: Delete sales area demographics by sales area short name
	Given the following documents created:
		| Id | SalesArea | ExternalRef | Exclude | SupplierCode |
		| 1  | SN1         | 1           | true    | AT           |
		| 2  | SN1         | 2           | true    | AA           |
		| 3  | SN2         | 3           | false   | QQ           |
	When I call DeleteBySalesAreaName method with parameters:
		| Parameter   | Value |
		| name | SN1   |
	And I get all documents
	Then there should be 1 documents returned


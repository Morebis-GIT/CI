@ManageDataStorage

Feature: ManageRSSettingsDataStorage
	In order to manage RSSettings
	As a Airtime manager
	I want to store RSSettings via RSSettings repository

Background: 
	Given there is a RSSettings repository
	
Scenario: Add new RSSettings
	When I create 3 documents
	And I get all documents
	Then there should be 3 documents returned

Scenario: Get all RSSettings
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Update a RSSettings
	Given the following documents created:
		| Id | SalesArea |
		| 2  | TM1       |
		| 3  | TM3       |
		| 4  | TM2       |
	When I get document with id '2'
	And I update received document by values:
		| Property  | Value    |
		| SalesArea | TM77     |
	And I get document with id '2'
	Then the received document should contain the following values:
		| Property                   | Value    |
		| SalesArea                  | TM77     |

Scenario: Remove an existing RSSettings
	Given the following documents created:
		| Id | SalesArea |
		| 2  | TM1       |
		| 3  | TM3       |
		| 4  | TM2       |
		| 5  | TM5       |
	When I call Delete method with parameters:
		| Parameter | Value |
		| salesArea | TM3   |
	And I get all documents
	Then there should be 3 documents returned

Scenario: Removing a non-existing RSSettings
	Given the following documents created:
		| Id | SalesArea |
		| 2  | TM1       |
		| 3  | TM3       |
		| 4  | TM2       |
		| 5  | TM5       |
	When I call Delete method with parameters:
		| Parameter | Value  |
		| salesArea | TM5675 |
	And I get all documents
	Then there should be 4 documents returned

Scenario Outline: Find RSSettings by salesArea
	Given the following documents created:
		| Id | SalesArea |
		| 2  | TM1       |
		| 3  | TM3       |
		| 4  | TM2       |
	When I call Find method with parameters:
        | Parameter | Value         |
        | salesArea | <ExternalRef> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples: 
		| ExternalRef | ExpectedReturnCount |
		| TM1         | 1                   |
		| TM67        | 0                   |
		| ""          | 0                   |

Scenario Outline: Find RSSettings list by salesAreas
	Given the following documents created:
		| Id | SalesArea |
		| 2  | TM1       |
		| 3  | TM3       |
		| 4  | TM2       |
	When I call FindBySalesAreas method with parameters:
        | Parameter  | Value        |
        | salesAreas | <SalesAreas> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples: 
		| SalesAreas     | ExpectedReturnCount |
		| TM1            | 1                   |
		| TM1, TM2       | 2                   |
		| TM1, TM2, TM3  | 3                   |
		| TM0            | 0                   |

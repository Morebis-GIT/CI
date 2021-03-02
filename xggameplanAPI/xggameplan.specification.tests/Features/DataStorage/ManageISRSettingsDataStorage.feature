@ManageDataStorage

Feature: ManageISRSettingsDataStorage
	In order to manage ISRSettings
	As a Airtime manager
	I want to store ISRSettings via ISRSettings repository

Background: 
	Given there is a ISRSettings repository
	And predefined ISRSettings.SalesAreas.json data
	And predefined data imported
	
Scenario: Add new ISRSettings
	When I create 3 documents
	And I get all documents
	Then there should be 3 documents returned

Scenario: Get all ISRSettings
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Update a ISRSettings
	Given the following documents created:
		| Id | SalesArea | DefaultEfficiencyThreshold | StartTime | EndTime  | BreakType |
		| 2  | TM1       | 3                          | 00:06:30  | 00:06:45 | I         |
		| 3  | TM3       | 7                          | 00:06:30  | 00:06:45 | I         |
		| 4  | TM2       | 5                          | 00:06:30  | 00:06:45 | I         |
	When I get document with id '2'
	And I update received document by values:
		| Property  | Value    |
		| SalesArea | TM77     |
		| StartTime | 00:07:30 |
		| EndTime   | 00:07:45 |
	And I get document with id '2'
	Then the received document should contain the following values:
		| Property                   | Value    |
		| SalesArea                  | TM77     |
		| DefaultEfficiencyThreshold | 3        |
		| StartTime                  | 00:07:30 |
		| EndTime                    | 00:07:45 |
		| BreakType                  | I        |

Scenario: Remove an existing ISRSettings
	Given the following documents created:
		| Id | SalesArea | DefaultEfficiencyThreshold | StartTime | EndTime  | BreakType |
		| 2  | TM1       | 3                          | 00:06:30  | 00:06:45 | I         |
		| 3  | TM3       | 7                          | 00:06:30  | 00:06:45 | I         |
		| 4  | TM2       | 5                          | 00:06:30  | 00:06:45 | I         |
		| 5  | TM5       | 5                          | 00:06:30  | 00:06:45 | I         |
	When I call Delete method with parameters:
		| Parameter | Value |
		| salesArea | TM3   |
	And I get all documents
	Then there should be 3 documents returned

Scenario: Removing a non-existing ISRSettings
	Given the following documents created:
		| Id | SalesArea | DefaultEfficiencyThreshold | StartTime | EndTime  | BreakType |
		| 2  | TM1       | 3                          | 00:06:30  | 00:06:45 | I         |
		| 3  | TM3       | 7                          | 00:06:30  | 00:06:45 | I         |
		| 4  | TM2       | 5                          | 00:06:30  | 00:06:45 | I         |
		| 5  | TM5       | 5                          | 00:06:30  | 00:06:45 | I         |
	When I try to call Delete method with parameters:
		| Parameter | Value  |
		| salesArea | TM5675 |
	Then the exception is thrown

Scenario Outline: Find ISRSettings by salesArea
	Given the following documents created:
		| Id | SalesArea | DefaultEfficiencyThreshold | StartTime | EndTime  | BreakType |
		| 2  | TM1       | 3                          | 00:06:30  | 00:06:45 | I         |
		| 3  | TM3       | 7                          | 00:06:30  | 00:06:45 | I         |
		| 4  | TM2       | 5                          | 00:06:30  | 00:06:45 | I         |
	When I call Find method with parameters:
        | Parameter | Value         |
        | salesArea | <ExternalRef> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples: 
		| ExternalRef | ExpectedReturnCount |
		| TM1         | 1                   |
		| TM67        | 0                   |
		| ""          | 0                   |

Scenario Outline: Find ISRSettings list by salesAreas
	Given the following documents created:
		| Id | SalesArea | DefaultEfficiencyThreshold | StartTime | EndTime  | BreakType |
		| 1  | TM1       | 3                          | 00:06:30  | 00:06:45 | I         |
		| 2  | TM3       | 7                          | 00:06:30  | 00:06:45 | I         |
		| 3  | TM2       | 5                          | 00:06:30  | 00:06:45 | I         |
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

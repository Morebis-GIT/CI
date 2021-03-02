@ManageDataStorage

Feature: ManageTenantSettingsStorage
	In order to manage tenant settings
	As a user
	I want to store tenant settings via TennantSettings repository

Background:
	Given there is a TenantSettings repository

Scenario: Add new TenantSettings
	When I create a document
	When I call Get method
	Then there should be 1 documents returned

Scenario: Update TenantSettings
	When I create a document
	And I update received document by values:
		| Property    | Value  |
		| PeakEndTime | 180000 |
	When I call Get method
	Then the received document should contain the following values:
		| Property    | Value  |
		| PeakEndTime | 180000 |

Scenario: GetDefaultSalesAreaPassPriorityId
	When I create a document
	And I update received document by values:
		| Property                       | Value                                |
		| DefaultSalesAreaPassPriorityId | CC18551E-0A0F-4B04-A7CC-1225044B75C0 |
	When I call GetDefaultSalesAreaPassPriorityId method
	Then the received document should contain the following values:
		| CC18551E-0A0F-4B04-A7CC-1225044B75C0 |

Scenario: GetDefaultScenarioId
	When I create a document
	And I update received document by values:
		| Property          | Value                                |
		| DefaultScenarioId | 0931C8DB-6C9C-42DF-B238-D307754A2713 |
	When I call GetDefaultScenarioId method
	Then the received document should contain the following values:
		| 0931C8DB-6C9C-42DF-B238-D307754A2713 |

Scenario: GetStartDayOfWeek
	When I create a document
	And I update received document by values:
		| Property       | Value    |
		| StartDayOfWeek | Saturday |
	When I call GetStartDayOfWeek method
	Then the received document should contain the following values:
		| Saturday |

Scenario: GetStartDayOfWeekLowerCase
	When I create a document
	And I update received document by values:
		| Property       | Value  |
		| StartDayOfWeek | monday |
	When I call GetStartDayOfWeek method
	Then the received document should contain the following values:
		| Monday |

Scenario: UpdateStartDayOfWeekWIthInvalidData
	When I create a document
	And I try to update received document by values:
		| Property       | Value   |
		| StartDayOfWeek | Pinguin |
	Then the exception is thrown
	And the exception type is InvalidOperationException

@ManageDataStorage

Feature: Manage AutoBook settings data storage
	In order to manage AutoBook settings
	As a user
	I want to store AutoBook settings in a data store

Background:
	Given there is a AutoBookSettings repository

Scenario: Add new AutoBookSettings
	Given the following documents created:
		| Id | ProvisioningAPIURL | AutoProvisioning | AutoProvisioningLastActive | MinInstances | MaxInstances | SystemMaxInstances | ApplicationVersion | BinariesVersion |
		| 1  | testUtl            | true             | 2019-01-01 09:00           | 0            | 9            | 10                 | 1.0                | 1.0             |
	When I call Get method
	Then there should be 1 documents returned

Scenario: Get AutoBookSettings
	Given the following documents created:
		| Id | ProvisioningAPIURL | AutoProvisioning | AutoProvisioningLastActive | MinInstances | MaxInstances | SystemMaxInstances | ApplicationVersion | BinariesVersion |
		| 1  | testUtl1           | true             | 2019-01-01 09:00           | 0            | 9            | 10                 | 1.0                | 1.0             |
	When I call Get method
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property                   | Value            |
		| ProvisioningAPIURL         | testUtl1         |
		| AutoProvisioning           | true             |
		| AutoProvisioningLastActive | 2019-01-01 09:00 |
		| MinInstances               | 0                |
		| MaxInstances               | 9                |
		| SystemMaxInstances         | 10               |
		| ApplicationVersion         | 1.0              |
		| BinariesVersion            | 1.0              |

Scenario: Update AutoBookSettings
	Given the following documents created:
		| Id | ProvisioningAPIURL | AutoProvisioning | AutoProvisioningLastActive | MinInstances | MaxInstances | SystemMaxInstances | ApplicationVersion | BinariesVersion |
		| 1  | testUtl1           | true             | 2019-01-01 09:00           | 0            | 9            | 10                 | 1.0                | 1.0             |
	When I call Get method
	And I update received document by values:
		| Property                   | Value            |
		| ProvisioningAPIURL         | changedUrl       |
		| AutoProvisioning           | true             |
		| AutoProvisioningLastActive | 2025-01-01 09:00 |
		| MinInstances               | 0                |
		| MaxInstances               | 2                |
		| SystemMaxInstances         | 3                |
		| ApplicationVersion         | 4.0              |
		| BinariesVersion            | 5.0              |
	And I call Get method
	Then the received document should contain the following values:
		| Property                   | Value            |
		| ProvisioningAPIURL         | changedUrl       |
		| AutoProvisioning           | true             |
		| AutoProvisioningLastActive | 2025-01-01 09:00 |
		| MinInstances               | 0                |
		| MaxInstances               | 2                |
		| SystemMaxInstances         | 3                |
		| ApplicationVersion         | 4.0              |
		| BinariesVersion            | 5.0              |

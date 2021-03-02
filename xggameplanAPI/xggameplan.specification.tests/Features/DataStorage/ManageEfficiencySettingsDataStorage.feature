@ManageDataStorage

Feature: ManageEfficiencySettingsDataStorage
	In order to manage efficiencySettings
	As a user
	I want to store efficiencySettings via EfficiencySettings repository

Background:
	Given there is a EfficiencySettings repository

Scenario: Update EfficiencySettings
	Given predefined DefaultEfficiencySettings data
	And predefined data imported
	When I call GetDefault method
	And I update received document by values:
		| Parameter                   | Value         |
		| efficiencyCalculationPeriod | NumberOfWeeks |
		| defaultNumberOfWeeks        | 1             |
		| persistEfficiency           | NightRun      |
	And I call GetDefault method
	Then the received document should contain the following values:
		| Parameter                   | Value         |
		| efficiencyCalculationPeriod | NumberOfWeeks |
		| defaultNumberOfWeeks        | 1             |
		| persistEfficiency           | NightRun      |

Scenario: Get default EfficiencySettings
	Given predefined DefaultEfficiencySettings data
	And predefined data imported
	When  I call GetDefault method
	Then there should be 1 documents returned

Scenario: Get non-existing EfficiencySettings
	When  I try to call GetDefault method
	Then the exception is thrown

Scenario: Get multiple EfficiencySettings
	Given predefined MultipleEfficiencySettings data
	And predefined data imported
	When  I try to call GetDefault method
	Then the exception is thrown

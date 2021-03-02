@ManageDataStorage

Feature: Manage RSGlobalSettings data storage
	In order to manage rsGlobalSettings
	As a user
	I want to store rsGlobalSettings via RSGlobalSettings repository

Background:
	Given there is a RSGlobalSettings repository

Scenario: Update rsGlobalSettings
	Given predefined DefaultRSGlobalSettings data
	And predefined data imported
	When I call Get method
	And I update received document by values:
		| Parameter                                 | Value |
		| excludeSpotsBookedByProgrammeRequirements | true  |
	And I call Get method
	Then the received document should contain the following values:
		| Parameter                                 | Value |
		| excludeSpotsBookedByProgrammeRequirements | true  |

Scenario: Get default rsGlobalSettings
	Given predefined DefaultRSGlobalSettings data
	And predefined data imported
	When  I call Get method
	Then there should be 1 documents returned

Scenario: Get non-existing rsGlobalSettings
	When  I try to call Get method
	Then the received document should contain the following values:
		| Parameter                                 | Value |
		| excludeSpotsBookedByProgrammeRequirements | false |

Scenario: Get multiple rsGlobalSettings
	Given predefined MultipleRSGlobalSettings data
	And predefined data imported
	When  I try to call Get method
	Then the exception is thrown

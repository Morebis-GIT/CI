@ManageDataStorage

Feature: ManageISRGlobalSettingsDataStorage
In order to manage isrGlobalSettings
As a user
I want to store isrGlobalSettings via ISRGlobalSettings repository

Background:
Given there is a ISRGlobalSettings repository

Scenario: Update isrGlobalSettings
Given predefined ISRGlobalSettings data
And predefined data imported
When I call Get method
And I update received document by values:
| Parameter                                 | Value |
| excludeSpotsBookedByProgrammeRequirements | true  |

And I call Get method
Then the received document should contain the following values:
| Parameter                                 | Value |
| excludeSpotsBookedByProgrammeRequirements | true  |

Scenario: Get default isrGlobalSettings
Given predefined ISRGlobalSettings data
And predefined data imported
When  I call Get method
Then there should be 1 documents returned

Scenario: Get non-existing isrGlobalSettings
When  I try to call Get method
Then the received document should contain the following values:
| Parameter                                 | Value |
| excludeSpotsBookedByProgrammeRequirements | false |

Scenario: Get multiple isrGlobalSettings
Given predefined MultipleISRGlobalSettings data
And predefined data imported
When  I try to call Get method
Then the exception is thrown

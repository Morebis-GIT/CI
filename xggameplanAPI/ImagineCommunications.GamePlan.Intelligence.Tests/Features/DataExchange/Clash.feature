@Exchange @SqlServer
Feature: Clash
	In order to synchronize data with xgGamePlan
	As a landmark user
	I want to send data to GamePlan

Background:
	Given GroupTransactionInfo for 1 event sent


Scenario: BulkClashCreatedOrUpdated successfully received and saved to empty Clash table
	Given I have BulkClashCreatedOrUpdated message to publish
		| ParentExternalIdentifier | Description | ExposureCount | ExternalRef |
		|                          | BASE        | 5423          | asdas       |
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Clashes data in GamePlan database is updated as following
		| ParentExternalIdentifier | Description | DefaultOffPeakExposureCount | ExternalRef |
		|                          | BASE        | 5423                        | asdas       |


Scenario: BulkClashCreatedOrUpdated successfully received and saved to empty Clash table linked Clashes
	Given I have BulkClashCreatedOrUpdated message to publish
		| ParentExternalIdentifier | Description | ExposureCount | ExternalRef |
		|                          | PARENT      | 5424          | asdas       |
		| asdas                    | BASE        | 5423          | ref         |
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Clashes data in GamePlan database is updated as following
		| ParentExternalIdentifier | Description | DefaultOffPeakExposureCount | ExternalRef |
		|                          | PARENT      | 5424                        | asdas       |
		| asdas                    | BASE        | 5423                        | ref         |


Scenario: BulkClashCreatedOrUpdated successfully received and Clashes table updated with new data
	Given The Clashes data exists in GamePlan database
		| Uid                                  | ParentExternalIdentifier | Description  | DefaultOffPeakExposureCount | ExternalRef |
		| 6942de2b-4e62-4bb0-baf1-65102c346865 |                          | first clash  | 1                           | abcd        |
		| 6942de2b-4e62-4bb0-baf1-65102c346866 |                          | second clash | 1                           | abce        |
		| 6942de2b-4e62-4bb0-baf1-65102c346867 |                          | third clash  | 10                          | abcd1       |
	And I have BulkClashCreatedOrUpdated message to publish
		| ParentExternalIdentifier | Description | ExposureCount | ExternalRef |
		|                          | BASE        | 5423          | asdas       |
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Clashes data in GamePlan database is updated as following
		| Uid                                  | ParentExternalIdentifier | Description  | DefaultOffPeakExposureCount | ExternalRef |
		| 6942de2b-4e62-4bb0-baf1-65102c346865 |                          | first clash  | 1                           | abcd        |
		| 6942de2b-4e62-4bb0-baf1-65102c346866 |                          | second clash | 1                           | abce        |
		| 6942de2b-4e62-4bb0-baf1-65102c346867 |                          | third clash  | 10                          | abcd1       |
		|                                      |                          | BASE         | 5423                        | asdas       |

Scenario: BulkClashCreatedOrUpdated model is invalid
	Given I have BulkClashCreatedOrUpdated message to publish
		| ParentExternalIdentifier | Description | ExposureCount | ExternalRef |
		|                          |             | 0             | asdas       |
		|                          |             | 2             | asdac       |
		|                          | test        | 2             | asdac       |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName          |
		| Data[0].ExposureCount |
		| Data[0].Description   |
		| Data[1].Description   |
		| DuplicateExternalRefs |

Scenario: BulkClashCreatedOrUpdated model property validations failed
	Given I have BulkClashCreatedOrUpdated message to publish
		| ParentExternalIdentifier | Description | ExposureCount | Externalref |
		|                          | test        | 1             |             |
		|                          |             | 2             | asdac6      |
		|                          | test1       | 3             | asd         |
		|                          | test2       | 4             | refref1     |
		|                          | test3       | 0             | ref         |
		| per1                     | test        | 3             | asd         |
		| per                      | test        | 3             | PER         |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName                  |
		| Data[0].Externalref           |
		| Data[1].Description           |
		| Data[3].Externalref.Length    |
		| Data[4].ExposureCount         |
		| Data[0].SameParentExternalRef |
		| Data[6].SameParentExternalRef |
		| DuplicateExternalRefs         |


Scenario: BulkClashCreatedOrUpdated published with valid data
	Given The Clashes data exists in GamePlan database
		| Uid                                  | ParentExternalIdentifier | Description  | DefaultOffPeakExposureCount | ExternalRef |
		| 6942de2b-4e62-4bb0-baf1-65102c346865 |                          | first clash  | 1                           | abcd        |
		| 6942de2b-4e62-4bb0-baf1-65102c346866 |                          | second clash | 1                           | abce        |
		| 6942de2b-4e62-4bb0-baf1-65102c346867 |                          | third clash  | 10                          | abcd1       |
	And I have BulkClashCreatedOrUpdated message to publish
		| ParentExternalIdentifier | Description  | ExposureCount | ExternalRef |
		|                          | first clash  | 1             | 12341       |
		|                          | second clash | 1             | 12342       |
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Clashes data in GamePlan database is updated as following
		| Uid                                  | ParentExternalIdentifier | Description  | DefaultOffPeakExposureCount | ExternalRef |
		| 6942de2b-4e62-4bb0-baf1-65102c346865 |                          | first clash  | 1                           | abcd        |
		| 6942de2b-4e62-4bb0-baf1-65102c346866 |                          | second clash | 1                           | abce        |
		| 6942de2b-4e62-4bb0-baf1-65102c346867 |                          | third clash  | 10                          | abcd1       |
		|                                      |                          | first clash  | 1                           | 12341       |
		|                                      |                          | second clash | 1                           | 12342       |

Scenario: BulkClashCreatedOrUpdated with empty data validation excepion
	Given I have BulkClashCreatedOrUpdated message to publish
		| ParentExternalIdentifier | Description | ExposureCount | ExternalRef |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName |
		| Data         |

Scenario: BulkClashCreatedOrUpdated validation fails same externalRef
	Given I have BulkClashCreatedOrUpdated message to publish
		| ParentExternalIdentifier | Description | ExposureCount | ExternalRef |
		| asdas                    |             | 0             | asdas       |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName                  |
		| Data[0].ExposureCount         |
		| Data[0].Description           |
		| Data[0].SameParentExternalRef |


Scenario: BulkClashCreatedOrUpdated published with non-existing ExternalRef and existing ExternalRef
	Given The Clashes data exists in GamePlan database
		| Uid                                  | ParentExternalIdentifier | Description  | DefaultOffPeakExposureCount | ExternalRef |
		| 6942de2b-4e62-4bb0-baf1-65102c346865 |                          | first clash  | 1                           | abcd        |
		| 6942de2b-4e62-4bb0-baf1-65102c346866 |                          | second clash | 1                           | abce        |
		| 6942de2b-4e62-4bb0-baf1-65102c346867 |                          | third clash  | 10                          | abcd1       |
	And I have BulkClashCreatedOrUpdated message to publish
		| ParentExternalIdentifier | Description | ExposureCount | ExternalRef |
		|                          | UPDATE      | 2             | abcd        |
		|                          | BASE        | 10            | 0000        |
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Clashes data in GamePlan database is updated as following
		| Uid                                  | ParentExternalIdentifier | Description | DefaultOffPeakExposureCount | ExternalRef |
		| 6942de2b-4e62-4bb0-baf1-65102c346865 |                          | UPDATE      | 2                           | abcd        |
		| 6942de2b-4e62-4bb0-baf1-65102c346866 |                          | second clash | 1                           | abce        |
		| 6942de2b-4e62-4bb0-baf1-65102c346867 |                          | third clash  | 10                          | abcd1       |
		|                                      |                          | BASE         | 10                          | 0000        |

Scenario: BulkClashCreatedOrUpdated published with non-existing ParentExternalIdentifier
	Given The Clashes data exists in GamePlan database
		| Uid                                  | ParentExternalIdentifier | Description  | DefaultOffPeakExposureCount | ExternalRef |
		| 6942de2b-4e62-4bb0-baf1-65102c346865 |                          | first clash  | 1                           | abcd        |
		| 6942de2b-4e62-4bb0-baf1-65102c346866 |                          | second clash | 1                           | abce        |
		| 6942de2b-4e62-4bb0-baf1-65102c346867 |                          | third clash  | 10                          | abcd1       |
	And I have BulkClashCreatedOrUpdated message to publish
		| ParentExternalIdentifier | Description | ExposureCount | ExternalRef |
		| hi                       | BASE        | 10            | abcd        |
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: Clash_ParentDoesNotExist

Scenario: BulkClashCreatedOrUpdated published with higher ExposureCount then parent's ExposureCount
	Given The Clashes data exists in GamePlan database
		| Uid                                  | ParentExternalIdentifier | Description  | DefaultOffPeakExposureCount | ExternalRef |
		| 6942de2b-4e62-4bb0-baf1-65102c346865 |                          | first clash  | 1                           | abcd        |
		| 6942de2b-4e62-4bb0-baf1-65102c346866 |                          | second clash | 1                           | abce        |
		| 6942de2b-4e62-4bb0-baf1-65102c346867 |                          | third clash  | 10                          | abcd1       |
	And I have BulkClashCreatedOrUpdated message to publish
		| ParentExternalIdentifier | Description | ExposureCount | ExternalRef |
		| abcd1                    | BASE        | 100           | abcd        |
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: Clash_ExposureCountGreaterThenParents

Scenario: BulkClashCreatedOrUpdated successfully updates data
	Given The Clashes data exists in GamePlan database
		| Uid                                  | ParentExternalIdentifier | Description  | DefaultOffPeakExposureCount | ExternalRef |
		| 6942de2b-4e62-4bb0-baf1-65102c346865 |                          | first clash  | 1                           | abcd        |
		| 6942de2b-4e62-4bb0-baf1-65102c346866 |                          | second clash | 1                           | abce        |
		| 6942de2b-4e62-4bb0-baf1-65102c346867 |                          | third clash  | 10                          | abcd1       |
	And I have BulkClashCreatedOrUpdated message to publish
		| ParentExternalIdentifier | Description | ExposureCount | ExternalRef |
		|                          | test        | 10            | abcd        |
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Clashes data in GamePlan database is updated as following
		| Uid                                  | ParentExternalIdentifier | Description  | DefaultOffPeakExposureCount | ExternalRef |
		| 6942de2b-4e62-4bb0-baf1-65102c346865 |                          | test         | 10                          | abcd        |
		| 6942de2b-4e62-4bb0-baf1-65102c346866 |                          | second clash | 1                           | abce        |
		| 6942de2b-4e62-4bb0-baf1-65102c346867 |                          | third clash  | 10                          | abcd1       |

		Scenario: ClashUpdated model is invalid
	Given I have ClashUpdated message to publish
		| ParentExternalIdentifier | Description | ExposureCount | ExternalRef |
		|                          |             | 0             | asdas       |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName  |
		| ExposureCount |
		| Description   |

Scenario: ClashUpdated validation fails same externalRef
	Given I have ClashUpdated message to publish
		| ParentExternalIdentifier | Description | ExposureCount | ExternalRef |
		| asdas                    |             | 0             | asdas       |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName          |
		| ExposureCount         |
		| Description           |
		| SameParentExternalRef |

Scenario: ClashUpdated published with non-existing ExternalRef
	Given The Clashes data exists in GamePlan database
		| Uid                                  | ParentExternalIdentifier | Description  | DefaultOffPeakExposureCount | ExternalRef |
		| 6942de2b-4e62-4bb0-baf1-65102c346865 |                          | first clash  | 1                           | abcd        |
		| 6942de2b-4e62-4bb0-baf1-65102c346866 |                          | second clash | 1                           | abce        |
		| 6942de2b-4e62-4bb0-baf1-65102c346867 |                          | third clash  | 10                          | abcd1       |
	And I have ClashUpdated message to publish
		| ParentExternalIdentifier | Description | ExposureCount | ExternalRef |
		| hi                       | BASE        | 10            | 0000        |
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: ExternalReferenceNotFound

Scenario: ClashUpdated published with non-existing ParentExternalIdentifier
	Given The Clashes data exists in GamePlan database
		| Uid                                  | ParentExternalIdentifier | Description  | DefaultOffPeakExposureCount | ExternalRef |
		| 6942de2b-4e62-4bb0-baf1-65102c346865 |                          | first clash  | 1                           | abcd        |
		| 6942de2b-4e62-4bb0-baf1-65102c346866 |                          | second clash | 1                           | abce        |
		| 6942de2b-4e62-4bb0-baf1-65102c346867 |                          | third clash  | 10                          | abcd1       |
	And I have ClashUpdated message to publish
		| ParentExternalIdentifier | Description | ExposureCount | ExternalRef |
		| hi                       | BASE        | 10            | abcd        |
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: Clash_ParentDoesNotExist

Scenario: ClashUpdated published with higher ExposureCount then parent's ExposureCount
	Given The Clashes data exists in GamePlan database
		| Uid                                  | ParentExternalIdentifier | Description  | DefaultOffPeakExposureCount | ExternalRef |
		| 6942de2b-4e62-4bb0-baf1-65102c346865 |                          | first clash  | 1                           | abcd        |
		| 6942de2b-4e62-4bb0-baf1-65102c346866 |                          | second clash | 1                           | abce        |
		| 6942de2b-4e62-4bb0-baf1-65102c346867 |                          | third clash  | 10                          | abcd1       |
	And I have ClashUpdated message to publish
		| ParentExternalIdentifier | Description | ExposureCount | ExternalRef |
		| abcd1                    | BASE        | 100           | abcd        |
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: Clash_ExposureCountGreaterThenParents

Scenario: ClashUpdated successfully updates data
	Given The Clashes data exists in GamePlan database
		| Uid                                  | ParentExternalIdentifier | Description  | DefaultOffPeakExposureCount | ExternalRef |
		| 6942de2b-4e62-4bb0-baf1-65102c346865 |                          | first clash  | 1                           | abcd        |
		| 6942de2b-4e62-4bb0-baf1-65102c346866 |                          | second clash | 1                           | abce        |
		| 6942de2b-4e62-4bb0-baf1-65102c346867 |                          | third clash  | 10                          | abcd1       |
	And I have ClashUpdated message to publish
		| ParentExternalIdentifier | Description | ExposureCount | ExternalRef |
		|                          | test        | 10            | abcd        |
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Clashes data in GamePlan database is updated as following
		| Uid                                  | ParentExternalIdentifier | Description  | DefaultOffPeakExposureCount | ExternalRef |
		| 6942de2b-4e62-4bb0-baf1-65102c346865 |                          | test         | 10                          | abcd        |
		| 6942de2b-4e62-4bb0-baf1-65102c346866 |                          | second clash | 1                           | abce        |
		| 6942de2b-4e62-4bb0-baf1-65102c346867 |                          | third clash  | 10                          | abcd1       |

Scenario: BulkClashDeleted Externalref is empty thows model validation exception
	Given I have BulkClashDeleted message to publish
		| Externalref |
		|             |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName        |
		| Data[0].Externalref |

Scenario: BulkClashDeleted deletes clash successfully
	Given The Clashes data exists in GamePlan database
		| Uid                                  | ParentExternalIdentifier | Description  | DefaultOffPeakExposureCount | ExternalRef |
		| 6942de2b-4e62-4bb0-baf1-65102c346865 |                          | first clash  | 1                           | abcd        |
		| 6942de2b-4e62-4bb0-baf1-65102c346866 |                          | second clash | 1                           | abce        |
		| 6942de2b-4e62-4bb0-baf1-65102c346867 |                          | third clash  | 10                          | abcd1       |
	And I have BulkClashDeleted message to publish
		| Externalref |
		| abcd1       |
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Clashes data in GamePlan database is updated as following
		| ParentExternalIdentifier | Description  | DefaultOffPeakExposureCount | ExternalRef |
		|                          | first clash  | 1                           | abcd        |
		|                          | second clash | 1                           | abce        |

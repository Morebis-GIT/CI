@Exchange @SqlServer
Feature: Demographic
	In order to synchronize data with xgGamePlan
	As a landmark user
	I want to send data to GamePlan

Background:
	Given GroupTransactionInfo for 1 event sent

Scenario: BulkDemographicCreatedOrUpdated model is invalid
	Given I have BulkDemographicCreatedOrUpdated message to publish
		| DisplayOrder | ShortName | Name | ExternalRef | Gameplan |
		| 1            |           | name | asdas       | true     |
		| 1            | short     |      | asdas       | true     |
		| 1            | short     | name |             | true     |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName        |
		| Data[0].ShortName   |
		| Data[1].Name        |
		| Data[2].ExternalRef |

Scenario: BulkDemographicCreatedOrUpdated model with empty data is invalid
	Given I have BulkDemographicCreatedOrUpdated message to publish
		| DisplayOrder | ShortName | Name | ExternalRef | Gameplan |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName |
		| Data         |

Scenario: BulkDemographicCreatedOrUpdated successfully saves Demographics
	Given I have BulkDemographicCreatedOrUpdated message to publish
		| CustomId | DisplayOrder | ShortName | Name | ExternalRef | Gameplan |
		| 1        | 1            | short     | name | asdas       | true     |
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Demographics data in GamePlan database is updated as following
		| Id | DisplayOrder | ShortName | Name | ExternalRef | Gameplan |
		| 1  | 1            | short     | name | asdas       | true     |

Scenario: DemographicUpdated model is invalid
	Given I have DemographicUpdated message to publish
		| DisplayOrder | ShortName | Name | ExternalRef | Gameplan |
		| 1            |           |      |             | true     |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName |
		| ShortName    |
		| Name         |
		| ExternalRef  |

Scenario: DemographicUpdated sent with non-existing external Reference
	Given The Demographics data exists in GamePlan database
		| DisplayOrder | ShortName | Name        | ExternalRef | Gameplan |
		| 1            | first     | first demo  | ref1        | true     |
		| 1            | second    | second demo | ref2        | true     |
	And I have DemographicUpdated message to publish
		| DisplayOrder | ShortName | Name        | ExternalRef | Gameplan |
		| 1            | changed   | nameChanged | ref_01      | true     |
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: ExternalReferenceNotFound

Scenario: DemographicUpdated successfully updates Demographics
	Given The Demographics data exists in GamePlan database
		| Id | DisplayOrder | ShortName | Name        | ExternalRef | Gameplan |
		| 1 | 1            | first     | first demo  | ref1        | true     |
		| 2 | 1            | second    | second demo | ref2        | true     |
	And I have DemographicUpdated message to publish
		| Id | DisplayOrder | ShortName | Name        | ExternalRef | Gameplan |
		| 1  | 1            | changed   | nameChanged | ref1        | true     |
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Demographics data in GamePlan database is updated as following
		| Id | DisplayOrder | ShortName | Name        | ExternalRef | Gameplan |
		| 1  | 1            | changed   | nameChanged | ref1        | true     |
		| 2  | 1            | second    | second demo | ref2        | true     |

Scenario: BulkDemographicDeleted model is invalid
	Given I have BulkDemographicDeleted message to publish
		| ExternalRef |
		|             |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName        |
		| Data[0].ExternalRef |

Scenario: BulkDemographicDeleted successfully deletes Demographics
	Given The Demographics data exists in GamePlan database
		| Id | DisplayOrder | ShortName | Name        | ExternalRef | Gameplan |
		| 1  | 1            | first     | first demo  | ref1        | true     |
		| 2  | 1            | second    | second demo | ref2        | true     |
	And I have BulkDemographicDeleted message to publish
		| ExternalRef |
		| ref1        |
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Demographics data in GamePlan database is updated as following
		| Id | DisplayOrder | ShortName | Name        | ExternalRef | Gameplan |
		| 2  | 1            | second    | second demo | ref2        | true     |

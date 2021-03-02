@Exchange @SqlServer
Feature: ProgrammeClassification
	In order to synchronize data with xgGamePlan
	As a landmark user
	I want to send data to GamePlan

Background:
	Given GroupTransactionInfo for 1 event sent

Scenario: ProgrammeClassification messages should successfully save records
	Given I have BulkProgrammeClassificationCreated message to publish
		| Uid | Code  | Description  |
		| 1   | code1 | description1 |
		| 2   | code2 | description2 |
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the ProgrammeClassifications data in GamePlan database is updated as following
		| Uid | Code  | Description  |
		| 1   | code1 | description1 |
		| 2   | code2 | description2 |

Scenario: ProgrammeClassification messages should throw exception for duplicate uid
	Given The data from file ProgrammeClassification.ProgrammeClassification_Setup exists in database
	And I have BulkProgrammeClassificationCreated message to publish
		| Uid | Code  | Description  |
		| 1   | code2 | description2 |
		| 3   | code3 | description3 |
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: UniqueConstraintViolation

Scenario: ProgrammeClassification messages should throw exception for duplicate code
	Given The data from file ProgrammeClassification.ProgrammeClassification_Setup exists in database
	And I have BulkProgrammeClassificationCreated message to publish
		| Uid | Code  | Description  |
		| 2   | code1 | description2 |
		| 3   | code3 | description3 |
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: UniqueConstraintViolation

Scenario: ProgrammeClassification messages should throw exception for duplicate description
	Given The data from file ProgrammeClassification.ProgrammeClassification_Setup exists in database
	And I have BulkProgrammeClassificationCreated message to publish
		| Uid | Code  | Description  |
		| 2   | code2 | description1 |
		| 3   | code3 | description3 |
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: UniqueConstraintViolation

Scenario: ProgrammeClassification messages should throw exception for duplicate passed properties
	Given I have BulkProgrammeClassificationCreated message to publish
		| Uid | Code     | Description  |	
		| 1   | codeSame | description1 |
		| 2   | codeSame | description2 |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName |
		| Data	       |

Scenario: DeleteAllProgrammeClassification message should successfully truncate table
	Given The data from file ProgrammeClassification.ProgrammeClassification_Setup exists in database
	And I have DeleteAllProgrammeClassification message to publish
		||
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	Then the ProgrammeClassifications data in GamePlan database is updated as following
		||

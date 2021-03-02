@Exchange @SqlServer
Feature: Universe
	In order to synchronize data with xgGamePlan
	As a landmark user
	I want to send data to GamePlan

Background:
	Given GroupTransactionInfo for 1 event sent


Scenario: Published BulkUniverseCreated message should create recordexists in database
	Given The data from file Universe.BulkUniverseCreated.Setup exists in database
	And I have BulkUniverseCreated message from file Universe.BulkUniverseCreated.SuccessPath to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Universes data in GamePlan database is updated as the data from the following file Universe.BulkUniverseCreated.Result

Scenario: Published BulkUniverseCreated message should throw validation exception for empty properties
	Given I have BulkUniverseCreated message from file Universe.BulkUniverseCreated.ValidationError to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName          |
		| Data[0].SalesArea     |
		| Data[0].Demographic   |
		| Data[0].StartDate     |
		| Data[0].UniverseValue |

Scenario: Published BulkUniverseCreated message should throw validation exception for start date greater than end date
	Given I have BulkUniverseCreated message from file Universe.BulkUniverseCreated.StartDateValidationError to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName      |
		| Data[0].StartDate |


Scenario: Published BulkProgrammeCreated message should throw exception when sales area is not found
	Given The data from file Universe.BulkUniverseCreated.Setup exists in database
	And I have BulkUniverseCreated message from file Universe.BulkUniverseCreated.Error_SalesAreaNotFound to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: SalesAreaNotFound


Scenario: Published BulkProgrammeCreated message should throw exception when demographic is not found
	Given The data from file Universe.BulkUniverseCreated.Setup exists in database
	And I have BulkUniverseCreated message from file Universe.BulkUniverseCreated.Error_DemographicNotFound to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: DemographicNotFound


Scenario: Published BulkProgrammeCreated message should throw exception for invalid start date
	Given The data from file Universe.BulkUniverseCreated.Setup exists in database
	And I have BulkUniverseCreated message from file Universe.BulkUniverseCreated.Error_InvalidStartDate to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: Universe_StartDateLessThanPredecessors


Scenario: Published BulkUniverseCreated message should update existing record when startdates are equal
	Given The data from file Universe.BulkUniverseCreated.Setup exists in database
	And I have BulkUniverseCreated message from file Universe.BulkUniverseCreated.UpdateRequestModel to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Universes data in GamePlan database is updated as the data from the following file Universe.BulkUniverseCreated.UpdateResult


Scenario: Published BulkProgrammeCreated message should throw exception when gap is more than 1 day
	Given The data from file Universe.BulkUniverseCreated.Setup exists in database
	And I have BulkUniverseCreated message from file Universe.BulkUniverseCreated.Error_InvalidDateGap to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: Universe_GapMoreThan1Day


Scenario: Published BulkProgrammeCreated message should throw exception when dates are incorrect
	Given The data from file Universe.BulkUniverseCreated.Setup exists in database
	And I have BulkUniverseCreated message from file Universe.BulkUniverseCreated.Error_InvalidDates to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: Universe_DateRangeOverlapsPredecessors


Scenario: Published BulkUniverseCreated message should update existing record plus create new one
	Given The data from file Universe.BulkUniverseCreated.Setup exists in database
	And I have BulkUniverseCreated message from file Universe.BulkUniverseCreated.SuccessPath_CreateAndUpdate to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Universes data in GamePlan database is updated as the data from the following file Universe.BulkUniverseCreated.Result_CreateAndUpdate

Scenario: Published BulkUniverseDeleted message should throw validation exception when all parameters are null
	Given I have BulkUniverseDeleted message from file Universe.BulkUniverseDeleted.Error_ParametersValidation to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName        |
		| Data[0].SalesArea   |
		| Data[0].Demographic |
		| Data[0].StartDate   |
		| Data[0].EndDate     |

Scenario: Published BulkUniverseDeleted message should throw validation exception when start date is greater than end date
	Given I have BulkUniverseDeleted message from file Universe.BulkUniverseDeleted.Error_StartDateValidation to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName      |
		| Data[0].StartDate |


Scenario: Published BulkUniverseDeleted message should delete records
	Given The data from file Universe.BulkUniverseDeleted.Setup exists in database
	And I have BulkUniverseDeleted message from file Universe.BulkUniverseDeleted.SuccessPath to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Universes data in GamePlan database is updated as the data from the following file Universe.BulkUniverseDeleted.Result

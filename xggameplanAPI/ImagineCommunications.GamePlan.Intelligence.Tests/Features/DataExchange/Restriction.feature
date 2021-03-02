@Exchange @SqlServer
Feature: Restriction
	In order to synchronize data with xgGamePlan
	As a landmark user
	I want to send data to GamePlan

Background:
	Given GroupTransactionInfo for 1 event sent

Scenario: BulkRestrictionCreatedOrUpdated message should throw validation exception for empty properties
	Given I have BulkRestrictionCreatedOrUpdated message from file Restriction.BulkRestrictionCreatedOrUpdated.Error_InvalidData to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName            |
		| Data[0].StartDate       |
		| Data[0].RestrictionDays |

Scenario: BulkRestrictionCreatedOrUpdated message should throw validation exception when RestrictionBasis equals Product and RestrictionType equals Time
	Given I have BulkRestrictionCreatedOrUpdated message from file Restriction.BulkRestrictionCreatedOrUpdated.Error_RestrictionBasis_Product_RestrictionType_Time to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName                    |
		| Data[0].ProductCode             |
		| Data[0].ClashCode               |
		| Data[0].ClearanceCode           |
		| Data[0].ExternalProgRef         |
		| Data[0].ProgrammeCategory       |
		| Data[0].ProgrammeClassification |
		| Data[0].IndexType               |
		| Data[0].IndexThreshold          |

Scenario: BulkRestrictionCreatedOrUpdated message should throw validation exception when RestrictionBasis equals Clash and RestrictionType equals Programme
	Given I have BulkRestrictionCreatedOrUpdated message from file Restriction.BulkRestrictionCreatedOrUpdated.Error_RestrictionBasis_Clash_RestrictionType_Programme to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName                    |
		| Data[0].ClashCode               |
		| Data[0].ProductCode             |
		| Data[0].ClockNumber             |
		| Data[0].ClearanceCode           |
		| Data[0].ExternalProgRef         |
		| Data[0].ProgrammeCategory       |
		| Data[0].ProgrammeClassification |
		| Data[0].IndexType               |
		| Data[0].IndexThreshold          |

Scenario: BulkRestrictionCreatedOrUpdated message should throw validation exception when RestrictionBasis equals ClearanceCode and RestrictionType equals ProgrammeCategory
	Given I have BulkRestrictionCreatedOrUpdated message from file Restriction.BulkRestrictionCreatedOrUpdated.Error_RestrictionBasis_ClearanceCode_RestrictionType_ProgrammeCategory to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName                    |
		| Data[0].ClearanceCode           |
		| Data[0].ProductCode             |
		| Data[0].ClockNumber             |
		| Data[0].ClashCode               |
		| Data[0].ProgrammeCategory       |
		| Data[0].ExternalProgRef         |
		| Data[0].IndexType               |
		| Data[0].IndexThreshold          |
		| Data[0].ProgrammeClassification |

Scenario: BulkRestrictionCreatedOrUpdated message should throw validation exception when RestrictionBasis equals Null and RestrictionType equals Index
	Given I have BulkRestrictionCreatedOrUpdated message from file Restriction.BulkRestrictionCreatedOrUpdated.Error_RestrictionBasis_Null_RestrictionType_Index to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName                    |
		| Data[0].RestrictionBasis        |
		| Data[0].IndexThreshold          |
		| Data[0].ExternalProgRef         |
		| Data[0].ProgrammeCategory       |
		| Data[0].ProgrammeClassification |

Scenario: BulkRestrictionCreatedOrUpdated message should throw validation exception when RestrictionBasis equals Null and RestrictionType equals ProgrammeClassification
	Given I have BulkRestrictionCreatedOrUpdated message from file Restriction.BulkRestrictionCreatedOrUpdated.Error_RestrictionBasis_Null_RestrictionType_ProgrammeClassification to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName                    |
		| Data[0].RestrictionBasis        |
		| Data[0].ProgrammeClassification |
		| Data[0].ExternalProgRef         |
		| Data[0].IndexType               |
		| Data[0].IndexThreshold          |
		| Data[0].ProgrammeCategory       |

Scenario: BulkRestrictionCreatedOrUpdated message should throw exception when sales area is not found
	Given I have BulkRestrictionCreatedOrUpdated message from file Restriction.BulkRestrictionCreatedOrUpdated.Error_SalesAreaNotFound to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: SalesAreaNotFound

Scenario: BulkRestrictionCreatedOrUpdated message should throw exception when programme category is not found in metadata table
	Given The data from file Restriction.BulkRestrictionCreatedOrUpdated.Setup exists in database
	And I have BulkRestrictionCreatedOrUpdated message from file Restriction.BulkRestrictionCreatedOrUpdated.Error_ProgrammeCategoryNotFound to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: ProgrammeCategoryNotFound

Scenario: BulkRestrictionCreatedOrUpdated message should throw exception when clearance code is not found
	Given The data from file Restriction.BulkRestrictionCreatedOrUpdated.Setup exists in database
	And I have BulkRestrictionCreatedOrUpdated message from file Restriction.BulkRestrictionCreatedOrUpdated.Error_ClearanceCodeNotFound to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: ClearanceCodeNotFound

Scenario: BulkRestrictionCreatedOrUpdated message should create record in the database
	Given The data from file Restriction.BulkRestrictionCreatedOrUpdated.Setup exists in database
	And I have BulkRestrictionCreatedOrUpdated message from file Restriction.BulkRestrictionCreatedOrUpdated.Success_Create to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Restrictions data in GamePlan database is updated as the data from the following file Restriction.BulkRestrictionCreatedOrUpdated.Success_Create_Result

Scenario: BulkRestrictionCreatedOrUpdated message should update record in the database
	Given The data from file Restriction.BulkRestrictionCreatedOrUpdated.Setup exists in database
	And I have BulkRestrictionCreatedOrUpdated message from file Restriction.BulkRestrictionCreatedOrUpdated.Success_Update to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Restrictions data is replaced as in the following file Restriction.BulkRestrictionCreatedOrUpdated.Success_Update_Result

Scenario: BulkRestrictionDeleted message should throw model validation exception for properties
	Given I have BulkRestrictionDeleted message to publish
		| ExternalReference |
		|                   |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName              |
		| Data[0].ExternalReference |

Scenario: BulkRestrictionDeleted message should delete record from database
	Given The data from file Restriction.BulkRestrictionDeleted.Setup exists in database
	And I have BulkRestrictionDeleted message to publish
		| ExternalReference |
		| space66           |
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Restrictions data in GamePlan database is updated as the data from the following file Restriction.BulkRestrictionDeleted.Result

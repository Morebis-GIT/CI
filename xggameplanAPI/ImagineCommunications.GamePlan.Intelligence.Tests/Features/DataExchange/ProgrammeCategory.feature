@Exchange @SqlServer
Feature: ProgrammeCategory
	In order to synchronize data with xgGamePlan
	As a landmark user
	I want to send data to GamePlan

Background:
	Given GroupTransactionInfo for 1 event sent

Scenario: ProgrammeCategory messages should successfully save records
	Given I have BulkProgrammeCategoryCreated message to publish
		| Name     | ExternalRef | ParentExternalRef |
		| ALL      |             |                   |
		| CHILDREN | CHD         | PARENT            |
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the ProgrammeCategoryHierarchies data in GamePlan database is updated as following
		| Name     | ExternalRef | ParentExternalRef |
		| ALL      |             |                   |
		| CHILDREN | CHD         | PARENT            |

Scenario: ProgrammeCategory messages should throw exception for duplicate name
	Given The data from file ProgrammeCategory.ProgrammeCategory_Setup exists in database
	And I have BulkProgrammeCategoryCreated message to publish
		| Name     | ExternalRef | ParentExternalRef |
		| CHILDREN |             |                   |
		| OTHER    |             |                   |
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: UniqueConstraintViolation

Scenario: ProgrammeCategory messages should throw exception for duplicate parent and external reference in ProgrammeCategory
	Given I have BulkProgrammeCategoryCreated message to publish
		| Name     | ExternalRef | ParentExternalRef |
		| ALL      | PARENT      |                   |
		| CHILDREN | PARENT      | PARENT            |
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: DuplicateParentAndExternalReference

Scenario: DeleteAllProgrammeCategory message should successfully truncate table
	Given The data from file ProgrammeCategory.ProgrammeCategory_Setup exists in database
	And I have BulkProgrammeCategoryDeleted message to publish
		||
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	Then the ProgrammeCategoryHierarchies data in GamePlan database is updated as following
		||

Scenario: BulkProgrammeCategoryCreated invalid model
	Given I have BulkProgrammeCategoryCreated message to publish
		| Data |
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName |
		| Data         |

Scenario: BulkProgrammeCategoryCreated model fileds are invalid
	Given I have BulkProgrammeCategoryCreated message to publish
		| Name | ExternalRef | ParentExternalRef |
		|      | PARENT      | PARENT            |      
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName |
		| Data[0].Name |     



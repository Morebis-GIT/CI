@Exchange @SqlServer
Feature: Campaign
	In order to synchronize data with xgGamePlan
	As a landmark user
	I want to send data to GamePlan

Background:
	Given GroupTransactionInfo for 1 event sent

# CampaignCreatedOrUpdated
Scenario: BulkCampaignCreatedOrUpdated message should throw model validation exception for campaign properties
	Given I have BulkCampaignCreatedOrUpdated message from file Campaign.BulkCampaignCreatedOrUpdated.Error_InvalidCampaignData to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName                    |
		| Data[0].ExternalId              |
		| Data[0].Name                    |
		| Data[0].DemoGraphic             |
		| Data[0].StartDateTime           |
		#| Data[0].EndDateTime             |
		| Data[0].EndDateTime             |
		| Data[0].Product                 |
		| Data[0].IncludeRightSizer       |
		| Data[0].CampaignGroup           |
		| Data[0].CampaignSpotMaxRatings  |
		| Data[0].BreakType               |
		| Data[0].SalesAreaCampaignTarget |

Scenario: BulkCampaignCreatedOrUpdated message should throw model validation exception for sales area campaign target properties
	Given I have BulkCampaignCreatedOrUpdated message from file Campaign.BulkCampaignCreatedOrUpdated.Error_InvalidSalesAreaCampaignTargetData to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName                                       |
		| Data[0].SalesAreaCampaignTarget[0].Multiparts      |
		| Data[0].SalesAreaCampaignTarget[0].CampaignTargets |

Scenario: BulkCampaignCreatedOrUpdated message should throw model validation exception for multipart, campaign target and strike weight properties
	Given I have BulkCampaignCreatedOrUpdated message from file Campaign.BulkCampaignCreatedOrUpdated.Error_InvalidMultipartAndCampaignTargetStrikeWeightData to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName                                                                                  |
		| Data[0].SalesAreaCampaignTarget[0].Multiparts[0].Lengths                                      |
		| Data[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].StartDate              |
		| Data[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].EndDate                |
		| Data[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].SpotMaxRatings         |
		| Data[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].DayParts               |
		| Data[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].DesiredPercentageSplit |
		| Data[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].CurrentPercentageSplit |

Scenario: BulkCampaignCreatedOrUpdated message should throw model validation exception for length, day part and timeslice properties
	Given I have BulkCampaignCreatedOrUpdated message from file Campaign.BulkCampaignCreatedOrUpdated.Error_InvalidLengthDayPartAndTimeslice to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName                                                                                                |
		| Data[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].Lengths[0].length                    |
		| Data[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].DayParts[0].SpotMaxRatings           |
		| Data[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].DayParts[0].DesiredPercentageSplit   |
		| Data[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].DayParts[0].CurrentPercentageSplit   |
		| Data[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].DayParts[0].Timeslices[0].FromTime   |
		| Data[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].DayParts[0].Timeslices[0].FromTime   |
		| Data[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].DayParts[0].Timeslices[0].ToTime     |
		| Data[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].DayParts[0].Timeslices[0].ToTime     |
		| Data[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].DayParts[0].Timeslices[0].DowPattern |

Scenario: BulkCampaignCreatedOrUpdated message should throw model validation exception for time restrictions properties
	Given I have BulkCampaignCreatedOrUpdated message from file Campaign.BulkCampaignCreatedOrUpdated.Error_InvalidTimeRestrictions to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName                                   |
		| Data[0].TimeRestrictions[0].StartDateTime      |
		#| Data[0].TimeRestrictions[0].StartDateTime      |
		| Data[0].TimeRestrictions[0].EndDateTime        |
		| Data[0].TimeRestrictions[0].IsIncludeOrExclude |
		| Data[0].TimeRestrictions[0].DowPattern[0]      |

Scenario: BulkCampaignCreatedOrUpdated message should throw model validation exception for programme restriction properties
	Given I have BulkCampaignCreatedOrUpdated message from file Campaign.BulkCampaignCreatedOrUpdated.Error_InvalidProgrammeRestriction to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName                                           |
		| Data[0].ProgrammeRestrictions[0].IsCategoryOrProgramme |
		| Data[0].ProgrammeRestrictions[0].IsIncludeOrExclude    |

Scenario: BulkCampaignCreatedOrUpdated message should throw exception when IncludeRightSizer is invalid
	Given I have BulkCampaignCreatedOrUpdated message from file Campaign.BulkCampaignCreatedOrUpdated.Error_InvalidIncludeRightSizer to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: InvalidRightSizer


Scenario: BulkCampaignCreatedOrUpdated message should throw exception when CampaignPassPriority is invalid
	Given The data from file Campaign.BulkCampaignCreatedOrUpdated.Setup exists in database
	And I have BulkCampaignCreatedOrUpdated message from file Campaign.BulkCampaignCreatedOrUpdated.Error_InvalidCampaignPassPriority to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: InvalidCampaignPassPriority


Scenario: BulkCampaignCreatedOrUpdated message should throw exception when demographic is not found
	Given The data from file Campaign.BulkCampaignCreatedOrUpdated.Setup exists in database
	And I have BulkCampaignCreatedOrUpdated message from file Campaign.BulkCampaignCreatedOrUpdated.Error_DemographicNotFound to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: DemographicNotFound


Scenario: BulkCampaignCreatedOrUpdated message should throw exception when product is not found
	Given The data from file Campaign.BulkCampaignCreatedOrUpdated.Setup exists in database
	And I have BulkCampaignCreatedOrUpdated message from file Campaign.BulkCampaignCreatedOrUpdated.Error_ProductNotFound to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: ProductNotFound


Scenario: BulkCampaignCreatedOrUpdated message should throw exception when BreakType is invalid
	Given The data from file Campaign.BulkCampaignCreatedOrUpdated.Setup exists in database
	And I have BulkCampaignCreatedOrUpdated message from file Campaign.BulkCampaignCreatedOrUpdated.Error_InvalidBreakType to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: InvalidBreakType


Scenario: BulkCampaignCreatedOrUpdated message should throw exception when sales areas are not found
	Given The data from file Campaign.BulkCampaignCreatedOrUpdated.Setup exists in database
	And I have BulkCampaignCreatedOrUpdated message from file Campaign.BulkCampaignCreatedOrUpdated.Error_SalesAreasNotFound to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: SalesAreaNotFound


Scenario: BulkCampaignCreatedOrUpdated message should throw exception when ProgrammeCategory is not found
	Given The data from file Campaign.BulkCampaignCreatedOrUpdated.Setup exists in database
	And I have BulkCampaignCreatedOrUpdated message from file Campaign.BulkCampaignCreatedOrUpdated.Error_ProgrammeCategoryNotFound to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws exception with the code: ProgrammeCategoryNotFound


Scenario: BulkCampaignCreatedOrUpdated message should successfully create record in the database
	Given The data from file Campaign.BulkCampaignCreatedOrUpdated.Setup exists in database
	And I have BulkCampaignCreatedOrUpdated message from file Campaign.BulkCampaignCreatedOrUpdated.Success_Create_Request to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Campaigns data in GamePlan database is updated as the data from the following file Campaign.BulkCampaignCreatedOrUpdated.Success_Create_Result


Scenario: BulkCampaignCreatedOrUpdated message should successfully update record in the database
	Given The data from file Campaign.BulkCampaignCreatedOrUpdated.Setup exists in database
	And I have BulkCampaignCreatedOrUpdated message from file Campaign.BulkCampaignCreatedOrUpdated.Success_Update_Request to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Campaigns data in GamePlan database is updated as the data from the following file Campaign.BulkCampaignCreatedOrUpdated.Success_Update_Result

# BulkCampaignDeleted
Scenario: BulkCampaignDeleted message should throw model validation exception
	Given I have BulkCampaignDeleted message from file Campaign.BulkCampaignDeleted.Error_InvalidModel to publish
	When I publish message to message broker
	Then GamePlanIntelligence throws ContractValidation exception for following properties
		| PropertyName       |
		| Data[0].ExternalId |


Scenario: BulkCampaignDeleted message should successfully delete record from the database
	Given The data from file Campaign.BulkCampaignDeleted.Setup exists in database
	And I have BulkCampaignDeleted message from file Campaign.BulkCampaignDeleted.Success_Request to publish
	When I publish message to message broker
	Then GamePlanIntelligence consumes message
	And the Campaigns data in GamePlan database is updated as the data from the following file Campaign.BulkCampaignDeleted.Success_Result
	And the Scenarios data in GamePlan database is updated as the data from the following file Campaign.BulkCampaignDeleted.Success_Scenario_Result

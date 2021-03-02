@ManageDataStorage

Feature: ManageRecommendationsDataStorage
	In order to manage Recommendations
	As an Airtime manager
	I want to store Recommendations via Recommendations repository

Background: 
	Given there is a Recommendations repository

Scenario: Add new Recommendations
	Given the following documents created:
		| ScenarioId                           | Processor | ExternalCampaignNumber |
		| 00c1a5cc-c6c3-48b5-b193-2c1fd9e29833 | autobook  | 15G1557673             |
		| 00c1a5cc-c6c3-48b5-b193-2c1fd9e29833 | autobook  | 30G1564969             |
	When I call GetByScenarioId method with parameters:
		| Parameter  | Value                                |
		| scenarioId | 00c1a5cc-c6c3-48b5-b193-2c1fd9e29833 |
	Then there should be 2 documents returned

Scenario Outline: Get Recommendation Simples by ScenarioId and Processors
	Given the following documents created:
        | ScenarioId                           | Processor | ExternalCampaignNumber |
        | 00c1a5cc-c6c3-48b5-b193-2c1fd9e29833 | autobook  | 15G1557673             |
        | 00c1a5cc-c6c3-48b5-b193-2c1fd9e29833 | autobook  | 30G1564969             |
        | b1e74691-4d6e-426f-b9c0-b0e792ca42a2 | smooth    | H1563709               |
        | b1e74691-4d6e-426f-b9c0-b0e792ca42a2 | smooth    | H1563386               |
        | b1e74691-4d6e-426f-b9c0-b0e792ca42a2 | rzr       | 15G1561842             |
	When I call GetRecommendationSimplesByScenarioIdAndProcessors method with parameters:
		| Parameter  | Value        |
		| scenarioId | <ScenarioId> |
		| processors | <Processors> |
	Then there should be <ExpectedCount> documents returned
	Examples: 
		| ScenarioId                           | Processors  | ExpectedCount |
		| 00c1a5cc-c6c3-48b5-b193-2c1fd9e29833 | autobook    | 2             |
		| b1e74691-4d6e-426f-b9c0-b0e792ca42a2 | rzr         | 1             |
		| b1e74691-4d6e-426f-b9c0-b0e792ca42a2 | smooth, rzr | 3             |
		| 00000000-0000-0000-0000-000000000000 | smooth, rzr | 0             |
		| b1e74691-4d6e-426f-b9c0-b0e792ca42a2 | AAA         | 0             |

Scenario Outline: Get Recommendations by ScenarioId and Processors
	Given the following documents created:
		| ScenarioId                           | Processor | ExternalCampaignNumber |
		| 00c1a5cc-c6c3-48b5-b193-2c1fd9e29833 | autobook  | 15G1557673             |
		| 00c1a5cc-c6c3-48b5-b193-2c1fd9e29833 | autobook  | 30G1564969             |
		| b1e74691-4d6e-426f-b9c0-b0e792ca42a2 | smooth    | H1563709               |
		| b1e74691-4d6e-426f-b9c0-b0e792ca42a2 | smooth    | H1563386               |
		| b1e74691-4d6e-426f-b9c0-b0e792ca42a2 | rzr       | 15G1561842             |
	When I call GetByScenarioIdAndProcessors method with parameters:
		| Parameter  | Value        |
		| scenarioId | <ScenarioId> |
		| processors | <Processors> |
	Then there should be <ExpectedCount> documents returned
	Examples: 
		| ScenarioId                           | Processors  | ExpectedCount |
		| 00c1a5cc-c6c3-48b5-b193-2c1fd9e29833 | autobook    | 2             |
		| b1e74691-4d6e-426f-b9c0-b0e792ca42a2 | rzr         | 1             |
		| b1e74691-4d6e-426f-b9c0-b0e792ca42a2 | smooth, rzr | 3             |
		| 00000000-0000-0000-0000-000000000000 | smooth, rzr | 0             |
		| b1e74691-4d6e-426f-b9c0-b0e792ca42a2 | AAA         | 0             |

Scenario Outline: Get Campaign Recommendations by ScenarioId
	Given the following documents created:
		| ScenarioId                           | Processor | ExternalCampaignNumber |
		| 00c1a5cc-c6c3-48b5-b193-2c1fd9e29833 | autobook  | 15G1557673             |
		| 00c1a5cc-c6c3-48b5-b193-2c1fd9e29833 | autobook  | 30G1564969             |
		| b1e74691-4d6e-426f-b9c0-b0e792ca42a2 | smooth    | H1563709               |
		| b1e74691-4d6e-426f-b9c0-b0e792ca42a2 | smooth    | H1563386               |
		| b1e74691-4d6e-426f-b9c0-b0e792ca42a2 | rzr       | 15G1561842             |
	When I call GetCampaigns method with parameters:
		| Parameter  | Value        |
		| scenarioId | <ScenarioId> |
	Then there should be <ExpectedCount> documents returned
	Examples: 
		| ScenarioId                           | ExpectedCount |
		| 00c1a5cc-c6c3-48b5-b193-2c1fd9e29833 | 2             |
		| b1e74691-4d6e-426f-b9c0-b0e792ca42a2 | 3             |
		| 00000000-0000-0000-0000-000000000000 | 0             |

Scenario Outline: Get Metrics by ScenarioId and CampaignId
	Given the following documents created:
		| ScenarioId                           | Processor | ExternalCampaignNumber |
		| 00c1a5cc-c6c3-48b5-b193-2c1fd9e29833 | autobook  | 15G1557673             |
		| 00c1a5cc-c6c3-48b5-b193-2c1fd9e29833 | autobook  | 30G1564969             |
		| b1e74691-4d6e-426f-b9c0-b0e792ca42a2 | smooth    | H1563709               |
		| b1e74691-4d6e-426f-b9c0-b0e792ca42a2 | smooth    | H1563386               |
		| b1e74691-4d6e-426f-b9c0-b0e792ca42a2 | rzr       | 15G1561842             |
	When I call GetMetrics method with parameters:
		| Parameter  | Value        |
		| scenarioId | <ScenarioId> |
		| campaignId | <CampaignId> |
	Then there should be <ExpectedCount> documents returned
	Examples: 
		| ScenarioId                           | CampaignId | ExpectedCount |
		| 00c1a5cc-c6c3-48b5-b193-2c1fd9e29833 | 15G1557673 | 1             |
		| b1e74691-4d6e-426f-b9c0-b0e792ca42a2 | H1563709   | 1             |
		| b1e74691-4d6e-426f-b9c0-b0e792ca42a2 | 30G1564969 | 0             |
		| 00000000-0000-0000-0000-000000000000 | 15G1557673 | 0             |

Scenario: Remove Recommendations by ScenarioId
	Given the following documents created:
		| ScenarioId                           | Processor | ExternalCampaignNumber |
		| 00c1a5cc-c6c3-48b5-b193-2c1fd9e29833 | autobook  | 15G1557673             |
		| 00c1a5cc-c6c3-48b5-b193-2c1fd9e29833 | autobook  | 30G1564969             |
	When I call RemoveByScenarioId method with parameters:
		| Parameter  | Value                                |
		| scenarioId | 00c1a5cc-c6c3-48b5-b193-2c1fd9e29833 |
	And I call GetByScenarioId method with parameters:
		| Parameter  | Value                                |
		| scenarioId | 00c1a5cc-c6c3-48b5-b193-2c1fd9e29833 |
	Then there should be 0 documents returned

Scenario: Remove Recommendations by ScenarioId and Processors
	Given the following documents created:
		| ScenarioId                           | Processor | ExternalCampaignNumber |
		| b1e74691-4d6e-426f-b9c0-b0e792ca42a2 | smooth    | H1563709               |
		| b1e74691-4d6e-426f-b9c0-b0e792ca42a2 | smooth    | H1563386               |
		| b1e74691-4d6e-426f-b9c0-b0e792ca42a2 | rzr       | 15G1561842             |
	When I call RemoveByScenarioIdAndProcessors method with parameters:
		| Parameter  | Value                                |
		| scenarioId | b1e74691-4d6e-426f-b9c0-b0e792ca42a2 |
		| processors | smooth, rzr                          |
	And I call GetByScenarioIdAndProcessors method with parameters:
		| Parameter  | Value                                |
		| scenarioId | b1e74691-4d6e-426f-b9c0-b0e792ca42a2 |
		| processors | smooth, rzr                          |
	Then there should be 0 documents returned

@ScenarioResults
Feature: ScenarioResults API
	In order to check the ScenarioResults functionality
	As an API user
	I want to test available ScenarioResults API endpoints

Background:
	Given I have added Test Run Result

Scenario: As an API User I want to request results for scenario
	When I request result for scenario
	Then result is returned

Scenario: As an API User I want to request top failures for scenario
	When I request top failures for scenario
	Then top failures are returned

Scenario: As an API User I want to request failures for scenario
	When I request failures for scenario
	Then failures are returned

Scenario: As an API User I want to request grouped results for scenario
	When I request grouped results by SalesArea for scenario
	Then grouped result is returned

Scenario: As an API User I want to delete scenario result
	When I delete scenario result
	Then no result is returned

Scenario: As an API User I want to request simple recommendations for scenarios
	When I request simple recommendations for scenarios
	Then simple recommendations are returned

Scenario: As an API User I want to request simple recommendations for scenario
	When I request simple recommendations for scenario
	Then simple recommendations are returned

Scenario: As an API User I want to request aggregated recommendations for scenario
	When I request aggregated recommendations for scenario
	Then aggregated recommendations are returned

Scenario: As an API user I want to request valid Output files
	When I request valid files by ids
	Then same as created files are returned

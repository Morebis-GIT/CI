@Runs
Feature: Runs API
	In order to check the Runs functionality
	As an API user
	I want to test available Runs API endpoints

Scenario: As an API User I want to create Runs
	Given I know how many Runs there are
	When I add 3 Runs
	Then 3 additional Runs are returned

Scenario: As an API User I want to search Run by Description
	Given I have added 1 Run
	When I search my Run by Description
	Then requested RunSearchResult with Description is found

Scenario: As an API User I want to request Run by ID
	Given I have added 1 Test Run Result
	When I request my Run by ID
	Then requested Run with ID is returned

Scenario: As an API User I want to update Run by ID
	Given I have added 1 Run
	When I update Run by ID
	Then updated Run is returned

Scenario: As an API User I want to request Runs
	Given I know how many Runs there are
	When I add 2 Test Run Results
	Then 2 additional Runs are returned

Scenario: As an API User I want to request Run metrics
	Given I have added 1 Test Run Result
	When I request metrics by Run ID
	Then requested metrics are returned

Scenario: As an API User I want to generate AutopilotScenarios
	Given I have a valid AutopilotEngage model
	When I configure autopilot to return 6 scenarios
	And I generate autopilot scenarios
	Then Correct number of autopilot scenarios is returned

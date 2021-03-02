@Scenarios
Feature: Scenarios API
	In order to check the Scenarios functionality
	As an API user
	I want to test available Scenarios API endpoints

Scenario: As an API User I want to request Scenarios
	Given I know how many Scenarios there are
	When I add 2 Scenarios
	Then 2 additional Scenarios are returned

Scenario: As an API User I want to request Scenario by ID
	Given I have added 1 Scenario
	When I request my Scenario by ID
	Then requested Scenario with ID is returned

Scenario: As an API User I want to update Scenario by ID
	Given I have added 1 Scenario
	When I update Scenario by ID
	Then updated Scenario is returned

Scenario: As an API User I want to remove Scenario by ID
	Given I have added 1 Scenario
	When I remove my Scenario by ID
	Then no Scenario with ID is returned

Scenario: As an API User I want to set Default Scenario's ID
	Given I have added 1 Scenario
	When I set Default Scenario's ID
	Then Default Scenario's ID is returned

Scenario: As an API User I want to search Scenario by Name
	Given I have added 1 Scenario
	When I search my Scenario by Name
	Then requested Scenario with Name is found

@Passes
Feature: AutopilotSettings API
	In order to check AutopilotSettings functionality
	As an API user
	I want to test available AutopilotSettings API endpoints

Scenario: As an API user I want to request AutopilotSettings
	When I request AutopilotSettings
	Then the method succeeded

Scenario: As an API User I want to update AutopilotSettings by Id
	Given I have a valid AutopilotSettings
	When I update AutopilotSettings by Id
	Then updated AutopilotSettings are returned

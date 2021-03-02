@AnalysisGroups
Feature: AnalysisGroups API
	In order to check the AnalysisGroups functionality
	As an API user
	I want to test available AnalysisGroups API endpoints

Background:
	Given I have a valid Clash

Scenario: As an API User I want to create analysis group
	Given I know how many analysis groups there are
	When I add 1 analysis group
	Then 1 additional analysis groups are returned

Scenario: As an API User I want to update analysis group by Id
	Given I have a valid analysis group
	When I update analysis group by Id
	Then analysis group is updated

Scenario: As an API User I want to delete analysis group
	Given I have a valid analysis group
	When I delete analysis group
	Then analysis group is deleted

Scenario: As an API User I want to get analysis group by Id
	Given I have a valid analysis group
	When I get analysis group by Id
	Then analysis group is returned
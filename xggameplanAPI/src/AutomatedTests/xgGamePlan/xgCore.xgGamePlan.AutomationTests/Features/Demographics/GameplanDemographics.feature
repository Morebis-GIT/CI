@Demographics
Feature: Demographics API
	An an xgGamePlan API user
	I want to manage demographics

Scenario: As an API user I want to request all GamePlan demographics
	Given I know how many GamePlan demographics there are
	When I add 5 GamePlan demographics
	And 4 other demographics
	Then 5 additional GamePlan demographics are returned

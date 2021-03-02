@AutomationTests SmokeTests
Feature: API Connectivity
	In order to use the xgGamePlan API in my UI solution
	As an xgGamePlan UI developer
	I want to know xgGamePlan API version 2 is available

Scenario: The xgGamePlan Test Verb GET is available
	Given I have a URL for an xgGamePlan API
	When I query the API test "GET" Verb
	Then the method succeeded

Scenario: The xgGamePlan Test Verb POST is available
	Given I have a URL for an xgGamePlan API
	When I query the API test "POST" Verb
	Then the method succeeded

Scenario: The xgGamePlan Test Verb PUT is available
	Given I have a URL for an xgGamePlan API
	When I query the API test "PUT" Verb
	Then the method succeeded

Scenario: The xgGamePlan Test Verb DELETE is available
	Given I have a URL for an xgGamePlan API
	When I query the API test "DELETE" Verb
	Then the method succeeded

Scenario: The xgGamePlan Test Verb PATCH is available
	Given I have a URL for an xgGamePlan API
	When I query the API test "PATCH" Verb
	Then the method succeeded

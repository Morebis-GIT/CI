@Passes
Feature: Passes API
	In order to check Passes functionality
	As an API user
	I want to test available Passes API endpoints

Scenario: As an API user I want to request Passes
	Given I know how many Passes there are
	When I add 3 Passes
	Then 3 additional Passes are returned

Scenario: As an API User I want to request Pass by ID
	Given I have added a Pass
	When I request my Pass by ID
	Then requested Pass with ID is returned

Scenario: As an API User I want to update Passes by ID
	Given I have added a Pass
	When I update Pass by ID
	Then updated Pass is returned

Scenario: As an API User I want to remove Passes by ID
	Given I have added a Pass
	When I remove my Pass by ID
	Then no Pass with ID is returned

Scenario: As an API User I want to search Pass in library by Name
	Given I have added a Pass
	When I search my Pass by Name
	Then requested Pass with Name is found

@ClashExceptions
Feature: Clash Exceptions API
	In order to check the Clash Exceptions functionality
	As an API user
	I want to test available Clash Exceptions API endpoints

Background:
	Given I have a valid Clash

Scenario: As an API User I want to request Clash Exceptions
	Given I have removed Clash Exceptions for next two days
	And I know how many Clash Exceptions there are
	When I add 2 Clash Exceptions for next two days
	Then 2 additional Clash Exceptions are returned

Scenario: As an API User I want to update Clash Exception by ID
	Given I have added 1 Clash Exception
	When I update Clash Exception by ID
	Then updated Clash Exception is returned

Scenario: As an API User I want to remove Clash Exception by ID
	Given I have added 1 Clash Exception
	When I remove my Clash Exception by ID
	Then no Clash Exception with ID is returned

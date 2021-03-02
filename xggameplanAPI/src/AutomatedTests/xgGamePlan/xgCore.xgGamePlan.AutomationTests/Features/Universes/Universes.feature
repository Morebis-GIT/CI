@Universes
Feature: Universes API
	In order to check the Universes functionality
	As an API user
	I want to test available Universes API endpoints

Background:
	Given I have a valid Sales Area

Scenario: As an API User I want to request Universes
	Given I know how many Universes there are
	And I have 5 valid Demographics
	When I add 5 Universes
	Then 5 additional Universes are returned

Scenario:  As an API User I want to remove all Universes
	Given I have 10 valid Demographics
	And I have added 10 Universes
	When I delete all Universes
	Then no Universes are returned

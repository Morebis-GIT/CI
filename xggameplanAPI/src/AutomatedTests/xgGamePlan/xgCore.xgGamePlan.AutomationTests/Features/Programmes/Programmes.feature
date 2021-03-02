@Programmes
Feature: Programmes API
	In order to check the Programmes functionality
	As an API user
	I want to test available Programmes API endpoints

Background:
	Given I have a valid Programme Categories
	And I have a valid Sales Area

Scenario: As an API User I want to request Programmes
	Given I know how many Programmes there are
	When I add 3 Programmes
	Then 3 additional Programmes are returned

Scenario:  As an API User I want to remove all Programmes
	Given I have added 5 Programmes
	When I delete all Programmes
	Then no Programmes are returned

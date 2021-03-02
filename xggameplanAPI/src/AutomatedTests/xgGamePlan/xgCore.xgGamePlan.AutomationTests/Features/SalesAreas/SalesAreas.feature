@SalesAreas
Feature: Sales Areas API
	In order to check the Sales Areas functionality
	As an API user
	I want to test available Sales Areas API endpoints

Background:
	Given I have a valid Demographic

Scenario: As an API User I want to request Sales Areas
	Given I know how many Sales Areas there are
	When I add 2 Sales Areas
	Then 2 additional Sales Areas are returned

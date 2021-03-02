@Spots
Feature: Spots API
	In order to check the Spots functionality
	As an API user
	I want to test available Spots API endpoints

Background:
	Given I have a valid Sales Area
	And I have valid date value for Spot and Schedule
	And I have a valid Programme Categories 

Scenario: As an API User I want to create Spots
	Given I know how many Spots there are
	When I add 3 Spots
	Then 3 additional Spots are returned

Scenario: As an API User I want to search Spots
	Given I have added 5 Spots
	And I know how many Spots for SalesArea and date there are
	When I add 5 Spots
	And I search Spots by SalesArea and date
	Then 5 additional Spots are found

Scenario: As an API User I want to search Spots with break and programme details
	Given I have added 5 Spots
	And I have added 5 Schedules
	And I know how many Spots for SalesArea and date there are
	When I add 5 Spots
	When I add 5 Schedules
	And I search Spots with break and programme details by SalesArea and date
	Then 5 additional Spots with break and programme are found

Scenario: As an API User I want to remove Spots by date range and sales areas
	Given I have added 5 Spots with break change
	When I delete Spots by date range and SalesArea
	Then no Spots within date range are returned

Scenario: As an API User I want to remove Spots by external references
	Given I have added 5 Spots
	And I know Spot external references
	When I delete Spots by external references
	Then no Spots are returned

Scenario:  As an API User I want to remove all Spots
	Given I have added 5 Spots
	When I delete all Spots
	Then no Spots are returned

Scenario: As an API User I want to upsert spot by nonexistent external id
	Given I have no Spots with provided external id and spot model
	And I know how many Spots there are
	When I upsert spot with provided external id
	Then 1 additional Spots are returned

Scenario: As an API User I want to update spot by existing external id without break change
	Given I have valid spot data without break change
	When I upsert spot
	Then it is successfully updated without break change

Scenario: As an API User I want to update spot by external id with break change
	Given I have valid spot data with break change
	When I upsert spot
	Then it is successfully updated with break change

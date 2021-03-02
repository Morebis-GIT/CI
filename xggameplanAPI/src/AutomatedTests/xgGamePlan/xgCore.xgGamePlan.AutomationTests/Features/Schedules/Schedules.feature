@Schedules
Feature: Schedules API
	In order to check the Schedules functionality
	As an API user
	I want to test available Schedules API endpoints

Background:
	Given I have a valid Programme Categories
	And I have a valid Break Types "BASE,PREMIUM"
	And I have a valid Sales Area
	And I have a valid 5 days date range

Scenario: As an API User I want to request a range of Programmes within the Schedules
	Given I know how many Programmes within the Schedules for date range
	When I add 3 Programmes for date range
	Then 3 additional Programmes within the Schedules are returned

Scenario: As an API User I want to request a range of Breaks within the Schedules
	Given I know how many Breaks within the Schedules for date range
	When I add 3 Breaks for date range
	Then 3 additional Breaks within the Schedules are returned

Scenario:  As an API User I want to remove all Schedules
	Given I have added 5 Programmes
	And I have added 7 Breaks
	When I delete all Schedules
	Then no Schedules are returned

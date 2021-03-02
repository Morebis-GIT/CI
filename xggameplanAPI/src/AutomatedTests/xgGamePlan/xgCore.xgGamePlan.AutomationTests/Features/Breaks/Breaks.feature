@Breaks
Feature: Breaks API
	In order to check the Breaks functionality
	As an API user
	I want to test available Breaks API endpoints

Background:
	Given I have a valid Break Types "BASE, PREMIUM"
	And I have a valid Sales Area
	And I have a valid 5 days date range

Scenario: As an API User I want to request Breaks
	Given I know how many Breaks there are
	When I add 5 Breaks
	Then 5 additional Breaks are returned

Scenario: As an API User I want to remove Breaks by date range and sales areas
	Given I know how many Breaks within date range there are
	When I add 5 Breaks for date range
	And I delete Breaks by date range
	Then no Breaks within date range are returned

Scenario: As an API User I want to remove all Breaks
	Given I have added 10 Breaks
	When I delete all Breaks
	Then no Breaks are returned

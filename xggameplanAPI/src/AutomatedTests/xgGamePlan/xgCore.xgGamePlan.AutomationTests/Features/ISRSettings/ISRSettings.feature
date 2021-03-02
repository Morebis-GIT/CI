@ISRSettings
Feature: ISRSettings API
	In order to check the ISRSettings functionality
	As an API user
	I want to test available ISRSettings API endpoints

Background:
	Given I have a valid Sales Area

Scenario: As an API User I want to request ISRSettings by sales area
	Given I have added ISRSettings for Sales Area
	When I get ISRSettings for Sales Area
	Then ISRSettings is returned

Scenario: As an API User I want to compare ISRSettings
	Given I have added ISRSettings for Sales Area
	When I compare ISRSettings
	Then compare result is returned

Scenario: As an API User I want to update ISRSettings
	Given I have added ISRSettings for Sales Area
	When I update ISRSettings
	Then updated ISRSettings is returned

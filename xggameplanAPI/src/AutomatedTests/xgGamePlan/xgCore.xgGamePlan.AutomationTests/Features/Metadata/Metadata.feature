@Metadata
Feature: Metadata API
	In order to check the Metadata functionality
	As an API user
	I want to test available Metadata API endpoints
Scenario: As an API User I want to save Metadata
	Given I have added 3 values for type BreakTypes
	When I add 4 Metadata values
	Then only 4 Metadata values are returned	 
Scenario:  As an API User I want to remove Metadata
	Given I have added 3 values for type BreakTypes
	When I delete Metadata for type BreakTypes
	Then no Metadata values are returned

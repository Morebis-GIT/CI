@ClearanceCode
Feature: Clearance Code API
	In order to check the Clearance Code functionality
	As an API user
	I want to test available Clearance Code API endpoints

Scenario: As an API User I want to request Clearance Codes
	Given I know how many Clearance Codes there are
	When I add 2 Clearance Codes
	Then 2 additional Clearance Codes are returned

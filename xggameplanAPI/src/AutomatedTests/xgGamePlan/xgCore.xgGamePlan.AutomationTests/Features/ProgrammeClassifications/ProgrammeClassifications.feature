@ProgrammeClassifications
Feature: ProgrammeClassifications API
	In order to check the ProgrammeClassifications functionality
	As an API user
	I want to test available ProgrammeClassifications API endpoints

Scenario: As an API user I want to request all ProgrammeClassifications
	Given I know how many ProgrammeClassifications there are
	When I add 4 ProgrammeClassifications
	Then 4 additional ProgrammeClassifications are returned

Scenario:  As an API User I want to remove all ProgrammeClassifications
	Given I have added 10 ProgrammeClassifications
	When I delete all ProgrammeClassifications
	Then no ProgrammeClassifications are returned

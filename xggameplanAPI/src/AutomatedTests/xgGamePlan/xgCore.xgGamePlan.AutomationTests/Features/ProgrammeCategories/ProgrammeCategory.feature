@ProgrammeCategories
Feature: ProgrammeCategory API
	In order to check the ProgrammeCategories functionality
	As an API user
	I want to test available ProgrammeCategories API endpoints

Scenario: As an API user I want to request all ProgrammeCategories
	Given I know how many ProgrammeCategories there are
	When I add 4 ProgrammeCategories
	Then 4 additional ProgrammeCategories are returned

Scenario:  As an API User I want to remove all ProgrammeCategories
	Given I have added 4 ProgrammeCategories
	When I delete all ProgrammeCategories
	Then no ProgrammeCategories are returned
